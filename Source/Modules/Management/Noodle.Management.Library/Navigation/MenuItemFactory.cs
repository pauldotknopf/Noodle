using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using MvcSiteMapProvider;

namespace Noodle.Management.Library.Navigation
{
    public class MenuItemFactory
    {
        private readonly ISiteMapNodeFactory _factory;
        private readonly ISiteMap _sitemap;

        public MenuItemFactory(ISiteMapNodeFactory factory, ISiteMap sitemap)
        {
            _factory = factory;
            _sitemap = sitemap;
        }

        public virtual MenuItemBuilder Add(string key)
        {
            var item = _factory.Create(_sitemap, key, "");
            return new MenuItemBuilder(item);
        }
    }
}
