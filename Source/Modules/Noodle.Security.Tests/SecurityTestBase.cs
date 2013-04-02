using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Noodle.Security.Activity;
using Noodle.Security.Permissions;
using Noodle.Security.Users;

namespace Noodle.Security.Tests
{
    public class SecurityTestBase : Noodle.Tests.DataTestBase
    {
        public const string Guest = "Guest";
        public const string Admin = "Admin";
        protected IPermissionService _permissionService;
        protected IUserService _userService;
        protected IUserActivityService _userActivityService;

        public override void SetUp()
        {
            base.SetUp();

            var securityContext = new Mock<ISecurityContext>();
            securityContext.Setup(x => x.CurrentUser).Returns(() => ContextUser);
            _container.Register(securityContext.Object);

            _permissionService = _container.Resolve<IPermissionService>();
            _userService = _container.Resolve<IUserService>();
            _userActivityService = _container.Resolve<IUserActivityService>();
        }

        public override IEnumerable<Engine.IDependencyRegistrar> GetDependencyRegistrars()
        {
            var result = base.GetDependencyRegistrars().ToList();
            result.Add(new DependencyRegistrar());
            result.Add(new MongoDB.DependencyRegistrar());
            return result;
        }

        public User ContextUser { get; set; }

        public User GetAndCreateUser(int index = 0, string role = "")
        {
            var testUser = GetTestUser(index);
            testUser.Deleted = false;
            _userService.InsertUser(testUser);
            if (!string.IsNullOrEmpty(role))
            {
                _userService.AddUserToRole(testUser.Id, role);
            }
            return testUser;
        }

        public User GetTestUser(int index = 0)
        {
            return new User
            {
                UserGuid = new Guid(index, 0, 0, new byte[8]),
                PasswordFormat = PasswordFormat.Clear,
                Password = "password{0}".F(index),
                PasswordSalt = "salt{0}".F(index),
                Active = (index % 2) == 0,
                Email = "email{0}@domain{0}.com".F(index),
                Username = "username{0}".F(index),
                Deleted = (index % 2) == 1,
                CreatedOnUtc = CommonHelper.CurrentTime().AddDays(index % 2 == 0 ? index + 5 : (index + 5) * -1),
                LastActivityDateUtc = CommonHelper.CurrentTime().AddDays(index % 2 == 0 ? index + 10 : (index + 10) * -1),
                FirstName = "first{0}".F(index),
                LastName = "last{0}".F(index),
                SystemName = "system{0}".F(index),
                TimeZoneId = "time{0}".F(index),
                LastLoginDateUtc = CommonHelper.CurrentTime().AddDays(index % 2 == 0 ? index + 15 : (index + 15) * -1)
            };
        } 

        //public UserRole GetTestUserRole(int index = 1)
        //{
        //    return new UserRole
        //               {
        //                   Active = (index) % 2 == 0,
        //                   IsSystemRole = (index) % 2 == 1,
        //                   Name = "name{0}".F(index),
        //                   SystemName = "system{0}".F(index)
        //               };
        //}

        //public PermissionRecord GetTestPermissionRecord(int index = 1)
        //{
        //    return new PermissionRecord
        //               {
        //                   Category = "category{0}".F(index),
        //                   Name = "name{0}".F(index),
        //                   SystemName = "system{0}".F(index)
        //               };
        //}

        //public ActivityLogType GetTestActivityLogType(int index = 1)
        //{
        //    return new ActivityLogType
        //               {
        //                   Enabled = (index % 2) == 0,
        //                   Name = "name{0}".F(index),
        //                   SystemKeyword = "system{0}".F(index)
        //               };
        //}
    }
}
