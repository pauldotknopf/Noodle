using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Noodle.Web.Mvc.Routes;

namespace Noodle.Management.Logging
{
    public class RouteProvider : AreaRouteProvider
    {
        public override void RegisterAreaRoutes(AreaRegistrationContext context)
        {
            context.MapRoute(
                "LogDefault",
                "Log/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }

        public override string AreaName
        {
            get { return "Logging"; }
        }
    }
}