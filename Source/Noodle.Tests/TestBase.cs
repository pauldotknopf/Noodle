using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Noodle.Caching;
using Noodle.Configuration;
using Noodle.Engine;
using Noodle.TinyIoC;

namespace Noodle.Tests
{
    [TestClass]
    public abstract class TestBase
    {
        [TestInitialize]
        public virtual void SetUp()
        {
        }

        [TestCleanup]
        public virtual void TearDown()
        {
        }

        public virtual TinyIoC.TinyIoCContainer GetTestKernel(params IDependencyRegistrar[] dependencyRegistrars)
        {
            var kernel = new TinyIoCContainer();
            CoreDependencyRegistrar.Register(kernel);
            var configuration = kernel.Resolve<ConfigurationManagerWrapper>();
            var typeFinder = kernel.Resolve<ITypeFinder>();

            foreach (var dependencyRegistrar in dependencyRegistrars.OrderBy(x => x.Importance))
            {
                dependencyRegistrar.Register(kernel, typeFinder, configuration);
            }

            kernel.Register<ICacheManager, NullCache>();
            return kernel;
        }
    }
}
