using System;
using System.Collections.Generic;

namespace Noodle.Security.Tests
{
    [TestClass]
    public class UserActivityServiceTests : SecurityTestBase
    {
        [TestMethod]
        public void Can_add_new_log_types_when_installing_a_provider()
        {
            // setup
            var testLogTypes = GetTestActivityLogTypes();
            var testLogTypeProvider = new Fakes.FakeActivityLogTypeProvider(testLogTypes);

            // act
            this.UserActivityService.InstallActivityLogTypes(testLogTypeProvider);

            // assert
            var databaseLogTypes = ActivityLogTypeRepository.Table.ToList();
            databaseLogTypes.Count.ShouldEqual(2);
            databaseLogTypes.SequenceEqual(testLogTypes, this.GetActivityLogTypeComparer()).ShouldBeTrue();

            // this is so that other tests can use this method to install activity log types. the "Table" is active and executed otherwise (old data)
            ActivityLogTypeRepository = Kernel.Resolve<IRepository<ActivityLogType>>();
        }

        [TestMethod]
        public void Can_ignore_already_installed_providers()
        {
            // setup
            Can_add_new_log_types_when_installing_a_provider();
            var testLogTypes = GetTestActivityLogTypes();
            testLogTypes[0].Name = "LogType1Modified";
            testLogTypes.Add(new ActivityLogType { Name = "LogType3", SystemKeyword = "LogType3System" });
            var testLogTypeProvider = new Fakes.FakeActivityLogTypeProvider(testLogTypes);

            // act
            UserActivityService.InstallActivityLogTypes(testLogTypeProvider);

            // assert
            var databaseLogTypes = ActivityLogTypeRepository.Table.ToList();
            databaseLogTypes.Count.ShouldEqual(2);
            databaseLogTypes[0].Name.ShouldEqual("LogType1");
        }

        [TestMethod]
        public void Can_update_existing_log_types_when_reinstalling_a_provider()
        {
            // setup
            Can_add_new_log_types_when_installing_a_provider();
            var testLogTypes = GetTestActivityLogTypes();
            testLogTypes[0].Name = "LogType1Modified";
            var testLogTypeProvider = new Fakes.FakeActivityLogTypeProvider(testLogTypes);

            // act
            UserActivityService.InstallActivityLogTypes(testLogTypeProvider, true);

            // assert
            var databaseLogTypes = ActivityLogTypeRepository.Table.ToList();
            databaseLogTypes.Count.ShouldEqual(2);
            databaseLogTypes[0].Name.ShouldEqual("LogType1Modified");
            databaseLogTypes[1].Name.ShouldEqual("LogType2");
        }

        [TestMethod, Ignore]
        public void Can_uninstall_a_provider()
        {
            // TODO
        }

        [TestMethod]
        public void Can_insert_a_valid_activity()
        {
            // setup
            Can_add_new_log_types_when_installing_a_provider();
            var testUser = GetAndCreateUser();
            var comment = string.Format("comment for user {0}.", testUser.Id);

            // act
            UserActivityService.InsertActivity("LogType1System", comment, testUser.Id);

            // assert
            var activities = ActivityLogRepository.Table.ToList();
            activities.Count.ShouldEqual(1);
            activities[0].Comment.ShouldEqual(comment);
        }

        [TestMethod]
        public void Can_ignore_log_if_invalid_userid_is_given()
        {
            // setup
            Can_add_new_log_types_when_installing_a_provider();

            // act
            UserActivityService.InsertActivity("LogType1System", string.Empty, 0);

            // assert
            var activities = ActivityLogRepository.Table.ToList();
            activities.Count.ShouldEqual(0);
        }

        [TestMethod]
        public void Can_ignore_log_if_logtype_doesnt_exist()
        {
            // setup
            Can_add_new_log_types_when_installing_a_provider();
            var testUser = GetAndCreateUser();
            var comment = string.Format("comment for user {0}.", testUser.Id);

            // act
            UserActivityService.InsertActivity("LogType1SystemTypo", comment, testUser.Id);

            // assert
            var activities = ActivityLogRepository.Table.ToList();
            activities.Count.ShouldEqual(0);
        }

        [TestMethod]
        public void Can_ignore_log_if_logtype_is_disabled()
        {
            // setup
            Can_add_new_log_types_when_installing_a_provider();
            var testUser = GetAndCreateUser();
            var logType = ActivityLogTypeRepository.Single(x => x.SystemKeyword == "LogType1System");
            logType.Enabled = false;
            ActivityLogTypeRepository.Update(logType);

            // act
            UserActivityService.InsertActivity("LogType1SystemTypo", string.Empty, testUser.Id);

            // assert
            var activities = ActivityLogRepository.Table.ToList();
            activities.Count.ShouldEqual(0);
        }

        [TestMethod]
        public void Can_get_all_activities()
        {
            // setup
            Can_add_new_log_types_when_installing_a_provider();
            CreateSampleActivityLogs();

            // act
            var allActivities = UserActivityService.GetAllActivities();
        
            // assert
            allActivities.Count.ShouldEqual(112);
            allActivities.TotalCount.ShouldEqual(112);
            allActivities.TotalPages.ShouldEqual(1);
        }

        [TestMethod]
        public void Can_get_all_activities_paged()
        {
            // setup
            Can_add_new_log_types_when_installing_a_provider();
            CreateSampleActivityLogs();

            // act
            var activities = UserActivityService.GetAllActivities(pageIndex:0, pageSize:10);

            // assert
            activities.Count.ShouldEqual(10);
            activities.TotalPages.ShouldEqual(12);
            activities.TotalCount.ShouldEqual(112);
            activities[0].Comment.Contains("number 1").ShouldBeTrue();
            activities[9].Comment.Contains("number 10").ShouldBeTrue();

            // act
            activities = UserActivityService.GetAllActivities(pageIndex: 11, pageSize: 10);

            // assert
            activities.Count.ShouldEqual(2);
            activities.TotalPages.ShouldEqual(12);
            activities.TotalCount.ShouldEqual(112);
            activities[0].Comment.Contains("number 111").ShouldBeTrue();
            activities[1].Comment.Contains("number 112").ShouldBeTrue();
        }

        [TestMethod]
        public void Can_get_all_activities_by_email()
        {
            // setup
            Can_add_new_log_types_when_installing_a_provider();
            CreateSampleActivityLogs();

            // act
            var activities = UserActivityService.GetAllActivities(email: "email1@domain1.com");

            // assert
            activities.Count.ShouldEqual(38);
            activities.Select(x => x.UserId).Distinct().Count().ShouldEqual(1);

            // act
            activities = UserActivityService.GetAllActivities(email: "email2@domain2.com");

            // assert
            activities.Count.ShouldEqual(74);
            activities.Select(x => x.UserId).Distinct().Count().ShouldEqual(1);
        }

        [TestMethod]
        public void Can_get_all_activities_by_username()
        {
            // setup
            Can_add_new_log_types_when_installing_a_provider();
            CreateSampleActivityLogs();

            // act
            var activities = UserActivityService.GetAllActivities(username: "username1");

            // assert
            activities.Count.ShouldEqual(38);
            activities.Select(x => x.UserId).Distinct().Count().ShouldEqual(1);

            // act
            activities = UserActivityService.GetAllActivities(username: "username2");

            // assert
            activities.Count.ShouldEqual(74);
            activities.Select(x => x.UserId).Distinct().Count().ShouldEqual(1);
        }

        [TestMethod]
        public void Can_get_all_activities_from_start_date()
        {
            // setup
            var time = DateTime.UtcNow;
            CommonHelper.CurrentTime = () => time;
            Can_add_new_log_types_when_installing_a_provider();
            CreateSampleActivityLogs();
            var start = time.Subtract(TimeSpan.FromDays(50));

            // act
            var activities = UserActivityService.GetAllActivities(createdOnFrom: start);

            // assert
            activities.Count.ShouldEqual(51);
            foreach (var activity in activities.Where(activity => activity.CreatedOnUtc.Tollerable() < start.Tollerable()))
            {
                Assert.Fail("{0} was before {1}", activity.CreatedOnUtc, start);
            }
        }

        [TestMethod]
        public void Can_get_all_acitivities_to_end_date()
        {
            // setup
            var time = DateTime.UtcNow;
            CommonHelper.CurrentTime = () => time;
            Can_add_new_log_types_when_installing_a_provider();
            CreateSampleActivityLogs();
            var end = time.Subtract(TimeSpan.FromDays(50));

            // act
            var activities = UserActivityService.GetAllActivities(createdOnTo: end);

            // assert
            activities.Count.ShouldEqual(62);
            foreach (var activity in activities.Where(activity => activity.CreatedOnUtc.Tollerable() > end.Tollerable()))
            {
                Assert.Fail("{0} was after {1}", activity.CreatedOnUtc, end);
            }
        }

        [TestMethod]
        public void Can_clear_all_activities()
        {
            // setup
            Can_add_new_log_types_when_installing_a_provider();
            CreateSampleActivityLogs();

            // act
            UserActivityService.ClearAllActivities();

            // assert
            ActivityLogRepository.Table.Count().ShouldEqual(0);
        }

        [TestMethod]
        public void Can_get_all_activity_types()
        {
            // setup
            Can_add_new_log_types_when_installing_a_provider();

            // act
            var logTypes = UserActivityService.GetAllActivityTypes();

            // assert
            logTypes.SequenceEqual(GetTestActivityLogTypes(), GetActivityLogTypeComparer()).ShouldBeTrue();
        }

        [TestMethod]
        public void Can_delete_an_activity_log_item()
        {
            // setup
            Can_add_new_log_types_when_installing_a_provider();
            var testUser = GetAndCreateUser();
            UserActivityService.GetAllActivities().Count.ShouldEqual(0);
            UserActivityService.InsertActivity("LogType1System", string.Empty, testUser.Id);
            var activityId = UserActivityService.GetAllActivities().Single().Id;

            // act
            UserActivityService.DeleteActivity(activityId);

            // assert
            UserActivityService.GetAllActivities().Count.ShouldEqual(0);
        }

        [TestMethod]
        public void Can_disable_activity_log_type()
        {
            // setup
            Can_enabled_activity_log_type();

            // act
            UserActivityService.EnabledActivityLogType("LogType1System");

            // assert
            UserActivityService.GetAllActivityTypes().Single(x => x.SystemKeyword == "LogType1System").Enabled.ShouldBeTrue();
        }

        [TestMethod]
        public void Can_enabled_activity_log_type()
        {
            // setup
            Can_add_new_log_types_when_installing_a_provider();

            // act
            UserActivityService.DisableActivityLogType("LogType1System");

            // assert
            UserActivityService.GetAllActivityTypes().Single(x => x.SystemKeyword == "LogType1System").Enabled.ShouldBeFalse();
        }

        #region Helpers

        private void CreateSampleActivityLogs()
        {
            var testUser1 = GetAndCreateUser(1);
            var testUser2 = GetAndCreateUser(2);

            var time = CommonHelper.CurrentTime();

            for(var x = 0;x < 112;x++)
            {
                var userId = x%3 == 0 ? testUser1.Id : testUser2.Id;
                var type = x % 2 == 0 ? "LogType1System" : "LogType2System";
                CommonHelper.CurrentTime = () => time.Subtract(TimeSpan.FromDays((double)x));
                UserActivityService.InsertActivity(type, "User {0} type {1} number {2}".F(userId, type, x + 1), userId);
            }
        }

        public static List<ActivityLogType> GetTestActivityLogTypes()
        {
            return new List<ActivityLogType>
            {
                new ActivityLogType
                    {Name = "LogType1", SystemKeyword = "LogType1System"},
                new ActivityLogType
                    {Name = "LogType2", SystemKeyword = "LogType2System"}
            };
        }

        public IEqualityComparer<ActivityLogType> GetActivityLogTypeComparer()
        {
            return new DelegateEqualityComparer<ActivityLogType>((x, y) =>
            {
                if (!x.Name.Equals(y.Name))
                    return false;

                if (!x.SystemKeyword.Equals(y.SystemKeyword))
                    return false;

                if (!x.Enabled.Equals(y.Enabled))
                    return false;

                return true;
            },
            (x) => x.Name.GetHashCode() + x.SystemKeyword.GetHashCode() + x.Enabled.GetHashCode());
        }

        #endregion

        #region Database

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            DataTestBaseHelper.DropCreateDatabase<UserActivityServiceTests>();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            DataTestBaseHelper.DropDatabase<UserActivityServiceTests>();
        }

        #endregion
    }
}
