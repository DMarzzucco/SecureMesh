using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;
using System.Text;
using Auth.Configuration.Swagger;
using Auth.Utils.Filter;
using Auth.Server.Service.Interfaces;
using Auth.Server.Service;
using Auth.Cookies.Interfaces;
using Auth.Cookies;
using Auth.JWT.Interfaces;
using Auth.JWT;
using Auth.Module.Services.Interfaces;
using Auth.Module.Services;
using Auth.Module.Filter;
using Auth.Configuration;
using Auth.Server.Helper;
using Auth.Queues;
using Auth.Queues.Interfaces;
using Auth.Queues.Messaging.Interfaces;
using Auth.Queues.Messaging;
using Auth.Server.Maps;
using Auth.Configuration.Redis;
using Auth.Configuration.Redis.Repository.Interfaces;
using Auth.Configuration.Redis.Repository;
using Auth.Utils.Helper;

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
            service.AddScoped<IUserService, UserService>();
            service.AddScoped<HandleGrpcError>();
            service.AddScoped<ICookieService, CookieService>();
            service.AddScoped<IJwtService, JwtService>();
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
