using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using System.Web.Routing;
using System.Web.UI;

namespace Noodle.Web.Routing
{
    /// <summary>
    /// A route handler to a specific page
    /// </summary>
    public class PageRouteHandler : IRouteHandler
    {
        private static readonly PropertyInfo QueryStringTextPropertyInfo = typeof(HttpRequest).GetProperty("QueryStringText",
                                                                                                  BindingFlags.Instance |
                                                                                                  BindingFlags.Public |
                                                                                                  BindingFlags.NonPublic);

        private readonly string _physicalFile;

        public PageRouteHandler(string physicalFile)
        {
            _physicalFile = physicalFile;
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            HttpContext.Current.Items["RouteData"] = requestContext.RouteData;

            // Combine new with old query string
            var existingQuery = requestContext.HttpContext.Request.QueryString.AllKeys.Select(x => x + "=" + requestContext.HttpContext.Request.QueryString[x]).ToList();
            existingQuery.AddRange(requestContext.RouteData.Values.Where(x => x.Value != null).Select(x => x.Key + "=" + x.Value.ToString()));
            var newQueryText = string.Join("&", existingQuery.ToArray());
            HttpContext.Current.Items["_newQueryString"] = newQueryText;

            var page = BuildManager.CreateInstanceFromVirtualPath(_physicalFile, typeof(Page)) as Page;
            if (page == null) throw new InvalidOperationException("Could not load page from file " + _physicalFile);
            page.Load += OnLoad;
            page.Init += OnLoad;
            page.PreInit += OnLoad;

            return page;
        }

        public void OnLoad(object sender, EventArgs e)
        {
            QueryStringTextPropertyInfo.SetValue(HttpContext.Current.Request, HttpContext.Current.Items["_newQueryString"], new List<object>().ToArray());
        }

    }
}
