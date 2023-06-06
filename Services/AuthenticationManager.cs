
using AutoMapper;
using Entities.DataTransferObject;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class AuthenticationManager : IAuthenticationService
    {
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _user;
        private readonly IConfiguration _configuration;

        private User? _currentUser;

        public AuthenticationManager(ILoggerService logger,
            IMapper mapper,
            UserManager<User> user,
            IConfiguration configuration)
        {
            _logger = logger;
            _mapper = mapper;
            _user = user;
            _configuration = configuration;
        }

        public async Task<TokenDto> CreateToken(bool populatedExp)
        {
            var signinCredentials = GetSigninCredentials(); // kimlik bilgileri alındı
            var claims = await GetClaims(); // claining
            var tokenOptions = GenerateTokenOptions(signinCredentials, claims); // generate tokens informaiton

            var refreshToken = GenerateRefreshToken();
            _currentUser.RefreshToken = refreshToken;
            if (populatedExp)
            {
                _currentUser.RefreshTokenExpiryTime= DateTime.Now.AddDays(7);
            }
            await _user.UpdateAsync(_currentUser);

            var accessToken= new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return new TokenDto()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };
        }



        public async Task<IdentityResult> RegisterUser(UserForRegistrationDto userForRegistrationDto)
        {
            var user = _mapper.Map<User>(userForRegistrationDto);

            var result = await _user.CreateAsync(user, userForRegistrationDto.Password);

            
            if (result.Succeeded)
                await _user.AddToRolesAsync(user, userForRegistrationDto.Roles);
            return result;
        }

        public async Task<bool> ValidateUser(UserForAuthenticationDto userForAuthDto)
        {

            _currentUser = await _user.FindByNameAsync(userForAuthDto.UserName);

            var result = (_currentUser != null && await _user.CheckPasswordAsync(_currentUser, userForAuthDto.Password));
            if (!result)
            {
                _logger.LogWarning($"{nameof(ValidateUser)}: Authentication failed. Wrong username or password");
            }

            return result;
        }

        private SigningCredentials GetSigninCredentials()
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.UTF8.GetBytes(jwtSettings["secretKey"]);
            var secret = new SymmetricSecurityKey(key);

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);//selected algorithm
        }
        private async Task<List<Claim>> GetClaims()
        {
            var claims = new List<Claim>()
            {   
                new Claim(ClaimTypes.Name,_currentUser.UserName)
            }; //added username to claims

            var roles = await _user
                .GetRolesAsync(_currentUser);//list of string 

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            } //return to calims type
            return claims;
        }
        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signinCredentials,
            List<Claim> claims)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var tokenOptions = new JwtSecurityToken(
                issuer: jwtSettings["validIssuer"],
                audience: jwtSettings["validAudience"],
                claims: claims,
                expires: DateTime.Now.AddHours(Convert.ToDouble(jwtSettings["expires"])),
                signingCredentials: signinCredentials
                );
            return tokenOptions;
        }
   
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
                using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }


        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["secretKey"];
          
            var tokenValidationParam = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["validIssuer"],
                ValidAudience = jwtSettings["validAudience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))

            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;

            var principal = tokenHandler.ValidateToken(token,tokenValidationParam, out securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if(jwtSecurityToken is null ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid Token");
            }

            return principal;
        }

        public async Task<TokenDto> RefreshToken(TokenDto tokenDto)
        {
            var principal = GetPrincipalFromExpiredToken(tokenDto.AccessToken);
            var user = await _user.FindByNameAsync(principal.Identity.Name);

            if(user == null ||
                user.RefreshToken != tokenDto.RefreshToken ||
                user.RefreshTokenExpiryTime <= DateTime.Now)
                throw new RefreshTokenBadRequestException();

            _currentUser = user;
            return await CreateToken(populatedExp: false);
        }
    }
}
