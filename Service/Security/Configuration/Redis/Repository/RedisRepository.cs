using Security.Configuration.Redis.Repository.Interfaces;
using StackExchange.Redis;

namespace Security.Configuration.Redis.Repository;

public class RedisRepository(IConnectionMultiplexer redis): IRedisRepository
{
    private readonly IDatabase _db = redis.GetDatabase();

    /// <summary>
    /// Get Model by Token
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<string?> GetByTokenAsync(string key)
    {
        var value = await this._db.StringGetAsync(key);
        if (value == "used")
            throw new UnauthorizedAccessException("This Token was used");
        return key;
    }
    /// <summary>
    /// Save Token in db 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task SetAsync(string key)
    {
        await this._db.StringSetAsync(key, "unused", TimeSpan.FromMinutes(11));
    }
    /// <summary>
    /// Update Status
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<bool> UpdateStateAsync(string key)
    {
        var value = await this._db.StringGetAsync(key);
        if (value.IsNullOrEmpty || value == "used")
            return false;

        return await this._db.StringSetAsync(key, "used");
    }
}