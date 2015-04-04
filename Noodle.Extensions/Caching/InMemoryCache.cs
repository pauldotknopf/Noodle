using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Noodle.FluentDateTime;

namespace Noodle.Caching
{
    /// <summary>
    /// This implemention of ICacheManager stores items in memory.
    /// </summary>
    public class InMemoryCache : ICacheManager
    {
        private readonly ConcurrentDictionary<string, object> _cache = new ConcurrentDictionary<string, object>(); 
        private readonly object _lock = new object();

        public T Get<T>(string key)
        {
            lock (_lock)
            {
                if (!IsSet(key))
                    return default(T);
                var entry = _cache[key];
                if (entry == null)
                    // why is this null? should be CacheEntry with null Data
                    return default(T);
                if (!(entry is CacheEntry))
                    // why is this not a cache entry?
                    return default(T);
                var cacheEntry = entry as CacheEntry;
                if (IsEntryExpired(key, cacheEntry))
                    return default(T);
                return (T)cacheEntry.Data;
            }
        }

        public void Set(string key, object data, int cacheTime)
        {
            lock (_lock)
            {
                var entry = new CacheEntry();
                entry.Data = data;
                if (cacheTime > 0)
                    entry.Expires = CommonHelper.CurrentTime().Add(cacheTime.Seconds());
                _cache[key] = entry;
            }
        }

        public bool IsSet(string key)
        {
            if (!_cache.ContainsKey(key))
                return false;

            // there is an entry, check to make sure it shouldn't be invalidated
            lock (_lock)
            {
                return !IsEntryExpired(key, _cache[key] as CacheEntry);
            }
        }

        public void Remove(string key)
        {
            if (IsSet(key))
            {
                lock (_lock)
                {
                    if (IsSet(key))
                    {
                        object value;
                        _cache.TryRemove(key, out value);
                    }
                }
            }
        }

        public void RemoveByPattern(string pattern)
        {
            var regex = new Regex(pattern.ToLower(), RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            foreach (var key in (from item in _cache where regex.IsMatch(item.Key) select item.Key).ToList())
            {
                Remove(key);
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                _cache.Clear();
            }
        }

        /// <summary>
        /// Returns true if the entry is expired.
        /// If expired, it auto remotes itself from the cache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        private bool IsEntryExpired(string key, CacheEntry entry)
        {
            if (entry == null)
            {
                // no entry? remote it from queu then
                object value;
                _cache.TryRemove(key, out value);
                return true;
            }
            if (entry.Expires != null)
            {
                if (entry.Expires <= CommonHelper.CurrentTime())
                {
                    // this item is expired, remote it and return null
                    object value;
                    _cache.TryRemove(key, out value);
                    return true;
                }
            }
            return false;
        }

        private class CacheEntry
        {
            /// <summary>
            /// The data being cached
            /// </summary>
            public object Data { get; set; }

            /// <summary>
            /// The time that this object expires. NULL if item never expires.
            /// </summary>
            public DateTime? Expires { get; set; }
        }
    }
}
