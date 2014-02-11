using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvcSiteMapProvider;

namespace Noodle.Management.Library.Navigation
{
    /// <summary>
    /// Implementations of this interface get to serve sitemap nodes steming from the root of the "Management" menu
    /// </summary>
    public interface INoodleSiteMapNodeProvider : ISiteMapNodeProvider
    {
        /// <summary>
        /// The order of this branch within the noodle/management menu, sorted least to greatest
        /// </summary>
        int SortOrder { get; }
    }
}
