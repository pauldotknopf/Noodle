using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using MvcSiteMapProvider;
using MvcSiteMapProvider.Builder;

namespace Noodle.Management.Library.Navigation
{
    public class MenuItemFactory
    {
        private readonly IList<MenuItemBuilder> _result;
        private readonly ISiteMapNodeHelper _helper;

        public MenuItemFactory(IList<MenuItemBuilder> result, ISiteMapNodeHelper helper)
        {
            _result = result;
            _helper = helper;
        }

        public virtual MenuItemBuilder Add()
        {
            var builder = new MenuItemBuilder(_helper);
            _result.Add(builder);
            return builder;
        }
    }
}
