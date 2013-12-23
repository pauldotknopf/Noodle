using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using MvcSiteMapProvider;
using MvcSiteMapProvider.Caching;
using MvcSiteMapProvider.Loader;
using MvcSiteMapProvider.Web;
using MvcSiteMapProvider.Web.Mvc;

namespace Noodle.Management.Library.Controllers
{
    public class DefaultController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
