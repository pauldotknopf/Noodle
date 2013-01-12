using Noodle.Configuration;
using Noodle.Engine;

namespace Noodle.Localization
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(Ninject.IKernel kernel, ITypeFinder typeFinder, ConfigurationManagerWrapper configuration)
        {
            kernel.Bind<ILanguageService>().To<LanguageService>().InRequestScope();
            kernel.Bind<ILocalizationService>().To<LocalizationService>().InRequestScope();
            kernel.Bind<ILocalizedEntityService>().To<LocalizedEntityService>().InRequestScope();
        }

        public int Importance
        {
            get { return 0; }
        }
    }
}
