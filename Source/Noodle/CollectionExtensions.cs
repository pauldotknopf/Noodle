using System;
using System.Collections.Generic;
using System.Linq;

namespace Noodle
{
    public static class CollectionExtensions
    {
        public static T GetValue<T>(this IDictionary<string, string> dictionary, string key, T defaultValue)
        {
            return dictionary.ContainsKey(key) ? CommonHelper.To<T>(dictionary[key]) : defaultValue;
        }

        public static T GetValue<T>(this IDictionary<string, string> dictionary, string key)
        {
            return dictionary.ContainsKey(key) ? CommonHelper.To<T>(dictionary[key]) : default(T);
        }
        public static IEnumerable<T> Randomize<T>(this IEnumerable<T> source)
        {
            var rnd = new Random();
            return source.OrderBy((item) => rnd.Next());
        }
    }
}
