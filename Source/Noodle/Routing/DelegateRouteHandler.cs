using System;
using System.Web;
using System.Web.Routing;

namespace Noodle.Routing
{
    /// <summary>
    /// Delegate route handler for quick mocking or quick handlers
    /// </summary>
    public class DelegateRouteHandler : IRouteHandler
    {
        private readonly Func<RequestContext, IHttpHandler> _action;

        public DelegateRouteHandler(Func<RequestContext, IHttpHandler> action)
        {
            _action = action;
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return _action(requestContext);
        }
    }
}
