using System.Collections.Generic;
using MvcSiteMapProvider;
using MvcSiteMapProvider.Builder;

namespace Noodle.Management.Library.Navigation
{
    public class FluentSiteMapNodeProvider : ISiteMapNodeProvider
    {
        private readonly ISiteMapNodeFactory _sitemapNodeFactory;

        public FluentSiteMapNodeProvider(ISiteMapNodeFactory sitemapNodeFactory)
        {
            _sitemapNodeFactory = sitemapNodeFactory;
        }

        public IEnumerable<ISiteMapNodeToParentRelation> GetSiteMapNodes(ISiteMapNodeHelper helper)
        {
            return new List<ISiteMapNodeToParentRelation>();
            //var menuItemFactory = new MenuItemFactory(_sitemapNodeFactory, siteMap);
        }
    }
}
