using System.Text;
using System.Text.Json.Serialization;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SwaggerSchemaExample.Nuget;
using UserManagementService.Configuration.gRPC;
using UserManagementService.Configuration.PostgreSQL;
using UserManagementService.Configuration.Redis;
using UserManagementService.Configuration.Redis.Repository;
using UserManagementService.Configuration.Redis.Repository.Interfaces;
using UserManagementService.JWT.Services;
using UserManagementService.JWT.Services.Interfaces;
using UserManagementService.Mapper;
using UserManagementService.Modules.Repository;
using UserManagementService.Modules.Repository.Interfaces;
using UserManagementService.Modules.Services;
using UserManagementService.Modules.Services.Interfaces;
using UserManagementService.Modules.Stub;
using UserManagementService.Modules.Stub.Helper;
using UserManagementService.Queues.Messaging;
using UserManagementService.Queues.Messaging.Interfaces;
using UserManagementService.Queues.Services;
using UserManagementService.Queues.Services.Interfaces;
using UserManagementService.Server.Hangfire.Services;
using UserManagementService.Server.Hangfire.Services.Interfaces;
using UserManagementService.Server.Idp.Services;
using UserManagementService.Server.Idp.Services.Interfaces;
using UserManagementService.Server.Sessions.Services;
using UserManagementService.Server.Sessions.Services.Interfaces;
using UserManagementService.Server.Users.Helper;
using UserManagementService.Server.Users.Maps;
using UserManagementService.Server.Users.Service;
using UserManagementService.Server.Users.Service.Interfaces;
using UserManagementService.Utils.Filter;

namespace UserManagementService.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseApplicationBuilderExtensions(this IApplicationBuilder app)
    {
        app.UseAuthorization();
        app.UseAuthentication();
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseEndpoints(e =>
        {
            e.MapGrpcService<IdpFacedeServiceImpl>();
            e.MapGrpcService<ScheduledDeletionAccount>();
        });
        app.UseStaticFiles();

        return app;
    }
}

public static class ServiceBuilderExtensions
{
    public static IServiceCollection AddServicesBuilderExtensions(this IServiceCollection services, IConfiguration configuration)
    {
        var secretKey = configuration.GetSection("JwtSettings").GetSection("seecretKey").ToString();
        if (string.IsNullOrEmpty(secretKey))
            throw new ArgumentNullException(nameof(secretKey), "Secret Key cannot be null or empty");

        services.AddPostgreSQLConnection(configuration);
        /// gRPC
        services.AddGrpcService();
        /// redis
        services.AddRedisConnection();
        /// rabbitmq
        services.AddAuthentication(conf =>
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

        services.AddControllers(static op =>
        {
            op.Filters.Add(typeof(GlobalFilterExceptions));
        }).AddJsonOptions(o =>
        {
            o.JsonSerializerOptions.PropertyNamingPolicy = null;
            o.JsonSerializerOptions.WriteIndented = true;
            o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });
        //scope service
        services.AddScoped<GlobalFilterExceptions>();
        services.AddScoped<HandleGrpcError>();
        services.AddScoped<MapModelsGrpc>();
        services.AddScoped<MapGrpcExceptions>();
        services.AddScoped<RequestMapperUserGrpc>();
        services.AddScoped<IRedisRepository, RedisRepository>();
        services.AddScoped<IRabbitMQServices, RabbitMQServices>();
        services.AddScoped<IMessagingQueues, MessagingQueues>();
        services.AddScoped<IJwtServices, JwtServices>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ISessionManagementServices, SessionManagementServices>();
        services.AddScoped<IIdpServices, IdpServices>();
        services.AddScoped<IHangFireService, AccountDeletionService>();
        services.AddScoped<IManagementUserServices, ManagementUserServices>();
        services.AddScoped<IManagementUserRepository, ManagementUserRepository>();
        //swagger
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(op =>
        {
            op.EnableAnnotations();
            op.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "User Management Service",
                Version = "0.1",
                Description = "UMS API"
            });
            op.SchemaFilter<SwaggerSchemaExampleFilter>();
        });

        //mapper
        var mappConfig = new MapperConfiguration(conf =>
        {
            conf.AddProfile<MapperProfile>();
        });
        IMapper mapper = mappConfig.CreateMapper();
        services.AddSingleton(mapper);
        services.AddMvc();

        return services;
    }
}
