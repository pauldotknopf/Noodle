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
        [SetUp]
        public virtual void SetUp()
        {
        }

        [SetUp]
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

            kernel.Rebind<ICacheManager>().To<NullCache>();
            return kernel;
        }
    }
}
