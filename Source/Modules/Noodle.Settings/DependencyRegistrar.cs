using MongoDB.Driver;
using Ninject;
using Noodle.Configuration;
using Noodle.Data;
using Noodle.Engine;
using Noodle.MongoDB;

namespace Noodle.Settings
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(IKernel kernel, ITypeFinder typeFinder, ConfigurationManagerWrapper configuration)
        {
            kernel.Bind<ISettingService>().To<SettingService>().InSingletonScope();
            kernel.Bind(typeof (IConfigurationProvider<>)).To(typeof (ConfigurationProvider<>));
            kernel.Bind<MongoDatabase>()
                  .ToMethod(context => context.Kernel.Resolve<IMongoService>().GetDatabase("Settings"))
                  .InSingletonScope()
                  .Named("Settings");
            kernel.Bind<MongoCollection<Setting>>().ToMethod(context => context.Kernel.Get<MongoDatabase>("Settings").GetCollection<Setting>("Settings")).InSingletonScope();
            kernel.MakeKernelResolveSettings();
        }

        public int Importance
        {
            get { return 0; }
        }
    }
}
