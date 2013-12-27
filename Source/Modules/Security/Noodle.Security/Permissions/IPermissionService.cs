using System.Collections.Generic;
using MongoDB.Bson;

namespace Noodle.Security.Permissions
{
    /// <summary>
    /// Permission service interface
    /// </summary>
    public interface IPermissionService
    {
        /// <summary>
        /// Gets all permissions
        /// </summary>
        /// <returns>Permissions</returns>
        IList<PermissionRecord> GetAllPermissionRecords();

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <returns>true - authorized; otherwise, false</returns>
        bool Authorize(string permissionRecordSystemName);

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="userId">The user id to authorize</param>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <returns>true - authorized; otherwise, false</returns>
        bool Authorize(ObjectId userId, string permissionRecordSystemName);

        /// <summary>
        /// Add a permission to a role
        /// </summary>
        /// <param name="roleSystemName">The role to add the permission to</param>
        /// <param name="permissionRecordSystemName">The permission to add to the role</param>
        void AddPermissionToRole(string roleSystemName, string permissionRecordSystemName);

        /// <summary>
        /// Removes a permission from a role
        /// </summary>
        /// <param name="roleSystemName">The role to remove the permission from</param>
        /// <param name="permissionRecordSystemName">The permission to remove from the role</param>
        void RemovePermissionFromRole(string roleSystemName, string permissionRecordSystemName);

        /// <summary>
        /// Is a role authorized to permission a specific permission
        /// </summary>
        /// <param name="roleSystemName">The role to check for permissions</param>
        /// <param name="permissionRecordSystemName">The permission to authorize against the role</param>
        /// <returns></returns>
        bool AuthorizeRole(string roleSystemName, string permissionRecordSystemName);

        #region Installing/Uninstalling

        /// <summary>
        /// Install permissions
        /// </summary>
        /// <param name="permissionProvider">Permission provider</param>
        /// <param name="reInstall">
        /// If true and the provider has already been installed, it will re add the permissions with the associated default permissions. 
        /// If not, nothing will be done with the permissions.
        /// </param>
        void InstallPermissions(IPermissionProvider permissionProvider, bool reInstall = false);

        /// <summary>
        /// Uninstall permissions
        /// </summary>
        /// <param name="permissionProvider">Permission provider</param>
        void UninstallPermissions(IPermissionProvider permissionProvider);

        /// <summary>
        /// Install all the permission providers found in the app domain
        /// </summary>
        void InstallFoundPermissions();

        #endregion
    }
}
