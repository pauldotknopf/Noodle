using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvcSiteMapProvider;
using MvcSiteMapProvider.Builder;

namespace Noodle.Management.Library.Navigation
{
    public class MenuItemBuilder
    {
        private readonly MenuItemBuilder _node;
        private readonly ISiteMapNodeHelper _helper;
        private readonly IList<MenuItemBuilder> _result;
        private readonly IList<MenuItemBuilder> _children = new List<MenuItemBuilder>();

        public MenuItemBuilder(MenuItemBuilder node, ISiteMapNodeHelper helper, IList<MenuItemBuilder> result)
        {
            _node = node;
            _helper = helper;
            _result = result;
        }

        public MenuItemBuilder Items(Action<MenuItemFactory> children)
        {
            if (children == null)
                return this;

            var factory = new MenuItemFactory(this, _children, _helper);
            children(factory);

            return this;
        }

        public IList<MenuItemBuilder> Children { get { return _children; } } 
    }
}
