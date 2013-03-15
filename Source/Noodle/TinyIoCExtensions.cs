using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noodle
{
    public partial class TinyIoCContainer
    {
        /// <summary>
        /// Get all the registered services that can be casted to T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<Type> GetServicesOf<T>()
        {
            foreach (var key in _RegisteredTypes.Keys)
            {
                if (typeof (T).IsAssignableFrom(key.Type))
                   yield return key.Type;
            }
        } 
    }
}
