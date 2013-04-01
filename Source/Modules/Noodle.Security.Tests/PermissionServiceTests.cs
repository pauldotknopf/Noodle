using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using NUnit.Framework;
using Noodle.Collections;
using Noodle.Security.Permissions;
using Noodle.Tests;

namespace Noodle.Security.Tests
{
    [TestFixture]
    public class PermissionServiceTests : SecurityTestBase
    {
        //[TestMethod]
        //public void Can_authorize_a_user_against_a_permission()
        //{
        //    // setup
        //    Can_install_new_permissions();
        //    var testUser = GetAndCreateUser(role: Guest);

        //    // act
        //    var isAuthorized = PermissionService.Authorize(testUser.Id, "Permission1System");

        //    // assert
        //    isAuthorized.ShouldBeTrue();
        //}

        //[TestMethod]
        //public void Can_deny_a_user_against_a_permission()
        //{
        //    // setup
        //    Can_install_new_permissions();
        //    var testUser = GetAndCreateUser(role:Guest);

        //    // act
        //    var isAuthorized = PermissionService.Authorize(testUser.Id, "Permission2System");

        //    // assert
        //    isAuthorized.ShouldBeFalse();
        //}

        //[TestMethod]
        //public void Can_get_all_permission_records()
        //{
        //    // setup
        //    Can_install_new_permissions();

        //    // act
        //    var allPermissions = PermissionService.GetAllPermissionRecords();

        //    // assert
        //    allPermissions.SequenceEqual(GetTestPermissionRecords(), GetPermissionRecordComparer()).ShouldBeTrue();
        //}

        [Test]
        public void Can_install_new_permissions()
        {
            // setup
            var testPermissionRecords = GetTestPermissionRecords();
            var testPermissionProvider = new Fakes.FakePermissionProvider(testPermissionRecords, GetTestDefaultPermissionRecords());

            // act
            _permissionService.InstallPermissions(testPermissionProvider);

            // assert
            var databasePermissions = _container.Resolve<MongoCollection<PermissionRecord>>().FindAll().ToList();
            databasePermissions.Count.ShouldEqual(2);
            databasePermissions.SequenceEqual(testPermissionRecords, GetPermissionRecordComparer()).ShouldBeTrue();
        }

        //[TestMethod]
        //public void Can_ignore_an_installation_provider_if_it_has_already_been_installed()
        //{
        //    // setup
        //    Can_install_new_permissions();
        //    var testPermissionRecords = GetTestPermissionRecords();
        //    testPermissionRecords[0].Name = "Permission1Modified";
        //    testPermissionRecords.Add(new PermissionRecord{Name = "NewPermission", Category="NewPermissionCategory", SystemName = "NewPermissionSystem"});
        //    var testPermissionProvider = new Fakes.FakePermissionProvider(testPermissionRecords, GetTestDefaultPermissionRecords());

        //    // act
        //    PermissionService.InstallPermissions(testPermissionProvider);

        //    // assert
        //    var databasePermissions = PermissionRecordRepository.Table.ToList();
        //    databasePermissions.Count.ShouldEqual(2);
        //    databasePermissions[0].Name.ShouldEqual("Permission1");
        //}

        //[TestMethod]
        //public void Can_udpate_database_when_reinstalling_a_provider()
        //{
        //    // setup
        //    Can_install_new_permissions();
        //    var testPermissionRecords = GetTestPermissionRecords();
        //    testPermissionRecords[0].Name = "Permission1Modified";
        //    var testPermissionProvider = new Fakes.FakePermissionProvider(testPermissionRecords, GetTestDefaultPermissionRecords());

        //    // act
        //    PermissionService.InstallPermissions(testPermissionProvider, true);

        //    // assert
        //    var databasePermissions = PermissionRecordRepository.Table.ToList();
        //    databasePermissions.Count.ShouldEqual(2);
        //    databasePermissions[0].Name.ShouldEqual("Permission1Modified");
        //    databasePermissions[1].Name.ShouldEqual("Permission2");
        //}

        //[TestMethod]
        //public void Can_add_permission_records_if_they_dont_exist_when_installign_a_provider()
        //{
        //    // setup
        //    var testPermissionProvider = new Fakes.FakePermissionProvider(new List<PermissionRecord>(), GetTestDefaultPermissionRecords());

        //    // act
        //    PermissionService.InstallPermissions(testPermissionProvider, true);

        //    // assert
        //    var databasePermissions = PermissionRecordRepository.Table.ToList();
        //    databasePermissions.Count.ShouldEqual(2);
        //    databasePermissions.SequenceEqual(GetTestPermissionRecords(), GetPermissionRecordComparer()).ShouldBeTrue();
        //}

        //[TestMethod]
        //public void Can_add_user_roles_if_installing_a_provider_with_new_user_roles()
        //{
        //    // setup
        //    var testPermissionProvider = new Fakes.FakePermissionProvider(GetTestPermissionRecords(), GetTestDefaultPermissionRecords());

        //    // act
        //    PermissionService.InstallPermissions(testPermissionProvider, true);

        //    // assert
        //    var roles = UserRoleRepository.Table.ToList();
        //    var roleMaps = RolePermissionMapRepository.Table.ToList();
        //    roles.Count.ShouldEqual(2);
        //    roleMaps.Count.ShouldEqual(2);
        //    roleMaps.Any(x => x.UserRoleId == roles[0].Id).ShouldBeTrue();
        //    roleMaps.Any(x => x.UserRoleId == roles[1].Id).ShouldBeTrue();
        //}

        //[TestMethod, Ignore]
        //public void Can_uninstall_a_provider()
        //{
        //    // TODO
        //    //_permissionService.UninstallPermissions()
        //}

        //[TestMethod]
        //public void Can_add_a_permission_to_a_role()
        //{
        //    // setup
        //    Can_install_new_permissions();
        //    PermissionService.AuthorizeRole(Guest, "Permission2System").ShouldBeFalse();

        //    // act
        //    PermissionService.AddPermissionToRole(Guest, "Permission2System");

        //    // assert
        //    PermissionService.AuthorizeRole(Guest, "Permission2System").ShouldBeTrue();
        //}

        //[TestMethod]
        //public void Can_remove_a_permission_from_a_role()
        //{
        //    // setup
        //    Can_install_new_permissions();
        //    PermissionService.AuthorizeRole(Guest, "Permission1System").ShouldBeTrue();

        //    // act
        //    PermissionService.RemovePermissionFromRole(Guest, "Permission1System");

        //    // assert
        //    PermissionService.AuthorizeRole(Guest, "Permission1System").ShouldBeFalse();
        //}

        [Test]
        public void Can_authorize_a_role_for_a_permission()
        {
            // setup
            Can_install_new_permissions();

            // act/assert
            _permissionService.AuthorizeRole(Guest, "Permission1System").ShouldBeTrue();
            _permissionService.AuthorizeRole(Guest, "Permission2System").ShouldBeFalse();
            _permissionService.AuthorizeRole(Admin, "Permission1System").ShouldBeFalse();
            _permissionService.AuthorizeRole(Admin, "Permission2System").ShouldBeTrue();
        }

        #region Helpers

        public static List<PermissionRecord> GetTestPermissionRecords()
        {
            return new List<PermissionRecord>
            {
                new PermissionRecord
                    {Name = "Permission1", Category = "Permission1Category", SystemName = "Permission1System"},
                new PermissionRecord
                    {Name = "Permission2", Category = "Permission2Category", SystemName = "Permission2System"}
            };
        }

        public static List<DefaultPermissionRecord> GetTestDefaultPermissionRecords()
        {
            return new List<DefaultPermissionRecord>
            {
                    new DefaultPermissionRecord
                        {
                            UserRoleSystemName = Guest,
                            PermissionRecords = GetTestPermissionRecords().Take(1).ToList()
                        },
                        new DefaultPermissionRecord
                            {
                                UserRoleSystemName = Admin,
                                PermissionRecords = GetTestPermissionRecords().Skip(1).ToList()
                            }
            };
        } 

        public IEqualityComparer<PermissionRecord> GetPermissionRecordComparer()
        {
            return new DelegateEqualityComparer<PermissionRecord>((x, y) =>
                {
                    if (!x.Name.Equals(y.Name))
                        return false;

                    if (!x.Category.Equals(y.Category))
                        return false;

                    if (!x.SystemName.Equals(y.SystemName))
                        return false;

                    return true;
                },
                (x) => x.Name.GetHashCode() + x.Category.GetHashCode() + x.SystemName.GetHashCode());
        } 

        #endregion

        #region Nested types

        public class TestPermissionProvider : IPermissionProvider
        {
            public IEnumerable<PermissionRecord> GetPermissions()
            {
                return GetTestPermissionRecords();
            }

            public IEnumerable<DefaultPermissionRecord> GetDefaultPermissions()
            {
                return GetTestDefaultPermissionRecords();
            }
        }

        #endregion
    }
}
