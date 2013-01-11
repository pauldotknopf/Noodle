using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Noodle.Caching;
using Noodle.Configuration;
using Noodle.Engine;

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

        public virtual IKernel GetTestKernel(params IDependencyRegistrar[] dependencyRegistrars)
        {
            var kernel = new StandardKernel();
            CoreDependencyRegistrar.Register(kernel);
            var configuration = kernel.Get<ConfigurationManagerWrapper>();
            var typeFinder = kernel.Get<ITypeFinder>();

            foreach (var dependencyRegistrar in dependencyRegistrars.OrderBy(x => x.Importance))
            {
                dependencyRegistrar.Register(kernel, typeFinder, configuration);
            }

            kernel.Bind<ICacheManager>().To<NullCache>();
            return kernel;
        }
    }
}
