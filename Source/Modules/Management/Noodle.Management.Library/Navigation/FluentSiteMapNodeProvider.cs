using System.Collections.Generic;
using System.Security.Cryptography;
using Antlr.Runtime.Misc;
using MvcSiteMapProvider;
using MvcSiteMapProvider.Builder;

namespace Noodle.Management.Library.Navigation
{
    /// <summary>
    /// The base class for building nodes fluently
    /// </summary>
    public abstract class FluentSiteMapNodeProvider : ISiteMapNodeProvider
    {
        public abstract void BuildSitemapNodes(FluentSiteMapNodeFactory fluentSiteMapNodeFactory); 

        public IEnumerable<ISiteMapNodeToParentRelation> GetSiteMapNodes(ISiteMapNodeHelper helper)
        {
            var builders = new List<FluentSiteMapNodeBuilder>();
            var menuItemFactory = new FluentSiteMapNodeFactory(builders, helper);

            BuildSitemapNodes(menuItemFactory);

            var nodes = new List<ISiteMapNodeToParentRelation>();
            foreach (var builder in builders)
                RecursivelyBuildNodes(helper, null, builder, nodes);
            return nodes;
        }

        private void RecursivelyBuildNodes(ISiteMapNodeHelper helper, ISiteMapNodeToParentRelation parent, FluentSiteMapNodeBuilder builder, List<ISiteMapNodeToParentRelation> nodes)
        {
            var node = builder.CreateNode(helper, parent != null ? parent.Node : null);
            nodes.Add(node);
            if (builder.Children != null)
                foreach (var child in builder.Children)
                    RecursivelyBuildNodes(helper, node, child, nodes);
        }
    }
}
