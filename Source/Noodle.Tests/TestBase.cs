using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Noodle.Caching;
using Noodle.Configuration;
using Noodle.Engine;
using SimpleInjector;

namespace Noodle.Tests
{
    [TestFixture]
    public abstract class TestBase
    {
        protected Container _container;

        [SetUp]
        public virtual void SetUp()
        {
            _container = BuildTestContainer();
            // placing the kernel here allows it to be used when doing EngineContext.Current
            Singleton<Container>.Instance = _container;
        }

        [TearDown]
        public virtual void TearDown()
        {
        }

        [TestFixtureSetUp]
        public virtual void FixtureSetUp()
        {
            
        }

        [TestFixtureTearDown]
        public virtual void FixtureTearDown()
        {
            
        }

        public virtual Container BuildTestContainer()
        {
            var container = new Container();
            CoreDependencyRegistrar.Register(container);
            foreach (var dependencyRegistrar in GetDependencyRegistrars().OrderBy(x => x.Importance))
            {
                dependencyRegistrar.Register(container);
            }
            return container;
        }

        /// <summary>
        /// Override to include/exclude dependency registrars
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<IDependencyRegistrar> GetDependencyRegistrars()
        {
            return Enumerable.Empty<IDependencyRegistrar>();
        }
    }
}
