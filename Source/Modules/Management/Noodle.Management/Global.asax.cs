using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.UI;
using MvcSiteMapProvider;
using MvcSiteMapProvider.Builder;
using MvcSiteMapProvider.Loader;
using MvcSiteMapProvider.Security;
using MvcSiteMapProvider.Visitor;
using MvcSiteMapProvider.Web.Compilation;
using MvcSiteMapProvider.Web.Mvc;
using MvcSiteMapProvider.Web.Mvc.Filters;
using Noodle.Engine;
using Noodle.Management.Library;
using Noodle.MongoDB;

namespace Noodle.Management
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
#if DEBUG
            var dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            if (!Directory.Exists(dataDirectory))
                Directory.CreateDirectory(dataDirectory);
            try { Singleton<MongoDB.MongoServerScope>.Instance = new MongoServerScope(Path.Combine(dataDirectory, "Data", Path.GetRandomFileName()), 2929, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", "mongod.exe")); }
            catch (Exception) {/*already started..*/}
#endif

            EngineContext.TypeFinder = new AppDomainTypeFinder(new PluginAssemblyFinder());
            EngineContext.Configure(false);
        }
    }
}