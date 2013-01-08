using System.Web;
using System.Web.Caching;

namespace Noodle.Caching
{
    public class InMemoryCache : HttpRuntimeCache
    {
        private Cache _cache;

        public InMemoryCache()
        {
            _cache = new Cache();
        }

        public override Cache Cache
        {
            get
            {
                return HttpRuntime.Cache;
            }
        }
    }
}
