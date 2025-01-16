using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Orders.Infrastructure.CacheStorage
{
    public class RedisCacheService(IDistributedCache _cache) : ICacheService
    {
        public async Task<T?> GetAsync<T>(string key)
        {
            var value = await _cache.GetStringAsync(key);
            return string.IsNullOrEmpty(value) ? default : JsonSerializer.Deserialize<T>(value);
        }
        public async Task SetAsync<T>(string key, T value, TimeSpan? timeToLive = null)
        {
            timeToLive = timeToLive.HasValue ? timeToLive.Value : TimeSpan.FromMinutes(10);

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = timeToLive,
                SlidingExpiration = TimeSpan.FromMinutes(5)
            };

            await _cache.SetStringAsync(key, JsonSerializer.Serialize(value), options);
        }
    }
}
