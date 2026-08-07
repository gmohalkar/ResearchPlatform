using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

public class RedisCacheService
    : ICacheService
{
    private readonly IDistributedCache
        _cache;

    public RedisCacheService(
        IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetAsync<T>(
        string key)
    {
        var value =
            await _cache.GetStringAsync(key);

        if (value == null)
            return default;

        return JsonSerializer.Deserialize<T>(
            value);
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan expiry)
    {
        await _cache.SetStringAsync(
            key,
            JsonSerializer.Serialize(value),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow =
                    expiry
            });
    }

    public async Task RemoveAsync(
        string key)
    {
        await _cache.RemoveAsync(key);
    }
}