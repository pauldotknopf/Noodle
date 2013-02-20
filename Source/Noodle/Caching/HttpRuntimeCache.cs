using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;

namespace Noodle.Caching
{
    /// <summary>
    /// This implementation of cache manager stores data in the httpruntime (not per request).
    /// This should be used with ASP.NET applications
    /// </summary>
    public class HttpRuntimeCache : ICacheManager
    {
        /// <summary>
        /// Gets the cache.
        /// </summary>
        /// <value>
        /// The cache.
        /// </value>
        public virtual Cache Cache
        {
            get { return HttpContext.Current.Cache; }
        }

        public T Get<T>(string key)
        {
            return (T) Cache[key.ToLower()];
        }

        public void Set(string key, object data, int cacheTime)
        {
            if (data == null)
                return;

            Cache.Insert(key.ToLower(), data, null, DateTime.Now.AddMinutes(cacheTime), TimeSpan.Zero);
        }

        public bool IsSet(string key)
        {
            return Cache[key.ToLower()] != null;
        }

        public void Remove(string key)
        {
            Cache.Remove(key.ToLower());
        }

        public void RemoveByPattern(string pattern)
        {
            var regex = new Regex(pattern.ToLower(), RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var keysToRemove = new List<String>();

            foreach (DictionaryEntry item in Cache)
                if (regex.IsMatch(item.Key.ToString()))
                    keysToRemove.Add(item.Key.ToString());

            foreach (string key in keysToRemove)
            {
                Remove(key);
            }
        }
        public void Clear()
        {
            foreach (KeyValuePair<string,object> item in Cache)
                Remove(item.Key);
        }
    }
}
