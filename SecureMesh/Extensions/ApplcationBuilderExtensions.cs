using Microsoft.AspNetCore.Authorization;
using SecureMesh.Authorization;
using SecureMesh.Utils.Filter;
using SecureMesh.ReverseProxy;
using SecureMesh.Configuration;

namespace SecureMesh.Extensions
{
    /// <summary>
    /// Application builder Extensions
    /// </summary>
    public static class ApplcationBuilderExtensions
    {
        public static IApplicationBuilder UseApplicationBuilderExtensions(this IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRateLimiter();

            return app;
        }
    }

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
            service.AddEndpointsApiExplorer();
            service.AddSwaggerGen();
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
}