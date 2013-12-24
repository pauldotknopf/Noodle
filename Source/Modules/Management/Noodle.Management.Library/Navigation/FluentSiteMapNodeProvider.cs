using System.Collections.Generic;
using Antlr.Runtime.Misc;
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
            var builders = new List<MenuItemBuilder>();
            var menuItemFactory = new MenuItemFactory(null, builders, helper);

            menuItemFactory.Add().Items(children =>
            {
                children.Add();
                children.Add().Items(c =>
                {
                    c.Add();
                });
            });

            var nodes = new List<ISiteMapNodeToParentRelation>();
            foreach (var builder in builders)
                RecursivelyBuildNodes(helper, null, builder, nodes);
            return nodes;
        }

        private void RecursivelyBuildNodes(ISiteMapNodeHelper helper, ISiteMapNodeToParentRelation parent, MenuItemBuilder builder, List<ISiteMapNodeToParentRelation> nodes)
        {
            var node = helper.CreateNode("", parent != null ? parent.Node.Key : string.Empty, "", "");
            nodes.Add(node);
            if (builder.Children != null)
                foreach (var child in builder.Children)
                    RecursivelyBuildNodes(helper, node, child, nodes);
        }
    }
}
