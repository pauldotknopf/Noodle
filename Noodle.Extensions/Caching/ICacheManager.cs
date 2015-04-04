namespace Noodle.Caching
{
    /// <summary>
    /// Cache manager interface
    /// </summary>
    public interface ICacheManager
    {
        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <typeparam name="T">The type of object stored in the cache.</typeparam>
        /// <param name="key">The key to look for.</param>
        /// <returns>The value associated with the specified key.</returns>
        T Get<T>(string key);

        /// <summary>
        /// Adds the specified key and object to the cache.
        /// </summary>
        /// <param name="key">The key of the object used for later retrieval</param>
        /// <param name="data">The object to store in the cache</param>
        /// <param name="cacheTime">The time (in seconds) to cache the item. Negative value indicates caching forever</param>
        void Set(string key, object data, int cacheTime = -1);

        /// <summary>
        /// Indicates whether the value associated with the specified key is cached
        /// </summary>
        /// <param name="key">The key to look in the cache for</param>
        /// <returns>Returns true if the item is cached, otherwise false</returns>
        bool IsSet(string key);

        /// <summary>
        /// Removes the value with the specified key from the cache
        /// </summary>
        /// <param name="key">/key</param>
        void Remove(string key);

        /// <summary>
        /// Removes items by pattern (regex).
        /// </summary>
        /// <param name="pattern">pattern</param>
        void RemoveByPattern(string pattern);

        /// <summary>
        /// Clear all cache data
        /// </summary>
        void Clear();
    }
}
