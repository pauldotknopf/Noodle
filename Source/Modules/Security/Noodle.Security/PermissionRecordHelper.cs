using Noodle.Security.Permissions;

namespace Noodle.Security
{
    public static class PermissionRecordHelper
    {
        public static PermissionRecord CreatePermission(string systemName, string friendlyName, string category)
        {
            return new PermissionRecord {SystemName = systemName, Name = friendlyName, Category = category};
        }
    }
}
