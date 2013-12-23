using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvcSiteMapProvider;

namespace Noodle.Management.Library.Navigation
{
    public class MenuItemBuilder
    {
        private readonly ISiteMapNode _node;

        public MenuItemBuilder(ISiteMapNode node)
        {
            _node = node;
        }
    }
}
