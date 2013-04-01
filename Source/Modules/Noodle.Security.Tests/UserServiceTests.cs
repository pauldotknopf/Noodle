using System;
using System.Collections.Generic;

namespace Noodle.Security.Tests
{
    [TestClass]
    public class UserServiceTests : SecurityTestBase
    {
        private User _testUser;

        [TestMethod]
        public void Can_insert_a_user()
        {
            // setup
            _testUser = GetTestUser();

            // act
            UserRepositoy.Insert(_testUser);

            // assert
            (_testUser.Id > 0).ShouldBeTrue();
            var dbUser = UserService.GetUserById(_testUser.Id);
            GetUserComparer().Equals(_testUser, dbUser).ShouldBeTrue();
        }

        [TestMethod]
        public void Deleting_a_system_user_throws_an_error()
        {
            // setup
            Can_insert_a_user();
            _testUser.IsSystemAccount = true;
            UserRepositoy.Update(_testUser);

            // act/assert
            try
            {
                UserService.DeleteUser(_testUser.Id);
                Assert.Fail("UserRepositoy.Delete didn't throw an error when trying to delete a system account.");
            }catch(Exception ex)
            {
                ex.Message.ShouldEqual("You cannot delete a system account");
            }
        }

        [TestMethod]
        public void Can_get_a_user_by_email()
        {
            // setup
            Can_insert_a_user();

            // act
            var user = UserService.GetUserByEmail("EmAiL0@DoMaIn0.com");

            // assert
            user.ShouldNotBeNull();
            user.Id.ShouldEqual(_testUser.Id);
        }

        [TestMethod]
        public void Getting_a_user_by_email_returns_null_if_none()
        {
            // setup
            Can_insert_a_user();

            // act
            var user = UserService.GetUserByEmail("none@none.com");

            // assert
            user.ShouldBeNull();
        }

        [TestMethod]
        public void Can_get_a_user_by_system_name()
        {
            // setup
            Can_insert_a_user();

            // act
            var user = UserService.GetUserBySystemName("SyStEm0");

            // assert
            user.ShouldNotBeNull();
            user.Id.ShouldEqual(_testUser.Id);
        }

        [TestMethod]
        public void Getting_a_user_by_system_name_returns_null_if_none()
        {
            // setup
            Can_insert_a_user();

            // act
            var user = UserService.GetUserBySystemName("noSystemName");

            // assert
            user.ShouldBeNull();
        }

        [TestMethod]
        public void Can_get_a_user_by_username()
        {
            // setup
            Can_insert_a_user();

            // act
            var user = UserService.GetUserByUsername("UsErNaMe0");

            // assert
            user.ShouldNotBeNull();
            user.Id.ShouldEqual(_testUser.Id);
        }

        [TestMethod]
        public void Getting_a_user_by_username_returns_null_if_none()
        {
            // setup
            Can_insert_a_user();

            // act
            var user = UserService.GetUserByUsername("noUsername");

            // assert
            user.ShouldBeNull();
        }

        [TestMethod]
        public void Can_get_all_user_roles()
        {
            // setup
            UserRoleRepository.Insert(new UserRole { Name = "Role1", SystemName = "Role1", Active = true });
            UserRoleRepository.Insert(new UserRole { Name = "Role2", SystemName = "Role2", Active = false });
            UserRoleRepository.Insert(new UserRole { Name = "Role3", SystemName = "Role3", Active = true });

            // act
            var roles = UserService.GetAllUserRoles();

            // assert
            roles.Count.ShouldEqual(2);
            roles[0].SystemName.ShouldEqual("Role1");
            roles[1].SystemName.ShouldEqual("Role3");
        }

        [TestMethod]
        public void Can_get_all_hidden_user_roles()
        {
            // setup
            Can_get_all_user_roles();

            // act
            var roles = UserService.GetAllUserRoles(true);

            // assert
            roles.Count.ShouldEqual(3);
            roles[0].SystemName.ShouldEqual("Role1");
            roles[1].SystemName.ShouldEqual("Role2");
            roles[2].SystemName.ShouldEqual("Role3");
        }

        [TestMethod]
        public void Can_add_user_to_role()
        {
            // setup
            Can_get_all_user_roles();
            Can_insert_a_user();
            
            // act
            UserService.AddUserToRole(_testUser.Id, "Role1");
            UserService.AddUserToRole(_testUser.Id, "Role2");

            // assert
            var userRoles = UserService.GetUserRolesByUserId(_testUser.Id, true);
            userRoles.Count.ShouldEqual(2);
            userRoles[0].SystemName.ShouldEqual("Role1");
            userRoles[1].SystemName.ShouldEqual("Role2");
        }

        [TestMethod]
        public void Can_remove_user_from_role()
        {
            // setup
            Can_add_user_to_role();

            // act
            UserService.RemoveUserFromRole(_testUser.Id, "Role2");

            // assert
            var userRoles = UserService.GetUserRolesByUserId(_testUser.Id, true);
            userRoles.Count.ShouldEqual(1);
            userRoles[0].SystemName.ShouldEqual("Role1");
        }

        [TestMethod]
        public void Can_get_all_users()
        {
            // setup
            CreateSampleUsers();

            // act
            var allUsers = UserService.GetAllUsers();

            // assert
            allUsers.Count.ShouldEqual(22);
            allUsers.TotalCount.ShouldEqual(22);
            allUsers.TotalPages.ShouldEqual(1);
        }

        [TestMethod]
        public void Can_get_all_users_paged()
        {
            // setup
            CreateSampleUsers();

            // act
            var users = UserService.GetAllUsers(pageIndex: 0, pageSize: 10);

            // assert
            users.Count.ShouldEqual(10);
            users.TotalPages.ShouldEqual(3);
            users.TotalCount.ShouldEqual(22);
            users[0].LastName.Contains("last42").ShouldBeTrue();
            users[9].LastName.Contains("last24").ShouldBeTrue();

            // act
            users = UserService.GetAllUsers(pageIndex: 2, pageSize: 10);

            // assert
            users.Count.ShouldEqual(2);
            users.TotalPages.ShouldEqual(3);
            users.TotalCount.ShouldEqual(22);
            users[0].LastName.Contains("last2").ShouldBeTrue();
            users[1].LastName.Contains("last0").ShouldBeTrue();
        }

        [TestMethod]
        public void Can_get_all_users_by_email()
        {
            // setup
            CreateSampleUsers();

            // act
            var users = UserService.GetAllUsers(email: "eMaIl1");

            // assert
            users.Count.ShouldEqual(5);
        }

        [TestMethod]
        public void Can_get_all_users_by_username()
        {
            // setup
            CreateSampleUsers();

            // act
            var users = UserService.GetAllUsers(username: "username1");

            // assert
            users.Count.ShouldEqual(5);
        }

        [TestMethod]
        public void Can_get_all_users_by_firstname()
        {
            // setup
            CreateSampleUsers();

            // act
            var users = UserService.GetAllUsers(firstName: "first1");

            // assert
            users.Count.ShouldEqual(5);
        }

        [TestMethod]
        public void Can_get_all_users_by_lastname()
        {
            // setup
            CreateSampleUsers();

            // act
            var users = UserService.GetAllUsers(lastName: "last1");

            // assert
            users.Count.ShouldEqual(5);
        }

        [TestMethod]
        public void Can_get_all_users_including_deleted()
        {
            // setup
            CreateSampleUsers();

            // act
            var users = UserService.GetAllUsers(showDeleted:true);

            // assert
            users.Count.ShouldEqual(43);
        }

        [TestMethod]
        public void Can_get_all_users_created_from()
        {
            // setup
            DateTime now = DateTime.UtcNow;
            CommonHelper.CurrentTime = () => now;
            var from = now.Add(TimeSpan.FromDays(10));
            CreateSampleUsers();
            
            // act
            var users = UserService.GetAllUsers(registrationFrom: from);

            // assert
            users.Count.ShouldEqual(19);
            users[0].FirstName.Equals("first42").ShouldBeTrue();
            users[18].FirstName.Equals("first6").ShouldBeTrue();
            users.All(x => x.CreatedOnUtc.Tollerable() >= from.Tollerable()).ShouldBeTrue();
        }

        [TestMethod]
        public void Can_get_all_users_created_to()
        {
            // setup
            DateTime now = DateTime.UtcNow;
            CommonHelper.CurrentTime = () => now;
            var to = now.Add(TimeSpan.FromDays(10));
            CreateSampleUsers();

            // act
            var users = UserService.GetAllUsers(registrationTo: to);

            // assert
            users.Count.ShouldEqual(3);
            users[0].FirstName.Equals("first4").ShouldBeTrue();
            users[2].FirstName.Equals("first0").ShouldBeTrue();
            users.All(x => x.CreatedOnUtc.Tollerable() <= to.Tollerable()).ShouldBeTrue();
        }

        [TestMethod]
        public void Can_get_all_users_within_role()
        {
            // setup
            CreateSampleUsers();
            var role1 = UserRoleRepository.Single(x => x.SystemName == "system1");
            var role2 = UserRoleRepository.Single(x => x.SystemName == "system2");

            // act
            var users = UserService.GetAllUsers(userRoleIds:new []{role1.Id});

            // assert
            users.All(x => UserService.GetUserRolesByUserId(x.Id, true).Any(y => y.SystemName == "system1")).ShouldBeTrue();

            // act
            users = UserService.GetAllUsers(userRoleIds: new[] { role2.Id });

            // assert
            users.All(x => UserService.GetUserRolesByUserId(x.Id, true).Any(y => y.SystemName == "system2")).ShouldBeTrue();
        }

        #region Helpers

        private void CreateSampleUsers()
        {
            var role1 = UserRoleRepository.Insert(GetTestUserRole(1));
            var role2 = UserRoleRepository.Insert(GetTestUserRole(2));

            for (var x = 0; x < 43; x++)
            {
                var user = UserRepositoy.Insert(GetTestUser(x));
                switch(x % 3)
                {
                    case 0:
                        UserService.AddUserToRole(user.Id, role1.SystemName);
                        break;
                    case 1:
                        UserService.AddUserToRole(user.Id, role2.SystemName);
                        break;
                    case 2:
                        break;
                }
            }
        }

        private IEqualityComparer<User> GetUserComparer()
        {
            return new DelegateEqualityComparer<User>((user1, user2) => 
                    user1.Id.Equals(user2.Id) &
                    user1.UserGuid.Equals(user2.UserGuid) &
                    user1.Username.Equals(user2.Username) &
                    user1.Email.Equals(user2.Email) &
                    user1.Password.Equals(user2.Password) &
                    user1.PasswordFormat.Equals(user2.PasswordFormat) &
                    user1.PasswordSalt.Equals(user2.PasswordSalt) &
                    user1.FirstName.Equals(user2.FirstName) &
                    user1.LastName.Equals(user2.LastName) &
                    user1.Active.Equals(user2.Active) &
                    user1.Deleted.Equals(user2.Deleted) &
                    user1.IsSystemAccount.Equals(user2.IsSystemAccount) &
                    user1.SystemName.Equals(user2.SystemName) &
                    user1.LastIpAddress.Equals(user2.LastIpAddress) &
                    user1.CreatedOnUtc.Tollerable().Equals(user2.CreatedOnUtc.Tollerable()) &
                    user1.LastActivityDateUtc.Tollerable().Equals(user2.LastActivityDateUtc.Tollerable()) &
                    user1.LastLoginDateUtc.Tollerable().Equals(user2.LastLoginDateUtc.Tollerable()) &
                    user1.TimeZoneId.Equals(user2.TimeZoneId),
                (user) => user.Id.GetHashCode()
                    + user.UserGuid.GetHashCode()
                    + user.Email.GetHashCode()
                    + user.Password.GetHashCode()
                    + user.PasswordFormat.GetHashCode()
                    + user.PasswordSalt.GetHashCode()
                    + user.FirstName.GetHashCode()
                    + user.LastName.GetHashCode()
                    + user.Active.GetHashCode()
                    + user.Deleted.GetHashCode()
                    + user.IsSystemAccount.GetHashCode()
                    + user.SystemName.GetHashCode()
                    + user.LastIpAddress.GetHashCode()
                    + user.CreatedOnUtc.Tollerable().GetHashCode()
                    + user.LastActivityDateUtc.Tollerable().GetHashCode()
                    + user.LastLoginDateUtc.Tollerable().GetHashCode()
                    + user.TimeZoneId.GetHashCode());
        }

        #endregion

        #region ChangePassword

        //[TestMethod]
        //public void CanChangePassword_ErrorForNoEmail()
        //{
        //}

        //[TestMethod]
        //public void CanChangePassword_ErrorForNoPassword()
        //{
        //}

        //[TestMethod]
        //public void CanChangePassword_ErrorForNoUserForPassword()
        //{
        //}

        //[TestMethod]
        //public void CanChangePassword_ErrorForNullRequest()
        //{
        //}

        //[TestMethod]
        //public void CanChangePassword_OldPasswordDoesntMatch_Encrypted()
        //{
        //}

        //[TestMethod]
        //public void CanChangePassword_OldPasswordDoesntMatch_Hashed()
        //{
        //}

        //[TestMethod]
        //public void CanChangePassword_OldPasswordDoesntMatch_Clear()
        //{
        //}

        //[TestMethod]
        //public void CanChangePassword_Successful_Encrypted()
        //{
        //}

        //[TestMethod]
        //public void CanChangePassword_Successful_Hashed()
        //{
        //}

        //[TestMethod]
        //public void CanChangePassword_Successful_Clear()
        //{
        //}

        #endregion

        #region Database

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            DataTestBaseHelper.DropCreateDatabase<UserServiceTests>();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            DataTestBaseHelper.DropDatabase<UserServiceTests>();
        }

        #endregion
    }
}
