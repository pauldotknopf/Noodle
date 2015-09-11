using System.Web;
using Noodle.Caching;
using Noodle.Extensions.Caching;

namespace Noodle.Web.Caching
{
    /// <summary>
    /// This implementation if ICacheManager internally uses http cache if asp.net, otherwise, it uses in memory cache
    /// </summary>
    public class AdaptiveCache : ICacheManager
    {
        private readonly ICacheManager _inMemoryCache;
        private readonly ICacheManager _httpRuntimeCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdaptiveCache"/> class.
        /// </summary>
        public AdaptiveCache()
        {
            _inMemoryCache = new InMemoryCache();
            _httpRuntimeCache = new HttpRuntimeCache();
        }

        /// <summary>
        /// Gets the cache manager.
        /// </summary>
        /// <value>
        /// The cache manager.
        /// </value>
        public ICacheManager CacheManager
        {
            get
            {
                return HttpContext.Current != null ? _httpRuntimeCache : _inMemoryCache;
            }
        }

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>
        /// The value associated with the specified key.
        /// </returns>
        public T Get<T>(string key)
        {
            return CacheManager.Get<T>(key);
        }

        /// <summary>
        /// Adds the specified key and object to the cache.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="data">Data</param>
        /// <param name="cacheTime">Cache time</param>
        public void Set(string key, object data, int cacheTime)
        {
            CacheManager.Set(key, data, cacheTime);
        }

        /// <summary>
        /// Gets a value indicating whether the value associated with the specified key is cached
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>
        /// Result
        /// </returns>
        public bool IsSet(string key)
        {
            return CacheManager.IsSet(key);
        }

        /// <summary>
        /// Removes the value with the specified key from the cache
        /// </summary>
        /// <param name="key">/key</param>
        public void Remove(string key)
        {
            CacheManager.Remove(key);
        }

        /// <summary>
        /// Removes items by pattern
        /// </summary>
        /// <param name="pattern">pattern</param>
        public void RemoveByPattern(string pattern)
        {
            CacheManager.RemoveByPattern(pattern);
        }

        /// <summary>
        /// Clear all cache data
        /// </summary>
        public void Clear()
        {
            CacheManager.Clear();
        }
    }
}
