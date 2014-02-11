using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvcSiteMapProvider;
using MvcSiteMapProvider.Builder;
using WebGrease.Css.Extensions;

namespace Noodle.Management.Library.Navigation
{
    /// <summary>
    /// This is the base ISiteMapNodeProvider that bulds the entire Noodle/Management navigation.
    /// Internally, it returns nodes from registered INoodleSiteMapNodeProviders.
    /// Implement an INoodleSiteMapNodeProvider and register as an INoodleSiteMapNodeProvider, and this class will serve it up.
    /// </summary>
    public class ManagementSiteMapNodeProvider : ISiteMapNodeProvider
    {
        private readonly INoodleSiteMapNodeProvider[] _noodleSiteMapNodeProviders;
        private readonly IFluentFactory _fluentFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagementSiteMapNodeProvider" /> class.
        /// </summary>
        /// <param name="noodleSiteMapNodeProviders">The noodle site map node providers.</param>
        /// <param name="fluentFactory">The fluent factory.</param>
        public ManagementSiteMapNodeProvider(INoodleSiteMapNodeProvider[] noodleSiteMapNodeProviders, IFluentFactory fluentFactory)
        {
            _noodleSiteMapNodeProviders = noodleSiteMapNodeProviders;
            _fluentFactory = fluentFactory;
        }

        /// <summary>
        /// Gets the site map nodes.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <returns></returns>
        public IEnumerable<ISiteMapNodeToParentRelation> GetSiteMapNodes(ISiteMapNodeHelper helper)
        {
            var result = new List<ISiteMapNodeToParentRelation>();

            var rootNode = new FluentSiteMapNodeBuilder(_fluentFactory, helper).Title("Home")
                .Controller("Default")
                .Action("Index")
                .CreateNode(helper, null);
            result.Add(rootNode);

            foreach (var nodeToParent in _noodleSiteMapNodeProviders.OrderBy(x => x.SortOrder).SelectMany(x => x.GetSiteMapNodes(helper)))
            {
                // put all of the root nodes from our providers onto our fixed root node "Home".
                result.Add(string.IsNullOrEmpty(nodeToParent.ParentKey)
                    ? new SiteMapNodeToParentRelation(rootNode.Node.Key, nodeToParent.Node, nodeToParent.SourceName)
                    : nodeToParent);
            }

            return result;
        }
    }
}
