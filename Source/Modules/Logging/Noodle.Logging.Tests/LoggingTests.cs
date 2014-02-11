using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;
using Noodle.Collections;
using Noodle.Tests;

namespace Noodle.Logging.Tests
{
    [TestFixture]
    public class LoggingTests : DataTestBase
    {
        private ILogger _logger;
        private Log _testLog;
        private string _ipAddress;
        private string _referrerUrl;
        private string _currentUrl;

        public override void SetUp()
        {
            base.SetUp();
            _logger = _container.Resolve<ILogger>();
        }

        [Test]
        public void Can_insert_log()
        {
            // setup
            _ipAddress = "ipaddress";
            _referrerUrl = "referrer";
            _currentUrl = "http://domain.com/?query=something";

            // act
            _testLog = _logger.InsertLog(LogLevel.Information, "short", "full", null, "user");

            // assert
            var logs = _logger.GetAllLogs();
            logs.Count.ShouldEqual(1);
            logs[0].LogLevel.ShouldEqual(LogLevel.Information);
            logs[0].ShortMessage.ShouldEqual("short");
            logs[0].FullMessage.ShouldEqual("full");
        }

        [Test]
        public void Can_delete_log()
        {
            // setup
            Can_insert_log();
            var log = _logger.GetAllLogs().Single();

            // act
            _logger.DeleteLog(log.Id);

            // assert
            _logger.GetLogById(log.Id).ShouldBeNull();
            _logger.GetAllLogs().Count.ShouldEqual(0);
        }

        [Test]
        public void Can_get_log_by_id()
        {
            // setup
            Can_insert_log();

            // act
            var log = _logger.GetLogById(_testLog.Id);

            // assert
            log.LogLevel.ShouldEqual(LogLevel.Information);
            log.ShortMessage.ShouldEqual("short");
            log.FullMessage.ShouldEqual("full");
        }

        [Test]
        public void Can_clear_log()
        {
            // setup
            Can_insert_log();

            // act
            _logger.ClearLog();

            // assert
            _container.Resolve<MongoCollection<Log>>().Count().ShouldEqual(0);
        }

        [Test]
        public void Can_get_all_logs()
        {
            // setup
            CreateSampleLogs();

            // act
            var allLogs = _logger.GetAllLogs();

            // assert
            allLogs.Count.ShouldEqual(112);
            allLogs.TotalCount.ShouldEqual(112);
            allLogs.TotalPages.ShouldEqual(1);
            // order by newest first
            allLogs.SequenceEqual(allLogs.OrderByDescending(x => x.CreatedOnUtc).ToList(), GetLogEqualityComparer()).ShouldBeTrue();
        }

        [Test]
        public void Can_get_all_logs_paged()
        {
            // setup
            CreateSampleLogs();

            // act
            var logs = _logger.GetAllLogs(pageIndex: 0, pageSize: 10);

            // assert
            logs.Count.ShouldEqual(10);
            logs.TotalPages.ShouldEqual(12);
            logs.TotalCount.ShouldEqual(112);
            logs[0].FullMessage.ShouldEqual("full0");
            logs[9].FullMessage.ShouldEqual("full9");

            // act
            logs = _logger.GetAllLogs(pageIndex: 11, pageSize: 10);

            // assert
            logs.Count.ShouldEqual(2);
            logs.TotalPages.ShouldEqual(12);
            logs.TotalCount.ShouldEqual(112);
            logs[0].FullMessage.ShouldEqual("full110");
            logs[1].FullMessage.ShouldEqual("full111");
        }

        [Test]
        public void Can_get_all_logs_starting_from()
        {
            var prev = CommonHelper.CurrentTime;
            try
            {
                // setup
                var time = DateTime.UtcNow;
                CommonHelper.CurrentTime = () => time;
                CreateSampleLogs();
                var from = time.Subtract(TimeSpan.FromDays(50));

                // act
                var logs = _logger.GetAllLogs(fromUtc: from);

                // assert
                logs.Count.ShouldEqual(50);
                logs.All(x => x.CreatedOnUtc.Tollerable() >= from.Tollerable()).ShouldBeTrue();
            }finally
            {
                CommonHelper.CurrentTime = prev;
            }
        }

        [Test]
        public void Can_get_all_logs_ending_at()
        {
            var prev = CommonHelper.CurrentTime;

            try
            {
                // setup
                var time = DateTime.UtcNow;
                CommonHelper.CurrentTime = () => time;
                CreateSampleLogs();
                var to = time.Subtract(TimeSpan.FromDays(30));

                // act
                var logs = _logger.GetAllLogs(toUtc: to);

                // assert
                logs.Count.ShouldEqual(83);
                logs.All(x => x.CreatedOnUtc.Tollerable() <= to.Tollerable()).ShouldBeTrue();
            }finally
            {
                CommonHelper.CurrentTime = prev;
            }
        }

        [Test]
        public void Can_get_all_logs_by_message()
        {
            // setup
            CreateSampleLogs();

            // act
            var logs = _logger.GetAllLogs(message: "full99");

            // assert
            logs.Count.ShouldEqual(1);
            logs[0].FullMessage.ShouldEqual("full99");

            // act
            logs = _logger.GetAllLogs(message: "short99");
            logs.Count.ShouldEqual(1);
            logs[0].ShortMessage.ShouldEqual("short99");
        }

        [Test]
        public void Can_get_all_logs_by_log_level()
        {
            // setup
            CreateSampleLogs();

            // act
            var logs = _logger.GetAllLogs(logLevel: LogLevel.Fatal);

            // assert
            logs.Count.ShouldEqual(22);
            logs.All(x => x.LogLevel == LogLevel.Fatal).ShouldBeTrue();
        }

        public override IEnumerable<Engine.IDependencyRegistrar> GetDependencyRegistrars()
        {
            var registrars = base.GetDependencyRegistrars().ToList();
            registrars.Add(new MongoDB.DependencyRegistrar());
            registrars.Add(new DependencyRegistrar());
            return registrars;
        }

        #region Helpers

        private void CreateSampleLogs()
        {
            var prev = CommonHelper.CurrentTime;
            try
            {
                var time = CommonHelper.CurrentTime();

                for (var x = 0; x < 112; x++)
                {
                    var logTime = time.Subtract(TimeSpan.FromDays(x + 1));
                    CommonHelper.CurrentTime = () => logTime;

                    _ipAddress = "ip" + x;
                    _referrerUrl = "referrer" + x;
                    _currentUrl = "http://www.domain{0}.com".F(x);
                    var logLevel = int.Parse(((x % 5) + 1).ToString(CultureInfo.InvariantCulture) + "0");
                    _logger.InsertLog((LogLevel)logLevel, "short" + x, "full" + x, null, "user" + x);
                }
            }finally
            {
                CommonHelper.CurrentTime = prev;
            }
        }

        public IEqualityComparer<Log> GetLogEqualityComparer()
        {
            Func<Log, int> hash = (log) => log.CreatedOnUtc.GetHashCode()
                                           + log.FullMessage.GetHashCode()
                                           + log.LogLevel.GetHashCode()
                                           + log.ShortMessage.GetHashCode();

            return new DelegateEqualityComparer<Log>((log1, log2) => hash(log1).Equals(hash(log2)), hash);
        }

        #endregion
    }
}
