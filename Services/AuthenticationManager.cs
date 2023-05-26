using AutoMapper;
using Entities.DataTransferObject;
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

        public async Task<string> CreateToken()
        {
            var signinCredentials = GetSigninCredentials(); // kimlik bilgileri alındı
            var claims = await GetClaims(); // claining
            var tokenOptions = GenerateTokenOptions(signinCredentials, claims); // generate tokens informaiton
            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
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
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["expires"])),
                signingCredentials: signinCredentials
                );
            return tokenOptions;
        }


    }
}
