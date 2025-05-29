using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;
using System.Text;
using Security.Configuration.Swagger;
using Security.Utils.Filter;
using Security.Server.Service.Interfaces;
using Security.Server.Service;
using Security.Cookies.Interfaces;
using Security.Cookies;
using Security.JWT.Interfaces;
using Security.JWT;
using Security.Module.Services.Interfaces;
using Security.Module.Services;
using Security.Module.Filter;
using Security.Configuration;
using Security.Server.Helper;
using Security.Queues;
using Security.Queues.Interfaces;
using Security.Queues.Messaging.Interfaces;
using Security.Queues.Messaging;
using Security.Server.Maps;
using Security.Configuration.Redis;
using Security.Configuration.Redis.Repository.Interfaces;
using Security.Configuration.Redis.Repository;

namespace Security.Extensions
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

            service.AddScoped<RequestMapperUserGrpc>();
            service.AddScoped<IUserService, UserService>();
            service.AddScoped<HandleGrpcError>();
            service.AddScoped<ICookieService, CookieService>();
            service.AddScoped<IJwtService, JwtService>();
            service.AddScoped<ISecurityService, SecurityService>();
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
                o.SchemaFilter<SwaggerSchemaFilter>();
            });
            //Cors Policy
            service.AddCors(o =>
            {
                o.AddPolicy("CorsPolicy", c =>
                {
                    c.WithOrigins("https:localhost:8888/")
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
