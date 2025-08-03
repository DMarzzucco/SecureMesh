using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;
using System.Text;
using Auth.Utils.Filter;
using Auth.Cookies.Interfaces;
using Auth.Cookies;
using Auth.JWT.Interfaces;
using Auth.JWT;
using Auth.Module.Services.Interfaces;
using Auth.Module.Services;
using Auth.Module.Filter;
using Auth.Configuration;
using Auth.Queues;
using Auth.Queues.Interfaces;
using Auth.Queues.Messaging.Interfaces;
using Auth.Queues.Messaging;
using Auth.Configuration.Redis;
using Auth.Configuration.Redis.Repository.Interfaces;
using Auth.Configuration.Redis.Repository;
using Auth.Utils.Helper;
using Auth.Server.Users.Helper;
using Auth.Server.Users.Maps;
using Auth.Server.Users.Service;
using Auth.Server.Users.Service.Interfaces;
using Auth.Server.Security.Service.Interfaces;
using Auth.Server.Security.Service;
using Auth.Configuration.PostgreSQL;
using Auth.Module.Repository.Interface;
using Auth.Module.Repository;
using Auth._2FA.Interfaces;
using Auth._2FA;
using Auth.Server.Hangfire.Interfaces;
using Auth.Server.Hangfire;
using System.Reflection;
using SwaggerSchemaExample.Nuget;

namespace Auth.Extensions
{
    /// <summary>
    /// Service builder
    /// </summary>
    public static class ServiceBuilderExtensions
    {
        public static IServiceCollection AddServiceBuilderExtensions(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddHttpContextAccessor();

            //JWT Configuration
            var secretKey = configuration.GetSection("JwtSettings").GetSection("seecretKey").ToString();
            if (string.IsNullOrEmpty(secretKey))
                throw new ArgumentNullException(nameof(secretKey), "Secret Key cannot be null or empty");

            service.AddPostgreSQLConnectionDatabase(configuration);
            service.AddRedisConnection();    
            service.AddAuthentication(conf =>
            {
                conf.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                conf.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(conf =>
            {
                var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
                var signingCredential = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature);

                conf.RequireHttpsMetadata = false;
                conf.SaveToken = true;
                conf.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            //Controller
            service.AddControllers(o =>
            {
                o.Filters.Add(typeof(GlobalFilterExceptions));
            }).AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.PropertyNamingPolicy = null;
                o.JsonSerializerOptions.WriteIndented = true;
                o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });
            //scope
            service.AddScoped<GlobalFilterExceptions>();
            //redis
            service.AddScoped<IRedisRepository, RedisRepository>();
            //rabbitMQ
            service.AddScoped<IRabbitMQServices, RabbitMQServices>();
            service.AddScoped<IMessagingQueues, MessagingQueues>();
            //code generation
            service.AddScoped<CodeGeneration>();
            
            service.AddScoped<RequestMapperUserGrpc>();
            service.AddScoped<ISecurityService, SecurityService>();
            service.AddScoped<IHangFireService, HangFireService>();
            service.AddScoped<IUserService, UserService>();
            service.AddScoped<HandleGrpcError>();
            service.AddScoped<ICookieService, CookieService>();
            service.AddScoped<IJwtService, JwtService>();
            service.AddScoped<IValidateTwoFactorAuth, ValidateTwoFactorAuth>();
            service.AddScoped<IAuthRepository, AuthRepository>();
            service.AddScoped<IAuthService, AuthService>();
            service.AddScoped<LocalAuthFilter>();

            //Swagger Configuration
            service.AddEndpointsApiExplorer();
            service.AddSwaggerGen(o =>
            {
                o.EnableAnnotations();
                o.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "v1",
                    Title = "Security API",
                    Description = " Api of Security"
                });
                o.SchemaFilter<SwaggerSchemaExampleFilter>();

            });
            //Cors Policy
            service.AddCors(o =>
            {
                o.AddPolicy("CorsPolicy", c =>
                {
                    c.WithOrigins("http://localhost:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
                });
            });
            //gRPC
            service.AddGrpcService();

            return service;
        }
    }
}
