using System;
using Hangfire;
using Hangfire.PostgreSql;

namespace HangfireUserServer.Configurations;

public static class HangfireService
{
    public static IServiceCollection AddHangfireServices(this IServiceCollection service, IConfiguration configuration)
    {
        // var connectionHangfireString = configuration.GetConnectionString("HangfireConnection");
        var connectionHangfireString = configuration.GetConnectionString("HangfireContainer");

        if (string.IsNullOrEmpty(connectionHangfireString))
            throw new ArgumentNullException(nameof(connectionHangfireString), "Connection String cannot be null or empty");

        //apply wait for it     

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
