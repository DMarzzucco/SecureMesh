using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;
using System.Text;
using IdentifyService.Configuration.Swagger;
using IdentifyService.Utils.Filter;
using IdentifyService.Cookies.Interfaces;
using IdentifyService.Cookies;
using IdentifyService.JWT.Interfaces;
using IdentifyService.JWT;
using IdentifyService.Module.Services.Interfaces;
using IdentifyService.Module.Services;
using IdentifyService.Module.Filter;
using IdentifyService.Configuration;
using IdentifyService.Queues;
using IdentifyService.Queues.Interfaces;
using IdentifyService.Queues.Messaging.Interfaces;
using IdentifyService.Queues.Messaging;
using IdentifyService.Configuration.Redis;
using IdentifyService.Configuration.Redis.Repository.Interfaces;
using IdentifyService.Configuration.Redis.Repository;
using IdentifyService.Utils.Helper;
using IdentifyService.Server.UMS.Helper;
using IdentifyService.Server.UMS.Maps;
using IdentifyService.Server.UMS.Services;
using IdentifyService.Server.UMS.Services.Interfaces;
using IdentifyService.Configuration.PostgreSQL;
using IdentifyService.Module.Repository.Interface;
using IdentifyService.Module.Repository;
using IdentifyService._2FA.Interfaces;
using IdentifyService._2FA;
using IdentifyService.JWT.Helper.Interfaces;
using IdentifyService.JWT.Helper;

namespace IdentifyService.Extensions
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
            service.AddScoped<IManagementUserFacedeServices, ManagementUserFacedeServices>();
            service.AddScoped<HandleGrpcError>();
            service.AddScoped<ICookieService, CookieService>();
            service.AddScoped<ITokenCreationServices, TokenCreationServices>();
            service.AddScoped<IJwtService, JwtService>();
            service.AddScoped<IValidateTwoFactorAuth, ValidateTwoFactorAuth>();
            service.AddScoped<IIdentityProviderRepository, IdentityProviderRepository>();
            service.AddScoped<IIdentityProviderService, IdentityProviderService>();
            service.AddScoped<LocalAuthFilter>();

            //Swagger Configuration
            service.AddEndpointsApiExplorer();
            service.AddSwaggerGen(o =>
            {
                o.EnableAnnotations();
                o.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "v1",
                    Title = "Identity Service",
                    Description = " Identity Provider"
                });
                o.SchemaFilter<SwaggerSchemaFilter>();

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
