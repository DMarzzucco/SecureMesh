using System;
using Security.Configurations.Connections;
using Security.Module.Repository;
using Security.Module.Repository.Interfaces;
using Security.Services;

namespace Security.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseApplicationBuilderExtensions(this IApplicationBuilder app)
    {
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseEndpoints(e =>
        {
            e.MapGrpcService<SecurityService>();
        });
        app.UseStaticFiles();

        return app;
    }
}

public static class ServicesBuilderExtensions
{
    public static IServiceCollection AddServicesBuilderExtensions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddConnectionDatabase(configuration);
        services.AddGrpc();

        services.AddScoped<ISecurityRepository, SecurityRepository>();

        return services;
    }
}
