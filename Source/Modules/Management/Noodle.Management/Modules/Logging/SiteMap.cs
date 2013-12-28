using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcSiteMapProvider.Builder;
using Noodle.Management.Library.Navigation;

namespace Noodle.Management.Logging
{
    public class SiteMap : FluentSiteMapNodeProvider, INoodleSiteMapNodeProvider
    {
        public SiteMap(IFluentFactory fluentFactory)
            :base(fluentFactory)
        {
            
        }

        public override void BuildSitemapNodes(IFluentSiteMapNodeFactory fluentSiteMapNodeFactory)
        {
            fluentSiteMapNodeFactory.Add().Title("Logging").Area("Logging").Controller("Logging").Action("List");
        }

        public override bool UseNestedDynamicNodeRecursion
        {
            get { return true; }
        }

        public int SortOrder
        {
            get { return 0; }
        }
    }
}