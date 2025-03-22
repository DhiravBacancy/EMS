using Microsoft.Extensions.Caching.Memory;

namespace EMS.Service
{
    public interface ICacheService
    {
        void Set(string key, object value, TimeSpan expiration);
        object Get(string key);
        bool Contains(string key);
        void Remove(string key);
    }
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        public CacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void Set(string key, object value, TimeSpan expiration)
        {
            _memoryCache.Set(key, value, expiration);
        }

        public object Get(string key)
        {
            _memoryCache.TryGetValue(key, out var value);
            return value;
        }

        public bool Contains(string key)
        {
            return _memoryCache.TryGetValue(key, out _);
        }

        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }
    }
}
