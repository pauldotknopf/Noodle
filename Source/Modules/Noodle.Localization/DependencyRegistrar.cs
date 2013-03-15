using MongoDB.Driver;
using Noodle.Engine;
using Noodle.Localization.Services;
using Noodle.MongoDB;

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
        public void Register(TinyIoCContainer container)
        {
            container.Register<ILanguageService, LanguageService>();
            container.Register<ILocalizationService, LocalizationService>();
            container.Register<ILanguageInstaller, LanguageInstaller>();
            container.Register<ILocalizedEntityService, LocalizedEntityService>();
            container.Register((context, p) => GetLocalizationDatabase(context).GetCollection<Language>("Languages"));
            container.Register((context, p) => GetLocalizationDatabase(context).GetCollection<LocaleStringResource>("LocaleStringResources"));
            container.Register((context, p) => GetLocalizationDatabase(context).GetCollection<LocalizedProperty>("LocalizedProperties"));
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

        public static MongoDatabase GetLocalizationDatabase(TinyIoCContainer container)
        {
            return container.Resolve<IMongoService>().GetDatabase("Localization");
        }
    }
}



