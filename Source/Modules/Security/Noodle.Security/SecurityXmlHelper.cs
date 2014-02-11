using System.Text;
using Noodle.Security.Activity;
using Noodle.Security.Permissions;

namespace Noodle.Security
{
    public static class SecurityXmlHelper
    {
        #region Permissions

        public static string ConvertPermissionProviderToXml(IPermissionProvider permissionProvider)
        {
            var xmlString = new StringBuilder();

            xmlString.AppendFormat("<{0}>", "permissionProvider");

            #region Permissions

            xmlString.AppendFormat("<{0}>", "permissions");

            foreach(var permission in permissionProvider.GetPermissions())
            {
                xmlString.Append(SerializePermission(permission));
            }

            xmlString.AppendFormat("</{0}>", "permissions");

            #endregion

            #region Default Permissions

            xmlString.AppendFormat("<{0}>", "defaultPermissions");

            foreach (var defaultPermission in permissionProvider.GetDefaultPermissions())
            {
                xmlString.AppendLine(SerializeDefaultPermission(defaultPermission));
            }

            xmlString.AppendFormat("</{0}>", "defaultPermissions");

            #endregion

            xmlString.AppendFormat("</{0}>", "permissionProvider");

            return xmlString.ToString();
        }

        private static string SerializeDefaultPermission(DefaultPermissionRecord defaultPermssion)
        {
            var xmlString = new StringBuilder();

            xmlString.AppendFormat("<{0}>", "defaultPermission");

            xmlString.AppendFormat("<role>{0}</role>", defaultPermssion.UserRoleSystemName);

            xmlString.AppendFormat("<{0}>", "permissions");

            foreach(var permission in defaultPermssion.PermissionRecords)
            {
                xmlString.Append(SerializePermission(permission));
            }

            xmlString.AppendFormat("</{0}>", "permissions");

            xmlString.AppendFormat("</{0}>", "defaultPermission");

            return xmlString.ToString();
        }

        private static string SerializePermission(PermissionRecord permission)
        {
            var xmlString = new StringBuilder();

            xmlString.AppendFormat("<{0}>", "permission");

            xmlString.AppendFormat("<name>{0}</name>", permission.Name);
            xmlString.AppendFormat("<systemName>{0}</systemName>", permission.SystemName);
            xmlString.AppendFormat("<category>{0}</category>", permission.Category);

            xmlString.AppendFormat("</{0}>", "permission");

            return xmlString.ToString();
        }

        #endregion

        #region Activities

        public static string ConvertActivityLogTypeProviderToXml(IActivityLogTypeProvider activityLogTypeProvider)
        {
            var xmlString = new StringBuilder();

            xmlString.AppendFormat("<{0}>", "activitylogTypes");

            foreach (var logType in activityLogTypeProvider.GetActivityLogTypes())
            {
                xmlString.AppendFormat("<{0}>", "activitylogType");
                xmlString.AppendFormat("<{0}>{1}</{0}>", "name", logType.Name);
                xmlString.AppendFormat("<{0}>{1}</{0}>", "systemKeyword", logType.SystemKeyword);
                xmlString.AppendFormat("<{0}>{1}</{0}>", "enabled", logType.Enabled);
                xmlString.AppendFormat("</{0}>", "activitylogType");
            }

            xmlString.AppendFormat("</{0}>", "activitylogTypes");

            return xmlString.ToString();
        }

        #endregion
    }
}
