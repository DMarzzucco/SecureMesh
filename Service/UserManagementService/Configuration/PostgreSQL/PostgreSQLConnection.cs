using System;
using Microsoft.EntityFrameworkCore;
using UserManagementService.Configuration.PostgreSQL.Helper;
using UserManagementService.Context;

namespace UserManagementService.Configuration.PostgreSQL;

public static class PostgreSQLConnection
{
    public static IServiceCollection AddPostgreSQLConnection(this IServiceCollection service, IConfiguration configuration)
    {
        // var connectionString = configuration.GetConnectionString("Connection");
        var connectionString = configuration.GetConnectionString("Container");

        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentNullException(nameof(connectionString), "Connection String cannot be null or empty");

        using (var serviceProvider = service.BuildServiceProvider())
        {
            var logger = serviceProvider.GetRequiredService<ILogger<object>>();

            WaitForIt.WaitForDatabaseAsync(connectionString, logger).GetAwaiter().GetResult();
        }

        service.AddDbContext<AppDbContext>(op =>
        {
            op.UseNpgsql(connectionString);
            if (configuration.GetValue<string>("APNETCORE_ENVIROMENT") == "Development")
                op.EnableSensitiveDataLogging();
        });

        return service;
    }
}
