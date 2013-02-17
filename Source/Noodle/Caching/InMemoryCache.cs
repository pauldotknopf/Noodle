using System.Web;
using System.Web.Caching;

namespace Noodle.Caching
{
    /// <summary>
    /// This implemention of ICacheManager stores items in memory.
    /// </summary>
    public class InMemoryCache : HttpRuntimeCache
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryCache"/> class.
        /// </summary>
        public InMemoryCache()
        {
            RemoveByPattern("");
        }

        /// <summary>
        /// Gets the cache.
        /// </summary>
        /// <value>
        /// The cache.
        /// </value>
        public override Cache Cache
        {
            get { return HttpRuntime.Cache; }
        }
    }
}
