using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Noodle.Web.Mvc
{
    /// <summary>
    /// A IDependencyResolver that resolves services from the container.
    /// </summary>
    public class NoodleDependencyResolver : IDependencyResolver
    {
        private readonly TinyIoCContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="NoodleDependencyResolver"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public NoodleDependencyResolver(TinyIoCContainer container)
        {
            _container = container;
        }

        /// <summary>
        /// Resolves singly registered services that support arbitrary object creation.
        /// </summary>
        /// <param name="serviceType">The type of the requested service or object.</param>
        /// <returns>
        /// The requested service or object.
        /// </returns>
        public object GetService(Type serviceType)
        {
            try
            {
                if(!_container.CanResolve(serviceType))
                    throw new Exception("Service not registered");

                return _container.Resolve(serviceType);
            }
            catch (TinyIoCResolutionException ex)
            {
                // We want this error to be displayed.
                throw;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Resolves multiply registered services.
        /// </summary>
        /// <param name="serviceType">The type of the requested services.</param>
        /// <returns>
        /// The requested services.
        /// </returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return _container.ResolveAll(serviceType);
            }
            catch
            {
                return Enumerable.Empty<object>();
            }
        }
    }
}
