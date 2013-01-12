using MongoDB.Driver;
using Ninject;
using Noodle.Configuration;
using Noodle.Engine;
using Noodle.Localization.Services;
using Noodle.MongoDB;
using Noodle.Settings;

namespace Noodle.Localization
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(Ninject.IKernel kernel, ITypeFinder typeFinder, ConfigurationManagerWrapper configuration)
        {
            kernel.Bind<ILanguageService>().To<LanguageService>().InRequestScope();
            kernel.Bind<ILocalizationService>().To<LocalizationService>().InRequestScope();
            kernel.Bind<ILocalizedEntityService>().To<LocalizedEntityService>().InRequestScope();
            kernel.Bind<MongoDatabase>()
                  .ToMethod(context => context.Kernel.Resolve<IMongoService>().GetDatabase("Localization"))
                  .InSingletonScope()
                  .Named("Localization");
            kernel.Bind<MongoCollection<Language>>().ToMethod(context => context.Kernel.Get<MongoDatabase>("Localization").GetCollection<Language>("Languages")).InSingletonScope();
            kernel.Bind<MongoCollection<LocaleStringResource>>().ToMethod(context => context.Kernel.Get<MongoDatabase>("Localization").GetCollection<LocaleStringResource>("LocaleStringResources")).InSingletonScope();
            kernel.Bind<MongoCollection<LocalizedProperty>>().ToMethod(context => context.Kernel.Get<MongoDatabase>("Localization").GetCollection<LocalizedProperty>("LocalizedProperties")).InSingletonScope();
            kernel.MakeKernelResolveSettings();
        }

        public int Importance
        {
            get { return 0; }
        }
    }
}
