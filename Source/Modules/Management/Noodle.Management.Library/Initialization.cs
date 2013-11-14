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
using Noodle.Web.Mvc;

namespace Noodle.Management.Library
{
    /// <summary>
    /// Initialize everything needed for management to work
    /// </summary>
    public class Initialization : IStartupTask
    {
        public void Execute()
        {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new NoodleManagementViewEngine());

            DependencyResolver.SetResolver(new NoodleDependencyResolver(EngineContext.Current));

            AreaRegistration.RegisterAllAreas();

            RegisterBundles(BundleTable.Bundles);
            RegisterRoutes(RouteTable.Routes);
        }

        private void RegisterBundles(BundleCollection bundles)
        {

            //bundles.Add<StylesheetBundle>("managementcss", new List<string>
            //                                         {
            //                                             GetContent("content/css/others/boilerplate.css"),
            //                                             GetContent("content/css/others/jquery-layout.css"),
            //                                             GetContent("content/css/others/jquery-ui-1.8.18.css"),
            //                                             GetContent("content/css/site.less")
            //                                         });

            //bundles.Add<ScriptBundle>("managementjs", new List<string>
            //                                         {
            //                                             GetContent("scripts/jquery-1.7.1.js"),
            //                                             GetContent("scripts/jquery-ui-1.8.18.js"),
            //                                             GetContent("scripts/modernizr-2.5.3.js"),
            //                                             GetContent("scripts/jquery.validate.js"),
            //                                             GetContent("scripts/jquery.validate.unobtrusive.js"),
            //                                             GetContent("scripts/jquery.unobtrusive-ajax.js"),
            //                                             GetContent("scripts/jquery.layout-1.3.0.rc30.4.js"),
            //                                             GetContent("scripts/jquery.blockUI.js"),
            //                                             GetContent("scripts/jquery.form.js"),
            //                                             GetContent("scripts/tiny_mce/jquery.tinymce.js"),
            //                                             GetContent("scripts/Kendo/kendo.web.min.js"),
            //                                             GetContent("scripts/Kendo/kendo.aspnetmvc.min.js"),
            //                                             GetContent("scripts/method.bootstrap.js"),
            //                                             GetContent("scripts/method.management.js")
            //                                         });
            var stylesBundle = new Bundle("~/managementcss");
            stylesBundle.Include(GetContent("~/content/styles/styles.less"));
            stylesBundle.Transforms.Add(new CssTransformer());
            bundles.Add(stylesBundle);

            var scriptsBundle = new Bundle("~/managementjs");
            scriptsBundle.Include(GetContent("~/content/scripts/jquery-1.9.1.js"));
            scriptsBundle.Include(GetContent("~/content/scripts/jquery-1.9.1.js"));
            scriptsBundle.Transforms.Add(new JsTransformer());
            bundles.Add(scriptsBundle);

            bundles.Add(new Bundle("~/scripts")
                .Include("~/noodle/content/scripts/jquery.min.js",
                    "~/noodle/content/scripts/kendo/kendo.web.min.js"));
            bundles.Add(new Bundle("~/styles")
                .Include("~/noodle/content/scripts/jquery.min.js",
                    "~/noodle/content/scripts/kendo/kendo.web.min.js"));
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

        public int Order
        {
            get { return 0; }
        }
    }
}
