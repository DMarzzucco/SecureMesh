using System;
using Auth.Configuration.PostgreSQL.Helper;
using Auth.Context;
using Microsoft.EntityFrameworkCore;

namespace Auth.Configuration.PostgreSQL;

public static class PostgreSQLConnection
{
    public static IServiceCollection AddPostgreSQLConnectionDatabase(this IServiceCollection service, IConfiguration configuration)
    {
        // var connectionString = configuration.GetConnectionString("Connection");
        var connectionString = configuration.GetConnectionString("Container");

        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentNullException(nameof(connectionString));

        using (var serviceProvider = service.BuildServiceProvider())
        {
            var logger = serviceProvider.GetRequiredService<ILogger<object>>();

            WaitForIt.WaitForDatabaseAsync(connectionString, logger).GetAwaiter().GetResult();
        }

        service.AddDbContext<AppDbContext>(op =>
        {
            op.UseNpgsql(connectionString);
            if (configuration.GetValue<string>("ASPNETCORE_ENVIROMENT") == "Devolopment")
                op.EnableSensitiveDataLogging();
        });

        return service;
    }
}
