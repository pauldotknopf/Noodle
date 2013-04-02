using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using NUnit.Framework;
using Noodle.Collections;
using Noodle.Security.Activity;
using Noodle.Tests;

namespace Noodle.Security.Tests
{
    [TestFixture]
    public class UserActivityServiceTests : SecurityTestBase
    {
        [Test]
        public void Can_add_new_log_types_when_installing_a_provider()
        {
            // setup
            var testLogTypes = GetTestActivityLogTypes();
            var testLogTypeProvider = new Fakes.FakeActivityLogTypeProvider(testLogTypes);

            // act
            _userActivityService.InstallActivityLogTypes(testLogTypeProvider);

            // assert
            var databaseLogTypes = _container.Resolve<MongoCollection<ActivityLogType>>().FindAll().ToList();
            databaseLogTypes.Count.ShouldEqual(2);
            databaseLogTypes.SequenceEqual(testLogTypes, this.GetActivityLogTypeComparer()).ShouldBeTrue();
        }

        [Test]
        public void Can_ignore_already_installed_providers()
        {
            // setup
            Can_add_new_log_types_when_installing_a_provider();
            var testLogTypes = GetTestActivityLogTypes();
            testLogTypes[0].Name = "LogType1Modified";
            testLogTypes.Add(new ActivityLogType { Name = "LogType3", SystemKeyword = "LogType3System" });
            var testLogTypeProvider = new Fakes.FakeActivityLogTypeProvider(testLogTypes);

            // act
            _userActivityService.InstallActivityLogTypes(testLogTypeProvider);

            // assert
            var databaseLogTypes = _container.Resolve<MongoCollection<ActivityLogType>>().FindAll().ToList();
            databaseLogTypes.Count.ShouldEqual(2);
            databaseLogTypes[0].Name.ShouldEqual("LogType1");
        }

        [Test]
        public void Can_update_existing_log_types_when_reinstalling_a_provider()
        {
            // setup
            Can_add_new_log_types_when_installing_a_provider();
            var testLogTypes = GetTestActivityLogTypes();
            testLogTypes[0].Name = "LogType1Modified";
            var testLogTypeProvider = new Fakes.FakeActivityLogTypeProvider(testLogTypes);

            // act
            _userActivityService.InstallActivityLogTypes(testLogTypeProvider, true);

            // assert
            var databaseLogTypes = _container.Resolve<MongoCollection<ActivityLogType>>().FindAll().ToList();
            databaseLogTypes.Count.ShouldEqual(2);
            databaseLogTypes[0].Name.ShouldEqual("LogType1Modified");
            databaseLogTypes[1].Name.ShouldEqual("LogType2");
        }

        [Test, Ignore]
        public void Can_uninstall_a_provider()
        {
            // TODO
        }

        [Test]
        public void Can_insert_a_valid_activity()
        {
            // setup
            Can_add_new_log_types_when_installing_a_provider();
            var testUser = GetAndCreateUser();
            var comment = string.Format("comment for user {0}.", testUser.Id);

            // act
            _userActivityService.InsertActivity("LogType1System", comment, testUser.Id);

            // assert
            var activities = _container.Resolve<MongoCollection<ActivityLog>>().FindAll().ToList();
            activities.Count.ShouldEqual(1);
            activities[0].Comment.ShouldEqual(comment);
        }

        [Test]
        public void Can_ignore_log_if_invalid_userid_is_given()
        {
            // setup
            Can_add_new_log_types_when_installing_a_provider();

            // act
            _userActivityService.InsertActivity("LogType1System", string.Empty);

            // assert
            var activities = _container.Resolve<MongoCollection<ActivityLog>>().FindAll().ToList();
            activities.Count.ShouldEqual(0);
        }

        [Test]
        public void Can_ignore_log_if_logtype_doesnt_exist()
        {
            // setup
            Can_add_new_log_types_when_installing_a_provider();
            var testUser = GetAndCreateUser();
            var comment = string.Format("comment for user {0}.", testUser.Id);

            // act
            _userActivityService.InsertActivity("LogType1SystemTypo", comment, testUser.Id);

            // assert
            var activities = _container.Resolve<MongoCollection<ActivityLog>>().FindAll().ToList();
            activities.Count.ShouldEqual(0);
        }

        [Test]
        public void Can_ignore_log_if_logtype_is_disabled()
        {
            // setup
            Can_add_new_log_types_when_installing_a_provider();
            var testUser = GetAndCreateUser();
            var logType = _container.Resolve<MongoCollection<ActivityLogType>>().FindAll().ToList().Single(x => x.SystemKeyword == "LogType1System");
            logType.Enabled = false;
            _container.Resolve<MongoCollection<ActivityLogType>>().Update(Query<ActivityLogType>.EQ(x => x.Id, logType.Id), Update<ActivityLogType>.Replace(logType));

            // act
            _userActivityService.InsertActivity("LogType1SystemTypo", string.Empty, testUser.Id);

            // assert
            var activities = _container.Resolve<MongoCollection<ActivityLog>>().FindAll().ToList();
            activities.Count.ShouldEqual(0);
        }

        [Test]
        public void Can_get_all_activities()
        {
            // setup
            Can_add_new_log_types_when_installing_a_provider();
            CreateSampleActivityLogs();

            // act
            var allActivities = _userActivityService.GetAllActivities();
        
            // assert
            allActivities.Count.ShouldEqual(112);
            allActivities.TotalCount.ShouldEqual(112);
            allActivities.TotalPages.ShouldEqual(1);
        }

        [Test]
        public void Can_get_all_activities_paged()
        {
            // setup
            Can_add_new_log_types_when_installing_a_provider();
            CreateSampleActivityLogs();

            // act
            var activities = _userActivityService.GetAllActivities(pageIndex: 0, pageSize: 10);

            // assert
            activities.Count.ShouldEqual(10);
            activities.TotalPages.ShouldEqual(12);
            activities.TotalCount.ShouldEqual(112);
            activities[0].Comment.Contains("number 1").ShouldBeTrue();
            activities[9].Comment.Contains("number 10").ShouldBeTrue();

            // act
            activities = _userActivityService.GetAllActivities(pageIndex: 11, pageSize: 10);

            // assert
            activities.Count.ShouldEqual(2);
            activities.TotalPages.ShouldEqual(12);
            activities.TotalCount.ShouldEqual(112);
            activities[0].Comment.Contains("number 111").ShouldBeTrue();
            activities[1].Comment.Contains("number 112").ShouldBeTrue();
        }

        [Test]
        public void Can_get_all_activities_by_email()
        {
            // setup
            Can_add_new_log_types_when_installing_a_provider();
            CreateSampleActivityLogs();

            // act
            var activities = _userActivityService.GetAllActivities(email: "email1@domain1.com");

            // assert
            activities.Count.ShouldEqual(38);
            activities.Select(x => x.UserId).Distinct().Count().ShouldEqual(1);

            // act
            activities = _userActivityService.GetAllActivities(email: "email2@domain2.com");

            // assert
            activities.Count.ShouldEqual(74);
            activities.Select(x => x.UserId).Distinct().Count().ShouldEqual(1);
        }

        [Test]
        public void Can_get_all_activities_by_username()
        {
            // setup
            Can_add_new_log_types_when_installing_a_provider();
            CreateSampleActivityLogs();

            // act
            var activities = _userActivityService.GetAllActivities(username: "username1");

            // assert
            activities.Count.ShouldEqual(38);
            activities.Select(x => x.UserId).Distinct().Count().ShouldEqual(1);

            // act
            activities = _userActivityService.GetAllActivities(username: "username2");

            // assert
            activities.Count.ShouldEqual(74);
            activities.Select(x => x.UserId).Distinct().Count().ShouldEqual(1);
        }

        [Test]
        public void Can_get_all_activities_from_start_date()
        {
            // setup
            var time = DateTime.UtcNow;
            CommonHelper.CurrentTime = () => time;
            Can_add_new_log_types_when_installing_a_provider();
            CreateSampleActivityLogs();
            var start = time.Subtract(TimeSpan.FromDays(50));

            // act
            var activities = _userActivityService.GetAllActivities(createdOnFromUtc: start);

            // assert
            activities.Count.ShouldEqual(51);
            foreach (var activity in activities.Where(activity => activity.CreatedOnUtc.Tollerable() < start.Tollerable()))
            {
                Assert.Fail("{0} was before {1}", activity.CreatedOnUtc, start);
            }
        }

        [Test]
        public void Can_get_all_acitivities_to_end_date()
        {
            // setup
            var time = DateTime.UtcNow;
            CommonHelper.CurrentTime = () => time;
            Can_add_new_log_types_when_installing_a_provider();
            CreateSampleActivityLogs();
            var end = time.Subtract(TimeSpan.FromDays(50));

            // act
            var activities = _userActivityService.GetAllActivities(createdOnToUtc: end);

            // assert
            activities.Count.ShouldEqual(62);
            foreach (var activity in activities.Where(activity => activity.CreatedOnUtc.Tollerable() > end.Tollerable()))
            {
                Assert.Fail("{0} was after {1}", activity.CreatedOnUtc, end);
            }
        }

        [Test]
        public void Can_clear_all_activities()
        {
            // setup
            Can_add_new_log_types_when_installing_a_provider();
            CreateSampleActivityLogs();

            // act
            _userActivityService.ClearAllActivities();

            // assert
            _container.Resolve<MongoCollection<ActivityLog>>().FindAll().Count().ShouldEqual(0);
        }

        [Test]
        public void Can_get_all_activity_types()
        {
            // setup
            Can_add_new_log_types_when_installing_a_provider();

            // act
            var logTypes = _userActivityService.GetAllActivityTypes();

            // assert
            logTypes.SequenceEqual(GetTestActivityLogTypes(), GetActivityLogTypeComparer()).ShouldBeTrue();
        }

        [Test]
        public void Can_delete_an_activity_log_item()
        {
            // setup
            Can_add_new_log_types_when_installing_a_provider();
            var testUser = GetAndCreateUser();
            _userActivityService.GetAllActivities().Count.ShouldEqual(0);
            _userActivityService.InsertActivity("LogType1System", string.Empty, testUser.Id);
            var activityId = _userActivityService.GetAllActivities().Single().Id;

            // act
            _userActivityService.DeleteActivity(activityId);

            // assert
            _userActivityService.GetAllActivities().Count.ShouldEqual(0);
        }

        [Test]
        public void Can_disable_activity_log_type()
        {
            // setup
            Can_enabled_activity_log_type();

            // act
            _userActivityService.EnabledActivityLogType("LogType1System");

            // assert
            _userActivityService.GetAllActivityTypes().Single(x => x.SystemKeyword == "LogType1System").Enabled.ShouldBeTrue();
        }

        [Test]
        public void Can_enabled_activity_log_type()
        {
            // setup
            Can_add_new_log_types_when_installing_a_provider();

            // act
            _userActivityService.DisableActivityLogType("LogType1System");

            // assert
            _userActivityService.GetAllActivityTypes().Single(x => x.SystemKeyword == "LogType1System").Enabled.ShouldBeFalse();
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
                _userActivityService.InsertActivity(type, "User {0} type {1} number {2}".F(userId, type, x + 1), userId);
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
    }
}
