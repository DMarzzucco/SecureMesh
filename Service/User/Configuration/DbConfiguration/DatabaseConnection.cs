using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore;
using User.Configuration.DbConfiguration.Helper;
using User.Context;

namespace User.Configuration.DbConfiguration;

public static class DatabaseConnection
{
    public static IServiceCollection AddDatabaseConnection(this IServiceCollection service,
        IConfiguration configuration)
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
