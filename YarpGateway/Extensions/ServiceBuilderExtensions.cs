using Microsoft.AspNetCore.Authorization;
using YarpGateway.Auth.Jwt;
using YarpGateway.Auth.RolesHierarchy.Handler;
using YarpGateway.Configuration.Swagger;
using YarpGateway.ReverseProxy;
using YarpGateway.Utils.Filter;

namespace YarpGateway.Extensions;

/// <summary>
/// Service builder extensions
/// </summary>
public static class ServiceBuilderExtensions
{
    public static IServiceCollection AddServiceBuilderExtensions(this IServiceCollection service,
        IConfiguration conf)
    {
        //jwt Config
        service.AddJwtBearerConfiguration(conf);
        //Yarp
        service.AddReverseProxyConfig(conf);
        //controller
        service.AddControllers(o => { o.Filters.Add(typeof(GlobalFilterExceptions)); });
        ///service add scope
        service.AddScoped<GlobalFilterExceptions>();
        service.AddSingleton<IAuthorizationHandler, MinimumRolesHandler>();
        //Swagger Configuration
        service.AddSwaggerConfigurationService();
        //Cors Policy
        service.AddCors(x =>
        {
            x.AddPolicy("CorsPolicy", c =>
            {
                c.WithOrigins("http://localhost:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
        return service;
    }
}
