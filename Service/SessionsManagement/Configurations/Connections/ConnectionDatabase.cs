using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using SessionsManagement.Configurations.Connections.Helper;
using SessionsManagement.Context;

namespace SessionsManagement.Configurations.Connections;

public static class ConnectionDatabase
{
    public static IServiceCollection AddConnectionDatabase(this IServiceCollection services, IConfiguration configuration)
    {
         //var connectionString = configuration.GetConnectionString("Connection");
        var connectionString = configuration.GetConnectionString("Container");

        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentNullException(nameof(connectionString));
            
        using (var serviceProvider = services.BuildServiceProvider())
        {
            var logger = serviceProvider.GetRequiredService<ILogger<object>>();

            WaitForIt.WaitForDatabaseAsync(connectionString, logger).GetAwaiter().GetResult();
        }

        services.AddDbContext<AppDbContext>(op =>
        {
            op.UseNpgsql(connectionString);
            if (configuration.GetValue<string>("ASPNETCORE_ENVIROMENT") == "Devolopment")
                op.EnableSensitiveDataLogging();
        });

        return services;
    }
}
