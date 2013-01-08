using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;

namespace Noodle.Caching
{
    public class PerRequestCache : ICacheManager
    {
        public virtual IDictionary Cache
        {
            get
            {
                if (HttpContext.Current == null)
                    return null;
                return HttpContext.Current.Items;
            }
        }

        public T Get<T>(string key)
        {
            return (T)Cache[key];
        }

        public void Set(string key, object data, int cacheTime)
        {
            var items = Cache;
            if (items == null)
                return;

            if (data != null)
            {
                if (items.Contains(key))
                    items[key] = data;
                else
                    items.Add(key, data);
            }
        }

        public bool IsSet(string key)
        {
            var items = Cache;
            if (items == null)
                return false;

            return (items[key] != null);
        }

        public void Remove(string key)
        {
            var items = Cache;
            if (items == null)
                return;

            items.Remove(key);
        }

        public void RemoveByPattern(string pattern)
        {
            var items = Cache;
            if (items == null)
                return;

            var enumerator = items.GetEnumerator();
            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var keysToRemove = new List<String>();
            while (enumerator.MoveNext())
            {
                if (regex.IsMatch(enumerator.Key.ToString()))
                {
                    keysToRemove.Add(enumerator.Key.ToString());
                }
            }

            foreach (string key in keysToRemove)
            {
                items.Remove(key);
            }
        }

        public void Clear()
        {
            var items = Cache;
            if (items == null)
                return;

            var enumerator = items.GetEnumerator();
            var keysToRemove = new List<String>();
            while (enumerator.MoveNext())
            {
                keysToRemove.Add(enumerator.Key.ToString());
            }

            foreach (string key in keysToRemove)
            {
                items.Remove(key);
            }
        }
    }
}
