using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Noodle.Caching;

namespace Noodle.Tests
{
    [TestFixture]
    public class InMemoryCacheTests : Noodle.Tests.TestBase
    {
        private ICacheManager _cacheManager;

        public override void SetUp()
        {
            base.SetUp();
            _cacheManager = new InMemoryCache();
        }

        [Test]
        public void Can_set_cache()
        {
            _cacheManager.IsSet("test").ShouldBeFalse();
            _cacheManager.Set("test", "value", 10);
            _cacheManager.IsSet("test").ShouldBeTrue();
        }

        [Test]
        public void Can_set_cache_forever()
        {
            var prev = CommonHelper.CurrentTime;
            try
            {
                var time = DateTime.Now;
                CommonHelper.CurrentTime = () => time;
                _cacheManager.IsSet("test").ShouldBeFalse();
                _cacheManager.Set("test", "value");
                time = DateTime.MaxValue;
                _cacheManager.IsSet("test").ShouldBeTrue();
            }
            finally
            {
                CommonHelper.CurrentTime = prev;
            }
        }

        [Test]
        public void Is_set_invalidates_expired_items()
        {
            var prev = CommonHelper.CurrentTime;
            try
            {
                var time = DateTime.Now;
                CommonHelper.CurrentTime = () => time;
                _cacheManager.Set("test", "value", 100);
                _cacheManager.IsSet("test").ShouldBeTrue();
                time = DateTime.MaxValue;
                _cacheManager.IsSet("test").ShouldBeFalse();
            }
            finally
            {
                CommonHelper.CurrentTime = prev;
            }
        }

        [Test]
        public void Get_returns_null_with_no_entry()
        {
            _cacheManager.Get<string>("test").ShouldBeNull();
            _cacheManager.Set("test", "value");
            _cacheManager.Get<string>("test").ShouldEqual("value");
        }

        [Test]
        public void Get_returns_null_when_cache_expired()
        {
            var prev = CommonHelper.CurrentTime;
            try
            {
                var time = DateTime.Now;
                CommonHelper.CurrentTime = () => time;
                _cacheManager.Get<string>("test").ShouldBeNull();
                _cacheManager.Set("test", "value", 10);
                _cacheManager.Get<string>("test").ShouldEqual("value");
                time = DateTime.MaxValue;
                _cacheManager.Get<string>("test").ShouldBeNull();
                _cacheManager.IsSet("test").ShouldBeFalse();
            }
            finally
            {
                CommonHelper.CurrentTime = prev;
            }
        }
    }
}
