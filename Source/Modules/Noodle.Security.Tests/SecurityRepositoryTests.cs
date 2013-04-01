using System;

namespace Noodle.Security.Tests
{
    [TestClass]
    public class SecurityRepositoryTests : SecurityTestBase
    {
        [TestMethod]
        public void Can_insert_update_delete_user()
        {
            Func<User, int> hash = (user) =>
                user.Active.GetHashCode()
                + user.CreatedOnUtc.Tollerable().GetHashCode()
                + user.Deleted.GetHashCode()
                + user.Email.GetHashCode()
                + user.FirstName.GetHashCode()
                + user.Id.GetHashCode()
                + user.IsSystemAccount.GetHashCode()
                + user.LastActivityDateUtc.Tollerable().GetHashCode()
                + user.LastIpAddress.GetHashCode()
                + user.LastLoginDateUtc.Tollerable().GetHashCode()
                + user.LastName.GetHashCode()
                + user.Password.GetHashCode()
                + user.PasswordFormatId.GetHashCode()
                + user.PasswordSalt.GetHashCode()
                + user.SystemName.GetHashCode()
                + user.TimeZoneId.GetHashCode()
                + user.UserGuid.GetHashCode()
                + user.Username.GetHashCode();

            new RepositoryTestHelper<User>(Kernel, hash, GetTestUser, user => user.Deleted)
                .CanInsertUpdateDelete();
        }

        [TestMethod]
        public void Can_insert_update_delete_user_role()
        {
            Func<UserRole, int> hash = (userRole) =>
                userRole.Id.GetHashCode()
                + userRole.Active.GetHashCode()
                + userRole.Id.GetHashCode()
                + userRole.IsSystemRole.GetHashCode()
                + userRole.Name.GetHashCode()
                + userRole.SystemName.GetHashCode();

            new RepositoryTestHelper<UserRole>(Kernel, hash, GetTestUserRole)
                .CanInsertUpdateDelete();
        }

        [TestMethod]
        public void Can_insert_update_delete_user_role_map()
        {
            Func<UserUserRoleMap, int> hash = (userRoleMap) =>
                userRoleMap.Id.GetHashCode()
                + userRoleMap.UserId.GetHashCode()
                + userRoleMap.UserRoleId.GetHashCode();

            var testUser1 = Kernel.Resolve<IRepository<User>>().Insert(GetAndCreateUser(1));
            var testUser2 = Kernel.Resolve<IRepository<User>>().Insert(GetAndCreateUser(2));

            var role1 = Kernel.Resolve<IRepository<UserRole>>().Insert(GetTestUserRole(1));
            var role2 = Kernel.Resolve<IRepository<UserRole>>().Insert(GetTestUserRole(2));

            new RepositoryTestHelper<UserUserRoleMap>(Kernel, 
                hash, 
                (index) => new UserUserRoleMap
                {
                    UserId = (index % 2 == 0) ? testUser1.Id : testUser2.Id,
                    UserRoleId = (index % 2 == 1) ? role1.Id : role2.Id
                })
                .CanInsertUpdateDelete();
        }

        [TestMethod]
        public void Can_insert_update_delete_permission_record()
        {
            Func<PermissionRecord, int> hash = (permissionRecord) => permissionRecord.Category.GetHashCode()
                + permissionRecord.Id.GetHashCode() 
                + permissionRecord.Name.GetHashCode()
                + permissionRecord.SystemName.GetHashCode();

            new RepositoryTestHelper<PermissionRecord>(Kernel,
                hash,
                GetTestPermissionRecord)
                .CanInsertUpdateDelete();
        }

        [TestMethod]
        public void Can_insert_update_delete_role_permission_map()
        {
            Func<RolePermissionMap, int> hash = (rolePermissionMap) => rolePermissionMap.Id.GetHashCode()
                + rolePermissionMap.PermissionRecordId.GetHashCode()
                + rolePermissionMap.UserRoleId.GetHashCode();

            var role1 = Kernel.Resolve<IRepository<UserRole>>().Insert(GetTestUserRole(1));
            var role2 = Kernel.Resolve<IRepository<UserRole>>().Insert(GetTestUserRole(2));

            var permission1 = Kernel.Resolve<IRepository<PermissionRecord>>().Insert(GetTestPermissionRecord(1));
            var permission2 = Kernel.Resolve<IRepository<PermissionRecord>>().Insert(GetTestPermissionRecord(1));

            new RepositoryTestHelper<RolePermissionMap>(Kernel,
                hash,
                (index) => new RolePermissionMap
                {
                    PermissionRecordId = (index % 2 == 0) ? permission1.Id : permission2.Id,
                    UserRoleId = (index % 2 == 1) ? role1.Id : role2.Id
                })
                .CanInsertUpdateDelete();
        }

        [TestMethod]
        public void Can_insert_udate_delete_activtiy_log_type()
        {
            Func<ActivityLogType, int> hash = (activityLogType) => activityLogType.Enabled.GetHashCode()
                + activityLogType.Id.GetHashCode()
                + activityLogType.Name.GetHashCode()
                + activityLogType.SystemKeyword.GetHashCode();

            new RepositoryTestHelper<ActivityLogType>(Kernel,
                hash,
                GetTestActivityLogType)
                .CanInsertUpdateDelete();
        }

        [TestMethod]
        public void Can_insert_update_delete_activity_log()
        {
            Func<ActivityLog, int> hash = (activityLog) => activityLog.ActivityLogTypeId.GetHashCode()
                + activityLog.Comment.GetHashCode() 
                + activityLog.CreatedOnUtc.Tollerable().GetHashCode()
                + activityLog.Id.GetHashCode()
                + activityLog.UserId.GetHashCode();

            var testUser1 = Kernel.Resolve<IRepository<User>>().Insert(GetAndCreateUser(1));
            var testUser2 = Kernel.Resolve<IRepository<User>>().Insert(GetAndCreateUser(2));

            var type1 = Kernel.Resolve<IRepository<ActivityLogType>>().Insert(new ActivityLogType { Name = "type1", SystemKeyword = "system1" });
            var type2 = Kernel.Resolve<IRepository<ActivityLogType>>().Insert(new ActivityLogType { Name = "type2", SystemKeyword = "system2"});

            new RepositoryTestHelper<ActivityLog>(Kernel,
                hash,
                (index) => new ActivityLog
                {
                    ActivityLogTypeId = (index % 2) == 0 ? type1.Id : type2.Id,
                    Comment = "comment{0}".F(index),
                    CreatedOnUtc = CommonHelper.CurrentTime().AddDays(index),
                    UserId = (index % 2) == 1 ? testUser1.Id : testUser2.Id
                })
                .CanInsertUpdateDelete();
        }

        #region Database

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            DataTestBaseHelper.DropCreateDatabase<SecurityRepositoryTests>();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            DataTestBaseHelper.DropDatabase<SecurityRepositoryTests>();
        }

        #endregion
    }
}
