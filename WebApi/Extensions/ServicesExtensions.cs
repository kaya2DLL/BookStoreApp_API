using AspNetCoreRateLimit;
using Entities.DataTransferObject;
using Entities.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Presentation.ActionFilter;
using Presentation.ActionFilters;
using Presentation.Controllers;
using Repositories.Contracts;
using Repositories.EFCore;
using Services;
using Services.Contracts;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace WebApi.Extensions
{
    public static class ServicesExtensions
    {

        /// <summary>
        /// Connection to SQL with sql info in appsettings.json 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<RepositoryContext>
                (options => options.UseSqlServer
                (configuration.GetConnectionString
                ("sqlConnection")));
        }

        public static void ConfigureRepositoryManager(this IServiceCollection services)
        {
            services.AddScoped<IRepositoryManager, RepositoryManager>();
        }

        public static void ConfigureServiceManager(this IServiceCollection services)
        {
            services.AddScoped<IServiceManager, ServiceManager>();
        }
        /// <summary>
        /// Added response or error information with log
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureLoggerService(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerService, LoggerManager>();
        }


        /// <summary>
        /// Added Filters For API Response
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureActionfilters(this IServiceCollection services)
        {
            services.AddScoped<ValidationFilterAttribute>();// IoC
            services.AddSingleton<LogFilterAttribute>(); // logs attribute
            services.AddScoped<ValidateMediaTypeAttribute>(); // media type validation
        }

        /// <summary>
        /// Added response with CSV format 
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders("X-Pagination"));
            });
        }

        /// <summary>
        /// DataSharper
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureDataShaper(this IServiceCollection services)
        {
            services.AddScoped<IDataShaper<BookDto>, DataShaper<BookDto>>();
        }
        /// <summary>
        /// HATEoas Art.
        /// </summary>
        /// <param name="services"></param>
        public static void AddCustomMediaTypes(this IServiceCollection services)
        {
            services.Configure<MvcOptions>(config =>
            {
                var systemTextJsonOutputFormatter = config
                .OutputFormatters
                .OfType<SystemTextJsonOutputFormatter>()?.FirstOrDefault();

                if (systemTextJsonOutputFormatter != null)
                {
                    systemTextJsonOutputFormatter.SupportedMediaTypes
                    .Add("application/vnd.bookapi.hateoas+json");

                    systemTextJsonOutputFormatter.SupportedMediaTypes
                    .Add("application/vnd.bookapi.apiroot+json");
                }

                var xmlOutputFormatter = config
                .OutputFormatters
                .OfType<XmlDataContractSerializerOutputFormatter>()?.FirstOrDefault();

                if (xmlOutputFormatter is not null)
                {
                    xmlOutputFormatter.SupportedMediaTypes
                    .Add("application/vnd.bookapi.hateoas+xml");

                    xmlOutputFormatter.SupportedMediaTypes
                    .Add("application/vnd.bookapi.apiroot+xml");
                }
            });
        }

        /// <summary>
        /// Added API versioning
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(opt =>
            {
                opt.ReportApiVersions = true;
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.DefaultApiVersion = new ApiVersion(1, 0);
                opt.ApiVersionReader = new HeaderApiVersionReader("api-version");
                opt.Conventions.Controller<BooksController>()
                .HasApiVersion(new ApiVersion(1, 0));// Describing API version

                opt.Conventions.Controller<BooksV2Controller>()
                .HasDeprecatedApiVersion(new ApiVersion(2, 0));
            });
        }
        /// <summary>
        /// Caching of response information
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureResponseCaching(this IServiceCollection services) =>
            services.AddResponseCaching();
        /// <summary>
        /// Caching of headers information
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigurehttpCacheHeaders(this IServiceCollection services) =>
            services.AddHttpCacheHeaders(expirationOPt =>
            {
                expirationOPt.MaxAge = 90;
                expirationOPt.CacheLocation = Marvin.Cache.Headers.CacheLocation.Public;
            },
                validationOpt =>
                {
                    validationOpt.MustRevalidate = false;
                });
        /// <summary>
        /// Rate Limit for Response
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureRateLimit(this IServiceCollection services)
        {
            var rateLimitRules = new List<RateLimitRule>()
            {
                new RateLimitRule()
                {
                    Endpoint="*",
                    Limit=50,
                    Period="1m"
                }
            };

            services.Configure<IpRateLimitOptions>(opt =>
            {
                opt.GeneralRules = rateLimitRules;
            });

            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();//
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureIdentity(this IServiceCollection services)
        {
            var builder = services.AddIdentity<User, IdentityRole>(opt =>
            {
                opt.Password.RequireDigit = true;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequiredLength = 8;
                opt.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<RepositoryContext>()
                .AddDefaultTokenProviders(); 

        }



        /// <summary>
        /// Json web token Configuration
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void ConfigureJWT(this IServiceCollection services,
            IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["secretKey"];

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options=>
                options.TokenValidationParameters= new TokenValidationParameters
                {
                    ValidateIssuer= true,
                    ValidateAudience= true,
                    ValidateLifetime= true,
                    ValidateIssuerSigningKey= true,
                    ValidIssuer = jwtSettings["validIssuer"],
                    ValidAudience = jwtSettings["validAudience"],
                    IssuerSigningKey= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                }
            );
        }
    }


}
