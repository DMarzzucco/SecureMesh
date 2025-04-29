using Microsoft.AspNetCore.Authorization;
using SecureMesh.Authorization;
using SecureMesh.Configuration;
using SecureMesh.Configuration.Swagger;
using SecureMesh.ReverseProxy;
using SecureMesh.Utils.Filter;

namespace SecureMesh.Extensions;

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
                c.WithOrigins("https://localhost:8878")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
        return service;
    }
}
