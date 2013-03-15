using System.Diagnostics;
using NUnit.Framework;
using Noodle.Caching;
using Noodle.Settings;
using Noodle.Tests;

namespace Noodle.Localization.Tests
{
    [TestFixture]
    public class LanguageServiceTests : LocalizationTestsBase
    {
        [Test]
        public void Can_update_default_language_id_when_deleting_a_language()
        {
            // setup
            var language1 = new Language {Name = "language 1", Published = true};
            var language2 = new Language { Name = "language 2", Published = true };

            // act
            _languageService.InsertLanguage(language1);
            _languageService.InsertLanguage(language2);
            Trace.WriteLine("Langauge 1:" + language1.Id.ToString());
            Trace.WriteLine("Language 2:" + language2.Id.ToString());

            // assert
            Assert.AreEqual(language1.Id.ToString(), _container.Resolve<IConfigurationProvider<LocalizationSettings>>().Settings.DefaultLanguageId);

            // act
            _languageService.DeleteLanguage(language1.Id);

            // assert
            Assert.AreEqual(language2.Id.ToString(), _container.Resolve<IConfigurationProvider<LocalizationSettings>>().Settings.DefaultLanguageId);
        }

        [Test]
        public void Can_get_all_published_languages()
        {
            // setup
            var language1 = CreateLanguage(1);
            language1.Published = false;
            var language2 = CreateLanguage(2);
            language2.Published = true;
            _languageService.InsertLanguage(language1);
            _languageService.InsertLanguage(language2);

            // act
            var languages = _languageService.GetAllLanguages();

            // assert
            languages.Count.ShouldEqual(1);
            GetLanguageComparer().Equals(languages[0], language2).ShouldBeTrue();
        }

        [Test]
        public void Can_get_all_languages()
        {
            // setup
            var language1 = CreateLanguage(1);
            language1.Published = true;
            var language2 = CreateLanguage(2);
            language2.Published = true;
            _languageService.InsertLanguage(language1);
            _languageService.InsertLanguage(language2);

            // act
            var languages = _languageService.GetAllLanguages();

            // assert
            languages.Count.ShouldEqual(2);
        }
    }
}
