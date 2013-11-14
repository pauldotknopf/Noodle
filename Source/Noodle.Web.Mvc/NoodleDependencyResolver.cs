using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Noodle.Web.Mvc
{
    /// <summary>
    /// Resolver for MVC integration
    /// </summary>
    public class NoodleDependencyResolver : IDependencyResolver
    {
        private readonly TinyIoCContainer _container;

        public NoodleDependencyResolver(Noodle.TinyIoCContainer container)
        {
            _container = container;
        }

        public object GetService(Type serviceType)
        {
            return _container.CanResolve(serviceType) ? _container.Resolve(serviceType) : null;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _container.ResolveAll(serviceType);
        }
    }
}
