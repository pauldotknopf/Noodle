namespace Noodle.Localization.Tests
{
    [TestClass]
    public class LanguageServiceTests : LocalizationTestsBase
    {
        [TestMethod]
        public void Can_update_default_language_id_when_deleting_a_language()
        {
            // setup
            var language1 = CreateLanguage(1);
            var language2 = CreateLanguage(2);
            LanguageService.InsertLanguage(language1);
            LanguageService.InsertLanguage(language2);
            var localizationSettings = Kernel.Resolve<LocalizationSettings>();
            localizationSettings.DefaultLanguageId = language1.Id;
            Kernel.Resolve<ISettingService>().SaveSetting(localizationSettings);

            // act
            LanguageService.DeleteLanguage(language1.Id);

            // assert
            localizationSettings = Kernel.Resolve<LocalizationSettings>();
            localizationSettings.DefaultLanguageId.ShouldEqual(language2.Id);
        }

        [TestMethod]
        public void Can_get_all_published_languages()
        {
            // setup
            var language1 = CreateLanguage(1);
            language1.Published = false;
            var language2 = CreateLanguage(2);
            language2.Published = true;
            LanguageService.InsertLanguage(language1);
            LanguageService.InsertLanguage(language2);

            // act
            var languages = LanguageService.GetAllLanguages();

            // assert
            languages.Count.ShouldEqual(1);
            GetLanguageComparer().Equals(languages[0], language2).ShouldBeTrue();
        }

        [TestMethod]
        public void Can_get_all_languages()
        {
            // setup
            var language1 = CreateLanguage(1);
            language1.Published = true;
            var language2 = CreateLanguage(2);
            language2.Published = true;
            LanguageService.InsertLanguage(language1);
            LanguageService.InsertLanguage(language2);

            // act
            var languages = LanguageService.GetAllLanguages();

            // assert
            languages.Count.ShouldEqual(2);
        }

        #region Database

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            DataTestBaseHelper.DropCreateDatabase<LanguageServiceTests>();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            DataTestBaseHelper.DropDatabase<LanguageServiceTests>();
        }

        #endregion
    }
}
