using System.Web.Mvc;
using System.Web.Routing;

namespace Noodle.Web.Mvc.Routes
{
    public abstract class AreaRouteProvider : IRouteProvider
    {
        public abstract void RegisterAreaRoutes(AreaRegistrationContext context);

        public abstract string AreaName { get; }

        public void RegisterRoutes(RouteCollection routes)
        {
            var context = new AreaRegistrationContext(AreaName, routes, null);

            var thisNamespace = GetType().Namespace;
            if (thisNamespace != null)
                context.Namespaces.Add(thisNamespace + ".*");

            RegisterAreaRoutes(context);
        }

        public virtual int Priority
        {
            get { return RouteProviderSortOrder.Area; }
        }
    }
}
