namespace Orders.Infrastructure.CacheStorage
{
    public interface ICacheService
    {
        Task<T> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? timeToLive = null);
    }
}
