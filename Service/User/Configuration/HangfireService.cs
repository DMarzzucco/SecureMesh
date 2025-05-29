using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using User.Configuration.DbConfiguration.Helper;

namespace User.Configuration;

public static class HangfireService
{
    public static IServiceCollection AddHangfireServices(this IServiceCollection service, IConfiguration configuration)
    {
        // var connectionHangfireString = configuration.GetConnectionString("HangfireConnection");
        var connectionHangfireString = configuration.GetConnectionString("HangfireContainer");

        if (string.IsNullOrEmpty(connectionHangfireString))
            throw new ArgumentNullException(nameof(connectionHangfireString), "Connection String cannot be null or empty");

        using (var serviceProvider = service.BuildServiceProvider())
        {
            var logger = serviceProvider.GetRequiredService<ILogger<object>>();

            WaitForIt.WaitForDatabaseAsync(connectionHangfireString, logger).GetAwaiter().GetResult();
        }
        
        service.AddHangfire(conf => conf
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(connectionHangfireString)
           );
        service.AddHangfireServer();

        return service;
    }
}