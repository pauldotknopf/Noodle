using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noodle.TinyIoC
{
    public partial class TinyIoCContainer
    {
        /// <summary>
        /// Returns all services that have an implementation that can be casted to T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal IEnumerable<Type> GetServiceWithImplementationsOf<T>()
        {
            foreach (var key in _RegisteredTypes.Keys)
            {
                ObjectFactoryBase objectFactory;
                if (!_RegisteredTypes.TryGetValue(key, out objectFactory)) continue;
                if (typeof (T).IsAssignableFrom(objectFactory.CreatesType))
                    yield return key.Type;
            }
        }
    }
}
