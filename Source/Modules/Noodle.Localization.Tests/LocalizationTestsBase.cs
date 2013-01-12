using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Noodle.Localization.Tests
{
    public class LocalizationTestsBase : DataTestBase
    {
        #region Fields

        protected IKernel Kernel;
        protected IRepository<LocaleStringResource> LocalStringResourceRepository;
        protected IRepository<LocalizedProperty> LocalizedPropertyRepostiory;
        protected IRepository<Language> LanguageRepository;
        protected ILanguageService LanguageService;
        protected ILocalizationService LocalizationService;
        protected ILocalizedEntityService LocalizedEntityService;

        #endregion

        public override List<AbstraceEmbeddedSchemaProvider> GetSchemaProviders()
        {
            return new List<AbstraceEmbeddedSchemaProvider>
                       {
                           new Data.LocalizationEmbeddedSchema()
                       };
        }

        public override IKernel GetTestKernel(params Engine.IDependencyRegistrar[] dependencyRegistrars)
        {
            var registrars = dependencyRegistrars.ToList();
            registrars.Insert(0, new DependencyRegistrar());
            registrars.Insert(0, new Settings.DependencyRegistrar());
            var kernel = base.GetTestKernel(registrars.ToArray());
            return kernel;
        }

        #region Helpers

        public IEqualityComparer<Language> GetLanguageComparer()
        {
            Func<Language, int> hash = (language) => language.DisplayOrder.GetHashCode()
                + language.FlagImageFileName.GetHashCode()
                + language.LanguageCulture.GetHashCode()
                + language.Name.GetHashCode()
                + language.Published.GetHashCode()
                + language.Rtl.GetHashCode()
                + language.UniqueSeoCode.GetHashCode();

            return new DelegateEqualityComparer<Language>((language1, language2) => hash(language1).Equals(hash(language2)), hash);
        }

        public Language CreateLanguage(int index = 1)
        {
            return new Language
                       {
                           DisplayOrder = index,
                           FlagImageFileName = "flag" + index,
                           LanguageCulture = "culture" + index,
                           Name = "name" + index,
                           Published = (index % 2) == 0,
                           Rtl = (index % 2) != 0,
                           UniqueSeoCode = index.ToString(CultureInfo.InvariantCulture)
                       };
        }

        public LocaleStringResource CreateResource(int index, int languageId)
        {
            return new LocaleStringResource
                {
                    ResourceName = "name{0}".F(index),
                    ResourceValue = "value{0}".F(index),
                    LanguageId = languageId
                };
        }

        public void CreateSampleResources(int languageId)
        {
            for(var x = 1; x <= 23; x++)
            {
                LocalStringResourceRepository.Insert(CreateResource(x, languageId));
            }
        }

        [TestInitialize]
        public void Setup()
        {
            Kernel = GetTestKernel();
            LocalStringResourceRepository = Kernel.Resolve<IRepository<LocaleStringResource>>();
            LocalizedPropertyRepostiory = Kernel.Resolve<IRepository<LocalizedProperty>>();
            LanguageRepository = Kernel.Resolve<IRepository<Language>>();
            LocalStringResourceRepository.DeleteAll();
            LocalizedPropertyRepostiory.DeleteAll();
            LanguageRepository.DeleteAll();
            LanguageService = Kernel.Resolve<ILanguageService>();
            LocalizationService = Kernel.Resolve<ILocalizationService>();
            LocalizedEntityService = Kernel.Resolve<ILocalizedEntityService>();
        }

        #endregion
    }
}
