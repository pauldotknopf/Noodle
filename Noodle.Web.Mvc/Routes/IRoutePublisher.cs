using System.Web.Routing;

namespace Noodle.Web.Mvc.Routes
{
    public interface IRoutePublisher
    {
        void RegisterRoutes(RouteCollection routeCollection);
    }
}
