using MongoDB.Driver;
using Noodle.Engine;
using Noodle.Localization.Services;
using Noodle.MongoDB;
using SimpleInjector;

namespace Noodle.Localization
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(Container container)
        {
            container.RegisterSingle<ILanguageService, LanguageService>(); // TODO: in request scope?
            container.RegisterSingle<ILocalizationService, LocalizationService>(); // TODO: in request scope?
            container.RegisterSingle<ILocalizedEntityService, LocalizedEntityService>(); // TODO: in request scope?
            container.RegisterSingle<ILanguageInstaller, LanguageInstaller>(); // TODO: in request scope?
            container.RegisterSingle(() => GetLocalizationDatabase(container).GetCollection<Language>("Languages"));
            container.RegisterSingle(() => GetLocalizationDatabase(container).GetCollection<LocaleStringResource>("LocaleStringResources"));
            container.RegisterSingle(() => GetLocalizationDatabase(container).GetCollection<LocalizedProperty>("LocalizedProperties"));
        }

        public int Importance
        {
            get { return 0; }
        }

        public static MongoDatabase GetLocalizationDatabase(Container container)
        {
            return container.GetInstance<IMongoService>().GetDatabase("Localization");
        }
    }
}
