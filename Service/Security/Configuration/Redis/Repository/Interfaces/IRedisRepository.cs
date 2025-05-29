
namespace Security.Configuration.Redis.Repository.Interfaces;

public interface IRedisRepository
{
    Task<string?> GetByTokenAsync(string key);
    Task SetAsync(string key);
    Task<bool> UpdateStateAsync(string key);
}
