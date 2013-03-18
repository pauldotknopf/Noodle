using System;
using System.Web;
using System.Web.Routing;
using Noodle.Web.Routing;

namespace Noodle.Web.Resources
{
    public class EmbeddedResourceHandler : IStartupTask
    {
        static EmbeddedResourceHandler()
        {
            EmebeddedRoutes.MapEmbeddedResource("jquery", "noodle/embedded/jquery", typeof(EmbeddedResourceHandler).Assembly, "Noodle.Web.Resources.Content.jquery.js");
            EmebeddedRoutes.MapEmbeddedResource("bootstrapjs", "noodle/embedded/bootstrapjs", typeof(EmbeddedResourceHandler).Assembly, "Noodle.Web.Resources.Content.bootstrap.js");
            EmebeddedRoutes.MapEmbeddedResource("bootstrapcss", "noodle/embedded/bootstrapcss", typeof(EmbeddedResourceHandler).Assembly, "Noodle.Web.Resources.Content.bootstrap.css");
            EmebeddedRoutes.MapEmbeddedResource("knockout", "noodle/embedded/knockout", typeof(EmbeddedResourceHandler).Assembly, "Noodle.Web.Resources.Content.knockout.js");
            EmebeddedRoutes.MapEmbeddedResource("knockoutmapping", "noodle/embedded/knockoutmapping", typeof(EmbeddedResourceHandler).Assembly, "Noodle.Web.Resources.Content.knockoutmapping.js");
        }

        public static RouteCollection EmebeddedRoutes = new RouteCollection();

        public void Execute()
        {
            EventBroker.Instance.PostResolveAnyRequestCache += (sender, e) =>
            {
                var httpApplication = sender as HttpApplication;
                if (httpApplication == null)
                    return;

                var url = new Url(httpApplication.Context.Request.RawUrl);

                if (!string.IsNullOrEmpty(url.Path) && url.Path.StartsWith("/noodle/embedded", StringComparison.InvariantCultureIgnoreCase))
                {
                    var httpContextBase = new HttpContextWrapper(httpApplication.Context);
                    var routeData = EmebeddedRoutes.GetRouteData(httpContextBase);
                    if (routeData == null)
                        return;

                    httpApplication.Context.RemapHandler(routeData.RouteHandler.GetHttpHandler(new RequestContext(httpContextBase, routeData)));
                }
            };
        }

        public int Order
        {
            get { return 0; }
        }
    }
}
