using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.ModelBinding;

namespace Noodle.Web.Mvc.Routes
{
    public static class RouteProviderSortOrder
    {
        public const int Pre = -2;
        public const int Area = -1;
        public const int Regular = 0;
        public const int Post = 1;
    }
}
