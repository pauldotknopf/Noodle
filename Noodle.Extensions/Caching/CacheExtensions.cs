using System;

namespace Noodle.Extensions.Caching
{
    /// <summary>
    /// Extensions to the cache managers
    /// </summary>
    public static class CacheExtensions
    {
        /// <summary>
        /// Gets a value from the cache.
        /// Invokes delegate to get value if no value found.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheManager"></param>
        /// <param name="key"></param>
        /// <param name="acquire"></param>
        /// <returns></returns>
        public static T Get<T>(this ICacheManager cacheManager, string key, Func<T> acquire)
        {
            return Get(cacheManager, key, 60, acquire);
        }

        /// <summary>
        /// Gets a value from the cache.
        /// Invokes delegate to get value if no value found
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheManager"></param>
        /// <param name="key"></param>
        /// <param name="cacheTime"></param>
        /// <param name="acquire"></param>
        /// <returns></returns>
        public static T Get<T>(this ICacheManager cacheManager, string key, int cacheTime, Func<T> acquire)
        {
            if (cacheManager.IsSet(key))
            {
                return cacheManager.Get<T>(key);
            }

            var result = acquire();
            cacheManager.Set(key, result, cacheTime);
            return result;
        }
    }
}
