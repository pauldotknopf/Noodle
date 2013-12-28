using System;
using System.Linq;
using System.Web.Routing;
using Noodle.Engine;

namespace Noodle.Web.Mvc.Routes
{
    public class RoutePublisher : IRoutePublisher
    {
        private readonly ITypeFinder _typeFinder;

        public RoutePublisher(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }

        public void RegisterRoutes(RouteCollection routes)
        {
            var routeProviderTypes = _typeFinder.Find<IRouteProvider>().Where(x => !x.IsAbstract);
            var routeProviders = routeProviderTypes.Select(providerType => Activator.CreateInstance(providerType) as IRouteProvider).ToList();
            routeProviders = routeProviders.OrderBy(rp => rp.Priority).ToList();
            routeProviders.ForEach(rp => rp.RegisterRoutes(routes));
        }
    }
}
