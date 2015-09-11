using System;
using System.Collections.Specialized;

namespace Noodle.Extensions.Collections
{
    /// <summary>
    /// A namevalue collection that is easilt build for a query string
    /// </summary>
    /// <typeparam name="TK"></typeparam>
    public class QuerystringBase<TK> : NameValueCollection where TK : QuerystringBase<TK>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuerystringBase{TK}"/> class.
        /// </summary>
        public QuerystringBase()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuerystringBase{TK}"/> class with a namevalue collection (gotten from asp.net current request?)
        /// </summary>
        /// <param name="q">The q.</param>
        public QuerystringBase(NameValueCollection q)
            : base(q)
        {
        }

        /// <summary>
        /// Provides culture-invariant parsing of byte, int, double, float, bool, and enum values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T? Get<T>(string name) where T : struct, IConvertible
        {
            return this.Get<T>(name, null);
        }
        /// <summary>
        /// Provides culture-invariant parsing of byte, int, double, float, bool, and enum values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T? Get<T>(string name, T? defaultValue) where T : struct, IConvertible
        {
            return NameValueCollectionExtensions.ParsePrimitive<T>(this[name], defaultValue);
        }
        /// <summary>
        /// Provides culture-invariant parsing of byte, int, double, float, bool, and enum values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T Get<T>(string name, T defaultValue) where T : struct, IConvertible
        {
            return NameValueCollectionExtensions.ParsePrimitive<T>(this[name], defaultValue).Value;
        }
        /// <summary>
        /// Serializes the given value by calling .ToString(). If the value is null, the key is removed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public TK SetAsString<T>(string name, T val) where T : class
        {
            return (TK)NameValueCollectionExtensions.SetAsString(this, name, val);
        }

        /// <summary>
        /// Provides culture-invariant serialization of value types, in lower case for querystring readability. Setting a key to null removes it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="q"></param>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public TK Set<T>(string name, T? val) where T : struct, IConvertible
        {
            return (TK)NameValueCollectionExtensions.Set(this, name, val);
        }
    }
}
