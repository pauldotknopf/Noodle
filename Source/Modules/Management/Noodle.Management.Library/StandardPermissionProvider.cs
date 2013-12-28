using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noodle.Security;
using Noodle.Security.Permissions;
using Noodle.Security.Users;

namespace Noodle.Management.Library
{
    public class StandardPermissionProvider : IPermissionProvider
    {
        // standard
        public const string AccessAdminPanel = "AccessAdminPanel";

        // users
        public const string ManageUsers = "ManageUsers";
        public const string ManageUserRoles = "ManageUserRoles";
        public const string ManageAcl = "ManageACL";

        // configuration
        public const string ManageSettings = "ManageSettings";
        public const string ManageLanguages = "ManageLanguages";
        public const string ManageActivityLog = "ManageActivityLog";
        public const string ManageEmailAccounts = "ManageEmailAccounts";
        public const string ManageMessageTemplates = "ManageMessageTemplates";
        public const string ManageQueuedEmails = "ManageQueuedEmails";

        // system
        public const string ManageSystemLog = "ManageSystemLog";

        #region IPermissionProvider

        public virtual IEnumerable<PermissionRecord> GetPermissions()
        {
            return new[] 
            {
                PermissionRecords.AccessAdminPanel,
                PermissionRecords.ManageUsers,
                PermissionRecords.ManageUserRoles,
                PermissionRecords.ManageAcl,
                PermissionRecords.ManageSettings,
                PermissionRecords.ManageLanguages,
                PermissionRecords.ManageSystemLog,
                PermissionRecords.ManageActivityLog,
                PermissionRecords.ManageEmailAccounts,
                PermissionRecords.ManageMessageTemplates,
                PermissionRecords.ManagedQueuedEmails
            };
        }

        public virtual IEnumerable<DefaultPermissionRecord> GetDefaultPermissions()
        {
            return new[] 
            {
                new DefaultPermissionRecord 
                {
                    UserRoleSystemName = SystemUserRoleNames.Administrators,
                    PermissionRecords = new[] 
                    {
                        PermissionRecords.AccessAdminPanel,
                        PermissionRecords.ManageUsers,
                        PermissionRecords.ManageUserRoles,
                        PermissionRecords.ManageAcl,
                        PermissionRecords.ManageSettings,
                        PermissionRecords.ManageLanguages,
                        PermissionRecords.ManageSystemLog,
                        PermissionRecords.ManageActivityLog,
                        PermissionRecords.ManageEmailAccounts,
                        PermissionRecords.ManageMessageTemplates,
                        PermissionRecords.ManagedQueuedEmails
                    }
                },
                new DefaultPermissionRecord 
                {
                    UserRoleSystemName = SystemUserRoleNames.Registered,
                    PermissionRecords = new[] 
                    {
                        PermissionRecords.AccessAdminPanel
                    }
                }
            };
        }

        #endregion

        #region Internal

        internal static class PermissionRecords
        {
            // general
            internal static readonly PermissionRecord AccessAdminPanel =
                PermissionRecordHelper.CreatePermission(StandardPermissionProvider.AccessAdminPanel, "Access admin area", "Standard");

            // users
            internal static readonly PermissionRecord ManageUsers =
                PermissionRecordHelper.CreatePermission(StandardPermissionProvider.ManageUsers, "Manage users", "Users");
            internal static readonly PermissionRecord ManageUserRoles =
                PermissionRecordHelper.CreatePermission(StandardPermissionProvider.ManageUserRoles, "Manage user roles", "Users");
            internal static readonly PermissionRecord ManageAcl =
                PermissionRecordHelper.CreatePermission(StandardPermissionProvider.ManageAcl, "Manage ACL", "Users");

            // configuration
            internal static readonly PermissionRecord ManageSettings =
                PermissionRecordHelper.CreatePermission(StandardPermissionProvider.ManageSettings, "Manage settings", "Configuration");
            internal static readonly PermissionRecord ManageLanguages =
                PermissionRecordHelper.CreatePermission(StandardPermissionProvider.ManageLanguages, "Manage languages", "Configuration");
            internal static readonly PermissionRecord ManageActivityLog =
                PermissionRecordHelper.CreatePermission(StandardPermissionProvider.ManageActivityLog, "Manage activity log", "Configuration");
            internal static readonly PermissionRecord ManageEmailAccounts =
                PermissionRecordHelper.CreatePermission(StandardPermissionProvider.ManageEmailAccounts, "Manage email accounts", "Configuration");
            internal static readonly PermissionRecord ManageMessageTemplates =
                PermissionRecordHelper.CreatePermission(StandardPermissionProvider.ManageMessageTemplates, "Manage message templates", "Configuration");
            internal static readonly PermissionRecord ManagedQueuedEmails =
                PermissionRecordHelper.CreatePermission(StandardPermissionProvider.ManageQueuedEmails, "Manage queued emails", "Configuration");

            // system
            internal static readonly PermissionRecord ManageSystemLog =
                PermissionRecordHelper.CreatePermission(StandardPermissionProvider.ManageSystemLog, "Manage system log", "System");
        }

        #endregion
    }
}
