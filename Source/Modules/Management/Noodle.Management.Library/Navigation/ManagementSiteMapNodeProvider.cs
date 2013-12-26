using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvcSiteMapProvider;
using MvcSiteMapProvider.Builder;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagementSiteMapNodeProvider"/> class.
        /// </summary>
        /// <param name="noodleSiteMapNodeProviders">The noodle site map node providers.</param>
        public ManagementSiteMapNodeProvider(INoodleSiteMapNodeProvider[] noodleSiteMapNodeProviders)
        {
            _noodleSiteMapNodeProviders = noodleSiteMapNodeProviders;
        }

        /// <summary>
        /// Gets the site map nodes.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <returns></returns>
        public IEnumerable<ISiteMapNodeToParentRelation> GetSiteMapNodes(ISiteMapNodeHelper helper)
        {
            var result = new List<ISiteMapNodeToParentRelation>();
            foreach(var noodleSiteMapNodeProvider in _noodleSiteMapNodeProviders.OrderBy(x => x.SortOrder))
                result.AddRange(noodleSiteMapNodeProvider.GetSiteMapNodes(helper));
            return result;
        }
    }
}
