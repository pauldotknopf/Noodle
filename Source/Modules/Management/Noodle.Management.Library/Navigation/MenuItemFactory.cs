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
        private readonly MenuItemBuilder _parent;
        private readonly IList<MenuItemBuilder> _result;
        private readonly ISiteMapNodeHelper _helper;

        public MenuItemFactory(MenuItemBuilder parent, IList<MenuItemBuilder> result, ISiteMapNodeHelper helper)
        {
            _parent = parent;
            _result = result;
            _helper = helper;
        }

        public virtual MenuItemBuilder Add()
        {
            var builder = new MenuItemBuilder(_parent, _helper, _result);
            _result.Add(builder);
            return builder;
        }
    }
}
