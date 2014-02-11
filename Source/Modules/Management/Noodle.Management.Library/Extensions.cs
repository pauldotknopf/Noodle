using System;
using System.Linq;
using System.Web.Mvc;
using Noodle.Localization;
using Noodle.Localization.Services;
using Noodle.Security.Permissions;
using Noodle.Web.Mvc;

namespace Noodle.Management.Library
{
    public static class Extensions
    {
        public static SelectList ToSelectList<TEnum>(this TEnum enumObj, ILocalizationService localizationService, bool markCurrentAsSelected = true) where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum) throw new ArgumentException("An Enumeration type is required.", "enumObj");

            var values = from TEnum enumValue in Enum.GetValues(typeof(TEnum))
                         select new { ID = Convert.ToInt32(enumValue), Name = localizationService.GetLocalizedEnum(enumValue) };

            object selectedValue = null;
            if (markCurrentAsSelected)
                selectedValue = Convert.ToInt32(enumObj);

            return new SelectList(values, "ID", "Name", selectedValue);
        }

        public static FormActionResult<T> RequiredPermissions<T>(this FormActionResult<T> form, IPermissionService permissionsService, params string[] permissions) where T : class
        {
            form.IsAuthorized.Add(() => permissions.All(permissionsService.Authorize));
            return form;
        } 
    }
}
