using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ninject;
using Noodle.Caching;
using Noodle.Configuration;
using Noodle.Engine;

namespace Noodle.Tests
{
    [TestFixture]
    public abstract class TestBase
    {
        protected IKernel _kernel;

        [SetUp]
        public virtual void SetUp()
        {
        }

        [TearDown]
        public virtual void TearDown()
        {
        }

        [TestFixtureSetUp]
        public virtual void FixtureSetUp()
        {
            _kernel = BuildTestKernel();
        }

        [TestFixtureTearDown]
        public virtual void FixtureTearDown()
        {
            if(!_kernel.IsDisposed)
                _kernel.Dispose();
        }

        public virtual IKernel BuildTestKernel()
        {
            var kernel = new StandardKernel();
            CoreDependencyRegistrar.Register(kernel);
            var configuration = kernel.Get<ConfigurationManagerWrapper>();
            var typeFinder = kernel.Get<ITypeFinder>();
            foreach (var dependencyRegistrar in GetDependencyRegistrars().OrderBy(x => x.Importance))
            {
                dependencyRegistrar.Register(kernel, typeFinder, configuration);
            }
            kernel.Rebind<ICacheManager>().To<NullCache>();
            return kernel;
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
