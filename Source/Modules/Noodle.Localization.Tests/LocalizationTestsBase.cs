using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using Noodle.Collections;
using Noodle.Localization.Services;
using Noodle.Settings;
using Noodle.Tests;

namespace Noodle.Localization.Tests
{
    public abstract class LocalizationTestsBase : DataTestBase
    {
        protected ILanguageService _languageService;
        protected ILocalizationService _localizationService;
        protected ILocalizedEntityService _localizedEntityService;

        public override void SetUp()
        {
            base.SetUp();

            _kernel.Resolve<MongoCollection<Language>>().RemoveAll();
            _kernel.Resolve<MongoCollection<LocaleStringResource>>().RemoveAll();
            _kernel.Resolve<MongoCollection<LocalizedProperty>>().RemoveAll();
            _kernel.Resolve<MongoCollection<Setting>>().RemoveAll();
            _languageService = _kernel.Resolve<ILanguageService>();
            _localizationService = _kernel.Resolve<ILocalizationService>();
            _localizedEntityService = _kernel.Resolve<ILocalizedEntityService>();
        }

        public override IEnumerable<Engine.IDependencyRegistrar> GetDependencyRegistrars()
        {
            var registrars = base.GetDependencyRegistrars().ToList();
            registrars.Add(new MongoDB.DependencyRegistrar());
            registrars.Add(new DependencyRegistrar());
            registrars.Add(new Settings.DependencyRegistrar());
            return registrars;
        }

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

        public LocaleStringResource CreateResource(int index, ObjectId languageId)
        {
            return new LocaleStringResource
                {
                    ResourceName = "name{0}".F(index),
                    ResourceValue = "value{0}".F(index),
                    LanguageId = languageId
                };
        }

        public void CreateSampleResources(ObjectId languageId)
        {
            for(var x = 1; x <= 23; x++)
            {
                _localizationService.InsertLocaleStringResource(CreateResource(x, languageId));
            }
        }
    }
}
