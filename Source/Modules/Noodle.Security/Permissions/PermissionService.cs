using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Noodle.Engine;
using Noodle.Security.Users;

namespace Noodle.Security.Permissions
{
    /// <summary>
    /// Permission service
    /// </summary>
    /// <remarks></remarks>
    public class PermissionService : IPermissionService
    {
        #region Fields

        private readonly MongoCollection<PermissionRecord> _permissionPecordCollection;
        private readonly MongoCollection<UserRole> _userRoleCollection;
        private readonly MongoCollection<RolePermissionMap> _rolePermissionMapCollection;
        private readonly MongoCollection<PermissionInstalled> _permissionsInstalledCollection;
        private readonly MongoCollection<UserUserRoleMap> _userRoleMap;
        private readonly IUserService _userService;
        private readonly ISecurityContext _securityContext;
        private readonly ITypeFinder _typeFinder;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="permissionPecordCollection">The permission pecord collection.</param>
        /// <param name="userRoleCollection">The user role collection.</param>
        /// <param name="rolePermissionMapCollection">The role permission map collection.</param>
        /// <param name="permissionsInstalledCollection">The permissions installed collection.</param>
        /// <param name="userRoleMap">The user role map.</param>
        /// <param name="userService">The user service.</param>
        /// <param name="securityContext">The security context.</param>
        /// <param name="typeFinder">The type finder.</param>
        public PermissionService(MongoCollection<PermissionRecord> permissionPecordCollection,
            MongoCollection<UserRole> userRoleCollection,
            MongoCollection<RolePermissionMap> rolePermissionMapCollection,
            MongoCollection<PermissionInstalled> permissionsInstalledCollection,
            MongoCollection<UserUserRoleMap> userRoleMap,
            IUserService userService,
            ISecurityContext securityContext,
            ITypeFinder typeFinder)
        {
            _permissionPecordCollection = permissionPecordCollection;
            _userRoleCollection = userRoleCollection;
            _rolePermissionMapCollection = rolePermissionMapCollection;
            _permissionsInstalledCollection = permissionsInstalledCollection;
            _userRoleMap = userRoleMap;
            _userService = userService;
            _securityContext = securityContext;
            _typeFinder = typeFinder;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets all permissions
        /// </summary>
        /// <returns>Permissions</returns>
        /// <remarks></remarks>
        public virtual IList<PermissionRecord> GetAllPermissionRecords()
        {
            return _permissionPecordCollection.FindAll().SetSortOrder(SortBy<PermissionRecord>.Ascending(x => x.Name)).ToList();
        }

        /// <summary>
        /// Add a permission to a role
        /// </summary>
        /// <param name="roleSystemName">The role to add the permission to</param>
        /// <param name="permissionRecordSystemName">The permission to add to the role</param>
        public void AddPermissionToRole(string roleSystemName, string permissionRecordSystemName)
        {
            var role = _userRoleCollection.Find(Query<UserRole>.EQ(x => x.SystemName, roleSystemName)).SetFields(Fields<UserRole>.Include(x => x.Id)).FirstOrDefault();

            if(role == null)
                throw new NoodleException("The role " + roleSystemName + " doesn't exist.");

            var permissionRecord = _permissionPecordCollection.Find(Query<PermissionRecord>.EQ(x => x.SystemName, permissionRecordSystemName)).SetFields(Fields<UserRole>.Include(x => x.Id)).FirstOrDefault();
            
            if(permissionRecord == null)
                throw new NoodleException("The permission record " + permissionRecordSystemName + " doesn't exist.");

            var mapping = new RolePermissionMap
            {
                PermissionRecordId = permissionRecord.Id,
                UserRoleId = role.Id
            };

            _rolePermissionMapCollection.Update(Query.And(Query<RolePermissionMap>.EQ(x => x.PermissionRecordId, permissionRecord.Id), Query<RolePermissionMap>.EQ(x => x.UserRoleId, role.Id)),
                Update<RolePermissionMap>.Replace(mapping), 
                UpdateFlags.Upsert);
        }

        /// <summary>
        /// Removes a permission from a role
        /// </summary>
        /// <param name="roleSystemName">The role to remove the permission from</param>
        /// <param name="permissionRecordSystemName">The permission to remove from the role</param>
        public void RemovePermissionFromRole(string roleSystemName, string permissionRecordSystemName)
        {
            var role = _userRoleCollection.Find(Query<UserRole>.EQ(x => x.SystemName, roleSystemName)).SetFields(Fields<UserRole>.Include(x => x.Id)).FirstOrDefault();

            if (role == null)
                throw new NoodleException("The role " + roleSystemName + " doesn't exist.");

            var permissionRecord = _permissionPecordCollection.Find(Query<PermissionRecord>.EQ(x => x.SystemName, permissionRecordSystemName)).SetFields(Fields<UserRole>.Include(x => x.Id)).FirstOrDefault();

            if (permissionRecord == null)
                throw new NoodleException("The permission record " + permissionRecordSystemName + " doesn't exist.");

            _rolePermissionMapCollection.Remove(Query.And(Query<RolePermissionMap>.EQ(x => x.PermissionRecordId, permissionRecord.Id), Query<RolePermissionMap>.EQ(x => x.UserRoleId, role.Id)));
        }

        /// <summary>
        /// Is a role authorized to permission a specific permission
        /// </summary>
        /// <param name="roleSystemName">The role to check for permissions</param>
        /// <param name="permissionRecordSystemName">The permission to authorize against the role</param>
        /// <returns><c>true</c> if [is role authorized] [the specified role system name]; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool AuthorizeRole(string roleSystemName, string permissionRecordSystemName)
        {
            var role = _userRoleCollection.Find(Query<UserRole>.EQ(x => x.SystemName, roleSystemName)).SetFields(Fields<UserRole>.Include(x => x.Id)).FirstOrDefault();

            if (role == null)
                throw new NoodleException("The role " + roleSystemName + " doesn't exist.");

            var permissionRecord = _permissionPecordCollection.Find(Query<PermissionRecord>.EQ(x => x.SystemName, permissionRecordSystemName)).SetFields(Fields<UserRole>.Include(x => x.Id)).FirstOrDefault();

            if (permissionRecord == null)
                throw new NoodleException("The permission record " + permissionRecordSystemName + " doesn't exist.");

            return _rolePermissionMapCollection.Count(Query.And(Query<RolePermissionMap>.EQ(x => x.PermissionRecordId, permissionRecord.Id), Query<RolePermissionMap>.EQ(x => x.UserRoleId, role.Id))) >= 1;
        }

        /// <summary>
        /// Install permissions
        /// </summary>
        /// <param name="permissionProvider">Permission provider</param>
        /// <param name="reInstall">If true and the provider has already been installed, it will re add the permissions with the associated default permissions.
        /// If not, nothing will be done with the permissions.</param>
        /// <remarks></remarks>
        public virtual void InstallPermissions(IPermissionProvider permissionProvider, bool reInstall = false)
        {
            var providerName = permissionProvider.GetType().Name;

            if(reInstall)
                _permissionsInstalledCollection.Remove(Query<PermissionInstalled>.EQ(x => x.Name, providerName));
            else
                if (_permissionsInstalledCollection.Count(Query<PermissionInstalled>.EQ(x => x.Name, providerName)) == 1)
                    return;
            
            _permissionsInstalledCollection.Insert(new PermissionInstalled { Name = providerName });

            foreach(var permission in permissionProvider.GetPermissions())
            {
                var existing = _permissionPecordCollection.FindOne(Query<PermissionRecord>.EQ(x => x.SystemName, permission.SystemName)) ?? new PermissionRecord();

                existing.Category = permission.Category;
                existing.Name = permission.Name;
                existing.SystemName = permission.SystemName;

                if(existing.Id == ObjectId.Empty)
                    _permissionPecordCollection.Insert(existing);
                else
                    _permissionPecordCollection.Update(Query<PermissionRecord>.EQ(x => x.Id, existing.Id), Update<PermissionRecord>.Replace(existing));
            }

            foreach(var defaultPermission in permissionProvider.GetDefaultPermissions())
            {
                var role = _userRoleCollection.FindOne(Query<UserRole>.EQ(x => x.SystemName, defaultPermission.UserRoleSystemName));
                if(role == null)
                {
                    role = new UserRole
                    {
                        Active = true, 
                        IsSystemRole = false, 
                        Name = defaultPermission.UserRoleSystemName, 
                        SystemName = defaultPermission.UserRoleSystemName
                    };
                    _userRoleCollection.Insert(role);
                }
                foreach(var defaultPermissionRecord in defaultPermission.PermissionRecords)
                {
                    var dbPermissionRecord = _permissionPecordCollection.FindOne(Query<PermissionRecord>.EQ(x => x.SystemName, defaultPermissionRecord.SystemName));
                    
                    if(dbPermissionRecord == null)
                        throw new InvalidOperationException("The permission provider " + providerName + " is trying to install a default permission for permission " + defaultPermissionRecord.SystemName + " but it isn't provided via GetDefaultPermissions or another IPermissionProvider");

                    var query = Query.And(Query<RolePermissionMap>.EQ(x => x.PermissionRecordId, dbPermissionRecord.Id), Query<RolePermissionMap>.EQ(x => x.UserRoleId, role.Id));

                    var existing = _rolePermissionMapCollection.FindOne(query);

                    if(existing != null)
                        continue;

                    _rolePermissionMapCollection.Insert(new RolePermissionMap
                    {
                        PermissionRecordId = dbPermissionRecord.Id,
                        UserRoleId = role.Id
                    });
                }
            }
        }

        /// <summary>
        /// Uninstall permissions
        /// </summary>
        /// <param name="permissionProvider">Permission provider</param>
        /// <remarks></remarks>
        public virtual void UninstallPermissions(IPermissionProvider permissionProvider)
        {
            //TODO:Stored procedure
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <returns>true - authorized; otherwise, false</returns>
        /// <remarks></remarks>
        public virtual bool Authorize(string permissionRecordSystemName)
        {
            if (string.IsNullOrEmpty(permissionRecordSystemName))
                return false;

            var currentUser = _securityContext.CurrentUser;

            if (currentUser == null)
                return false;

            return Authorize(currentUser.Id, permissionRecordSystemName);
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="userId">The user id to authorize</param>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <returns>true - authorized; otherwise, false</returns>
        /// <remarks></remarks>
        public virtual bool Authorize(ObjectId userId, string permissionRecordSystemName)
        {
            if (string.IsNullOrEmpty(permissionRecordSystemName))
                return false;

            if (userId == ObjectId.Empty)
                return false;

            var permissionRecord = _permissionPecordCollection.FindOne(Query<PermissionRecord>.EQ(x => x.SystemName, permissionRecordSystemName));
            
            if(permissionRecord == null)
                throw new InvalidOperationException("Permission record name " + permissionRecordSystemName + " doesn't exist.");

            var rolesWithPermission = _rolePermissionMapCollection.Find(Query<RolePermissionMap>.EQ(x => x.PermissionRecordId, permissionRecord.Id))
                .SetFields(Fields<RolePermissionMap>.Include(x => x.UserRoleId))
                .Select(x => x.UserRoleId).ToList();

            if (rolesWithPermission.Count == 0)
                return false; // no one has this permission

            return _userRoleMap.Count(Query.And(Query<UserUserRoleMap>.EQ(x => x.UserId, userId), Query<UserUserRoleMap>.In(x => x.UserRoleId, rolesWithPermission))) > 0;
        }

        /// <summary>
        /// Install all the permission providers found in the app domain
        /// </summary>
        /// <remarks></remarks>
        public void InstallFoundPermissions()
        {
            foreach(var permissionProvider in _typeFinder.Find<IPermissionProvider>())
            {
                IPermissionProvider provider;
                try
                {
                    provider = Activator.CreateInstance(permissionProvider) as IPermissionProvider;
                }catch
                {
                    throw new Exception("Type {0} must have a empty constructer.".F(permissionProvider.FullName));
                }
                
                InstallPermissions(provider);
            }
        }

        #endregion
    }
}
