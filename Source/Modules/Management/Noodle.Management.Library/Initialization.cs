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
using Noodle.Security.Permissions;

namespace Noodle.Management.Library
{
    /// <summary>
    /// Initialize everything needed for management to work
    /// </summary>
    public class Initialization : IStartupTask
    {
        private readonly TinyIoCContainer _container;
        private readonly IPermissionService _permissionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="Initialization" /> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="permissionService">The permission service.</param>
        public Initialization(TinyIoCContainer container, IPermissionService permissionService)
        {
            _container = container;
            _permissionService = permissionService;
        }

        /// <summary>
        /// Excute is run once on startup of the application
        /// </summary>
        public void Execute()
        {
            _permissionService.InstallFoundPermissions();

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new NoodleManagementViewEngine());

            RegisterBundles(BundleTable.Bundles);

            SiteMaps.Loader = _container.Resolve<ISiteMapLoader>();
        }

        private void RegisterBundles(BundleCollection bundles)
        {
            var stylesBundle = new Bundle("~/managementcss");
            stylesBundle.Include(GetContent("~/content/styles/styles.less"));
            stylesBundle.Transforms.Add(new CssTransformer());
            bundles.Add(stylesBundle);

            var preManagementScriptsBundle = new Bundle("~/premanagementjs");
            preManagementScriptsBundle.Include(GetContent("~/content/scripts/modernizr.js"));
            preManagementScriptsBundle.Include(GetContent("~/content/scripts/respond.js"));
            preManagementScriptsBundle.Transforms.Add(new JsTransformer());
            bundles.Add(preManagementScriptsBundle);

            var managementScriptsBundle = new Bundle("~/managementjs");
            managementScriptsBundle.Include(GetContent("~/content/scripts/jquery-1.9.1.js"));
            managementScriptsBundle.Include(GetContent("~/content/scripts/jquerytax.js"));
            managementScriptsBundle.Transforms.Add(new JsTransformer());
            bundles.Add(managementScriptsBundle);
        }

        private void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Default",
                "{controller}/{action}/{id}",
                new { controller = "Default", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute("DefaultRoute",
                "Logging/{controller}/{action}/{id}",
                new { area="Logging", controller = "Default", action = "Index", id = UrlParameter.Optional }
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
