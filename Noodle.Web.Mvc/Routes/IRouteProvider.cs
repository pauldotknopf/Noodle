using System.Web.Routing;

namespace Noodle.Web.Mvc.Routes
{
    public interface IRouteProvider
    {
        void RegisterRoutes(RouteCollection routes);

        int Priority { get; }
    }
}
