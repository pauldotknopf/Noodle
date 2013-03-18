using System;
using System.Reflection;
using System.Web.Routing;

namespace Noodle.Web.Routing
{
    /// <summary>
    /// Ported from .Net 4
    /// Extensions to handle route mapping.
    /// </summary>
    public static class RouteCollectionExtensions
    {
        public static Route MapPageRoute(this RouteCollection route, string routeName, string routeUrl, string physicalFile)
        {
            return MapPageRoute(route, routeName, routeUrl, physicalFile, null, null, null);
        }

        public static Route MapPageRoute(this RouteCollection route, string routeName, string routeUrl, string physicalFile, RouteValueDictionary defaults)
        {
            return MapPageRoute(route, routeName, routeUrl, physicalFile, defaults, null, null);
        }

        public static Route MapPageRoute(this RouteCollection route, string routeName, string routeUrl, string physicalFile, RouteValueDictionary defaults, RouteValueDictionary constraints)
        {
            return MapPageRoute(route, routeName, routeUrl, physicalFile, defaults, constraints, null);
        }

        public static Route MapPageRoute(this RouteCollection route, string routeName, string routeUrl, string physicalFile, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens)
        {
            if (routeUrl == null)
                throw new ArgumentNullException("routeUrl");
            var item = new Route(routeUrl, defaults, constraints, dataTokens, new PageRouteHandler(physicalFile));
            route.Add(routeName, item);
            return item;
        }

        public static Route MapEmbeddedResource(this RouteCollection route, string routeName, string routeUrl, Assembly assembly, string resourceName)
        {
            if (routeUrl == null)
                throw new ArgumentNullException("routeUrl");
            var item = new Route(routeUrl, new EmbeddedResourceRouteHandler(assembly, resourceName));
            route.Add(routeName, item);
            return item;
        }
    }
}
