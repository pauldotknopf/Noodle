using MongoDB.Driver;
using Noodle.Engine;
using Noodle.Localization.Services;
using Noodle.MongoDB;
using SimpleInjector;

namespace Noodle.Localization
{
    /// <summary>
    /// Register dependencies for the language service
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        /// Register your services with the container. You are given a type finder to help you find anything you need.
        /// </summary>
        /// <param name="container"></param>
        public void Register(Container container)
        {
            container.RegisterSingle<ILanguageService, LanguageService>();
            container.RegisterSingle<ILocalizationService, LocalizationService>();
            container.RegisterSingle<ILanguageInstaller, LanguageInstaller>();
            container.RegisterSingle<ILocalizedEntityService, LocalizedEntityService>();
            container.RegisterSingle(() => GetLocalizationDatabase(container).GetCollection<Language>("Languages"));
            container.RegisterSingle(() => GetLocalizationDatabase(container).GetCollection<LocaleStringResource>("LocaleStringResources"));
            container.RegisterSingle(() => GetLocalizationDatabase(container).GetCollection<LocalizedProperty>("LocalizedProperties"));
        }

        /// <summary>
        /// The lower numbers will be registered first. Higher numbers the latest.
        /// If you are registering fakes, give a high integer (int.Max ?) so that that it will be registered last,
        /// and the container will use it instead of the previously registered services.
        /// </summary>
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



