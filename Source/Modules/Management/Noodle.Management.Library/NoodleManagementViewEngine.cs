using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noodle.Management.Library
{
    public class NoodleManagementViewEngine : System.Web.Mvc.RazorViewEngine
    {
        public NoodleManagementViewEngine()
        {
            AreaViewLocationFormats = new[] {
                "~/Areas/{2}/Views/{1}/{0}.cshtml",
                "~/Areas/{2}/Views/{1}/{0}.vbhtml",
                "~/Areas/{2}/Views/Shared/{0}.cshtml",
                "~/Areas/{2}/Views/Shared/{0}.vbhtml",

                "~/Modules/{2}/Views/{1}/{0}.cshtml",
                "~/Modules/{2}/Views/{1}/{0}.vbhtml",
                "~/Modules/{2}/Views/Shared/{0}.cshtml",
                "~/Modules/{2}/Views/Shared/{0}.vbhtml",

                "~/Noodle/Modules/{2}/Views/{1}/{0}.cshtml",
                "~/Noodle/Modules/{2}/Views/{1}/{0}.vbhtml",
                "~/Noodle/Modules/{2}/Views/Shared/{0}.cshtml",
                "~/Noodle/Modules/{2}/Views/Shared/{0}.vbhtml"
            };
            AreaMasterLocationFormats = new[] {
                "~/Areas/{2}/Views/{1}/{0}.cshtml",
                "~/Areas/{2}/Views/{1}/{0}.vbhtml",
                "~/Areas/{2}/Views/Shared/{0}.cshtml",
                "~/Areas/{2}/Views/Shared/{0}.vbhtml",

                 "~/Modules/{2}/Views/{1}/{0}.cshtml",
                "~/Modules/{2}/Views/{1}/{0}.vbhtml",
                "~/Modules/{2}/Views/Shared/{0}.cshtml",
                "~/Modules/{2}/Views/Shared/{0}.vbhtml",

                "~/Noodle/Modules/{2}/Views/{1}/{0}.cshtml",
                "~/Noodle/Modules/{2}/Views/{1}/{0}.vbhtml",
                "~/Noodle/Modules/{2}/Views/Shared/{0}.cshtml",
                "~/Noodle/Modules/{2}/Views/Shared/{0}.vbhtml"
            };
            AreaPartialViewLocationFormats = new[] {
                "~/Areas/{2}/Views/{1}/{0}.cshtml",
                "~/Areas/{2}/Views/{1}/{0}.vbhtml",
                "~/Areas/{2}/Views/Shared/{0}.cshtml",
                "~/Areas/{2}/Views/Shared/{0}.vbhtml",

                "~/Modules/{2}/Views/{1}/{0}.cshtml",
                "~/Modules/{2}/Views/{1}/{0}.vbhtml",
                "~/Modules/{2}/Views/Shared/{0}.cshtml",
                "~/Modules/{2}/Views/Shared/{0}.vbhtml",

                "~/Noodle/Modules/{2}/Views/{1}/{0}.cshtml",
                "~/Noodle/Modules/{2}/Views/{1}/{0}.vbhtml",
                "~/Noodle/Modules/{2}/Views/Shared/{0}.cshtml",
                "~/Noodle/Modules/{2}/Views/Shared/{0}.vbhtml"
            };

            ViewLocationFormats = new[] {
                "~/Views/{1}/{0}.cshtml",
                "~/Views/{1}/{0}.vbhtml",
                "~/Views/Shared/{0}.cshtml",
                "~/Views/Shared/{0}.vbhtml",

                "~/Noodle/Views/{1}/{0}.cshtml",
                "~/Noodle/Views/{1}/{0}.vbhtml",
                "~/Noodle/Views/Shared/{0}.cshtml",
                "~/Noodle/Views/Shared/{0}.vbhtml"
            };
            MasterLocationFormats = new[] {
                "~/Views/{1}/{0}.cshtml",
                "~/Views/{1}/{0}.vbhtml",
                "~/Views/Shared/{0}.cshtml",
                "~/Views/Shared/{0}.vbhtml",

                "~/Noodle/Views/{1}/{0}.cshtml",
                "~/Noodle/Views/{1}/{0}.vbhtml",
                "~/Noodle/Views/Shared/{0}.cshtml",
                "~/Noodle/Views/Shared/{0}.vbhtml"
            };
            PartialViewLocationFormats = new[] {
                "~/Views/{1}/{0}.cshtml",
                "~/Views/{1}/{0}.vbhtml",
                "~/Views/Shared/{0}.cshtml",
                "~/Views/Shared/{0}.vbhtml",

                "~/Noodle/Views/{1}/{0}.cshtml",
                "~/Noodle/Views/{1}/{0}.vbhtml",
                "~/Noodle/Views/Shared/{0}.cshtml",
                "~/Noodle/Views/Shared/{0}.vbhtml"
            };
        }
    }
}
