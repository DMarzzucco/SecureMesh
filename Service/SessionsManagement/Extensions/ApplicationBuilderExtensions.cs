using System;
using SessionsManagement.Configurations.Connections;
using SessionsManagement.Module.Repository;
using SessionsManagement.Module.Repository.Interfaces;
using SessionsManagement.Services;

namespace SessionsManagement.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseApplicationBuilderExtensions(this IApplicationBuilder app)
    {
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseEndpoints(e =>
        {
            e.MapGrpcService<ManagementSessionsServices>();
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

        services.AddScoped<IManagementSessionsRepository, ManagementSessionsRepository>();

        return services;
    }
}
