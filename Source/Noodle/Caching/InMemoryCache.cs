using System.Web;
using System.Web.Caching;

namespace Noodle.Caching
{
    public class InMemoryCache : HttpRuntimeCache
    {
        public InMemoryCache()
        {
            RemoveByPattern("");
        }

        public override Cache Cache
        {
            get { return HttpRuntime.Cache; }
        }
    }
}
