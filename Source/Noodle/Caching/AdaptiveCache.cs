using System.Web;

namespace Noodle.Caching
{
    public class AdaptiveCache : ICacheManager
    {
        private ICacheManager _inMemoryCache;
        private ICacheManager _httpRuntimeCache;

        public AdaptiveCache()
        {
            _inMemoryCache = new InMemoryCache();
            _httpRuntimeCache = new HttpRuntimeCache();
        }

        public ICacheManager CacheManager
        {
            get {
                if (HttpContext.Current != null)
                    return _httpRuntimeCache;
                else
                    return _inMemoryCache;
            }
        }

        public T Get<T>(string key)
        {
            return CacheManager.Get<T>(key);
        }

        public void Set(string key, object data, int cacheTime)
        {
            CacheManager.Set(key, data, cacheTime);
        }

        public bool IsSet(string key)
        {
            return CacheManager.IsSet(key);
        }

        public void Remove(string key)
        {
            CacheManager.Remove(key);
        }

        public void RemoveByPattern(string pattern)
        {
            CacheManager.RemoveByPattern(pattern);
        }

        public void Clear()
        {
            CacheManager.Clear();
        }
    }
}
