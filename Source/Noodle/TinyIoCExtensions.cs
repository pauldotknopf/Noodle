using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noodle
{
    /// <summary>
    /// Some custom stuff for the TinyIOCContainer
    /// </summary>
    public partial class TinyIoCContainer
    {
        /// <summary>
        /// Get all the registered services that can be casted to T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<Type> GetServicesOf<T>()
        {
            return from key in _RegisteredTypes.Keys where typeof (T).IsAssignableFrom(key.Type) select key.Type;
        }

        /// <summary>
        /// Listeners can resolve unregistered types.
        /// </summary>
        public event EventHandler<TinyIoCUnregisteredType> ResolveUnregisteredType;
    }

    /// <summary>
    /// The request to resolve an unregistered type
    /// </summary>
    public class TinyIoCUnregisteredType : EventArgs
    {
        private object _result;

        /// <summary>
        /// The request (and couldn't be found)
        /// </summary>
        public TinyIoCContainer.TypeRegistration Request { get; protected set; }

        /// <summary>
        /// The container associated with the request
        /// </summary>
        public TinyIoCContainer Container { get; protected set; }

        /// <summary>
        /// The result. This will be NULL to begin with. If handlers set this instance to anything, then the result will be returned
        /// </summary>
        public object Result
        {
            get { return _result; }
            set
            {
                if(_result != null)
                {
                    if (!Request.Type.IsInstanceOfType(_result))
                        throw new InvalidOperationException("You can't return an instance that is not assignable to the requested service.");
                }
                _result = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TinyIoCUnregisteredType"/> class.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="container">The container.</param>
        public TinyIoCUnregisteredType(TinyIoCContainer.TypeRegistration request, TinyIoCContainer container)
        {
            Request = request;
            Container = container;
        }
    }
}
