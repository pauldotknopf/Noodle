using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Noodle.Engine;

namespace Noodle.Tests
{
    [TestFixture]
    public abstract class TestBase
    {
        protected TinyIoCContainer _container;

        [SetUp]
        public virtual void SetUp()
        {
            _container = BuildTestContainer();
            ConfigureContainer();
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

        public virtual TinyIoCContainer BuildTestContainer()
        {
            var container = new TinyIoCContainer();
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

        /// <summary>
        /// Override this method to modify the container directly after creation
        /// </summary>
        public virtual void ConfigureContainer()
        {
            
        }
    }
}
