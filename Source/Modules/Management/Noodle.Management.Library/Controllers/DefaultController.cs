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
            var siteMapCache = new SiteMapCache(new RuntimeCacheProvider<ISiteMap>(System.Runtime.Caching.MemoryCache.Default));
            var contextFactory = new MvcContextFactory();
            var keyGenerator = new SiteMapCacheKeyGenerator(contextFactory);
            var sitemap = new NoodleSiteMap(new SiteMapPluginProvider(), )
        }

        public class NoodleSiteMap : SiteMap
        {
            public NoodleSiteMap(ISiteMapPluginProvider pluginProvider, IMvcContextFactory mvcContextFactory, ISiteMapChildStateFactory siteMapChildStateFactory, IUrlPath urlPath, ISiteMapSettings siteMapSettings)
                : base(pluginProvider, mvcContextFactory, siteMapChildStateFactory, urlPath, siteMapSettings)
            {
                
            }

            public MvcSiteMapProvider.ISiteMap GetSiteMap(string siteMapCacheKey)
            {
                throw new NotImplementedException();
            }

            public MvcSiteMapProvider.ISiteMap GetSiteMap()
            {
                throw new NotImplementedException();
            }

            public void ReleaseSiteMap(string siteMapCacheKey)
            {
                throw new NotImplementedException();
            }

            public void ReleaseSiteMap()
            {
                throw new NotImplementedException();
            }
        }
    }
}
