using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using BundleTransformer.Core.Transformers;
using MvcSiteMapProvider;
using MvcSiteMapProvider.Loader;

namespace Noodle.Management.Library
{
    /// <summary>
    /// Initialize everything needed for management to work
    /// </summary>
    public class Initialization : IStartupTask
    {
        private readonly TinyIoCContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="Initialization"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public Initialization(TinyIoCContainer container)
        {
            _container = container;
        }

        /// <summary>
        /// Excute is run once on startup of the application
        /// </summary>
        public void Execute()
        {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new NoodleManagementViewEngine());

            AreaRegistration.RegisterAllAreas();

            RegisterBundles(BundleTable.Bundles);
            RegisterRoutes(RouteTable.Routes);

            SiteMaps.Loader = _container.Resolve<ISiteMapLoader>();
        }

        private void RegisterBundles(BundleCollection bundles)
        {
            var stylesBundle = new Bundle("~/managementcss");
            stylesBundle.Include(GetContent("~/content/styles/styles.less"));
            stylesBundle.Transforms.Add(new CssTransformer());
            bundles.Add(stylesBundle);

            var scriptsBundle = new Bundle("~/managementjs");
            scriptsBundle.Include(GetContent("~/content/scripts/jquery-1.9.1.js"));
            scriptsBundle.Include(GetContent("~/content/scripts/jquery-1.9.1.js"));
            scriptsBundle.Transforms.Add(new JsTransformer());
            bundles.Add(scriptsBundle);
        }

        private void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Default",
                "{controller}/{action}/{id}",
                new { controller = "Default", action = "Index", id = UrlParameter.Optional }
            );
        }

        /// <summary>
        /// Pass it in a path like /content/css/site.css
        /// If the path doesn't exist, it will return /method/content/css/site.css.
        /// This way, you can override css without modifiying the ~/Method/ directory directly for upgrading.
        /// </summary>
        /// <param name="path"></param>
        private string GetContent(string path)
        {
            return File.Exists(System.Web.Hosting.HostingEnvironment.MapPath(path))
                       ? path
                       : "~/noodle/" + path.Substring(2, path.Length - 2);
        }

        /// <summary>
        /// The order at which the startup task will run. Smaller numbers run first.
        /// </summary>
        public int Order
        {
            get { return 0; }
        }
    }
}
