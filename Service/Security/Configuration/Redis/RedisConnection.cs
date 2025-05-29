using System;
using StackExchange.Redis;

namespace Security.Configuration.Redis;

public static class RedisConnection
{
    public static IServiceCollection AddRedisConnection(this IServiceCollection service)
    {
        service.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            
            // var redisConnect = configuration.GetConnectionString("RedisLocal");
            var redisConnect = configuration.GetConnectionString("RedisCont");

            if (string.IsNullOrEmpty(redisConnect))
                throw new ArgumentNullException(nameof(redisConnect) + " redis connect could not must be null");

            return ConnectionMultiplexer.Connect(redisConnect);
        });
        return service;
    }
}
