using System.Collections.Generic;
using MvcSiteMapProvider.Builder;

namespace Noodle.Management.Library.Navigation
{
    public class FluentSiteMapNodeFactory
    {
        private readonly IList<FluentSiteMapNodeBuilder> _result;
        private readonly ISiteMapNodeHelper _helper;

        public FluentSiteMapNodeFactory(IList<FluentSiteMapNodeBuilder> result, ISiteMapNodeHelper helper)
        {
            _result = result;
            _helper = helper;
        }

        public virtual FluentSiteMapNodeBuilder Add()
        {
            var builder = new FluentSiteMapNodeBuilder(_helper);
            _result.Add(builder);
            return builder;
        }
    }
}
