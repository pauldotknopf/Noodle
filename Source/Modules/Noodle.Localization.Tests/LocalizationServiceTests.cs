namespace Noodle.Localization.Tests
{
    [TestClass]
    public class LocalizationServiceTests : LocalizationTestsBase
    {
        [TestMethod]
        public void Can_get_all_resources_by_langage()
        {
            // setup
            var language1 = LanguageRepository.Insert(CreateLanguage(1));
            var language2 = LanguageRepository.Insert(CreateLanguage(2));
            CreateSampleResources(language1.Id);
            CreateSampleResources(language2.Id);

            // act
            var resources = LocalizationService.GetAllResourcesByLanguageId(language1.Id);

            // assert
            resources.Count.ShouldEqual(23);
            resources.All(x => x.LanguageId == language1.Id).ShouldBeTrue();
        }

        [TestMethod]
        public void Can_get_resource_by_id()
        {
            // setup
            var resource = LocalStringResourceRepository.Insert(CreateResource(1, LanguageRepository.Insert(CreateLanguage()).Id));

            // act
            var dbResource = LocalizationService.GetLocaleStringResourceById(resource.Id);

            // assert
            dbResource.ShouldNotBeNull();
            dbResource.Id.ShouldEqual(resource.Id);
        }

        [TestMethod]
        public void Can_get_resource_by_name()
        {
            // setup
            var resource = LocalStringResourceRepository.Insert(CreateResource(1, LanguageRepository.Insert(CreateLanguage()).Id));

            // act
            var dbResource = LocalizationService.GetLocaleStringResourceByName(resource.ResourceName.ToUpper(), resource.LanguageId.Value);

            // assert
            dbResource.ShouldNotBeNull();
            dbResource.Id.ShouldEqual(resource.Id);
        }

        [TestMethod]
        public void Can_get_resource_from_default_language()
        {
            // setup
            var language = LanguageRepository.Insert(CreateLanguage(1));
            var localizationSetting = Kernel.Resolve<LocalizationSettings>();
            localizationSetting.DefaultLanguageId = language.Id ;
            Kernel.Resolve<ISettingService>().SaveSetting(localizationSetting);
            var resource = CreateResource(1, language.Id);
            resource.ResourceName = "test default";
            resource.ResourceValue = "test default value";
            LocalStringResourceRepository.Insert(resource);

            // act
            var value = Kernel.Resolve<ILocalizationService>().GetResource("test default");

            // assert
            value.ShouldEqual("test default value");
        }

        [TestMethod]
        public void Can_get_default_value_if_resource_not_found()
        {
            // setup
            var resource = LocalStringResourceRepository.Insert(CreateResource(1, LanguageRepository.Insert(CreateLanguage()).Id));
            var localizationSetting = Kernel.Resolve<LocalizationSettings>();
            localizationSetting.DefaultLanguageId = resource.LanguageId.Value;
            Kernel.Resolve<ISettingService>().SaveSetting(localizationSetting);

            // act
            var value = Kernel.Resolve<ILocalizationService>().GetResource("non existing value....", defaultValue: "return this if not found...");

            // assert
            value.ShouldEqual("return this if not found...");
        }

        [TestMethod]
        public void Can_return_empty_if_not_resource_found()
        {
            // setup
            var resource = LocalStringResourceRepository.Insert(CreateResource(1, LanguageRepository.Insert(CreateLanguage()).Id));
            var localizationSetting = Kernel.Resolve<LocalizationSettings>();
            localizationSetting.DefaultLanguageId = resource.LanguageId.Value;
            Kernel.Resolve<ISettingService>().SaveSetting(localizationSetting);

            // act
            var value = Kernel.Resolve<ILocalizationService>().GetResource("non existing value....", returnEmptyIfNotFound:true);

            // assert
            value.ShouldEqual(string.Empty);

            // act
            value = Kernel.Resolve<ILocalizationService>().GetResource("non existing value....", returnEmptyIfNotFound: false);

            // assert
            value.ShouldEqual("non existing value....");
        }

        [TestMethod, Ignore]
        public void Can_log_if_resource_not_found()
        {
            // todo
        }

        #region Database

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            DataTestBaseHelper.DropCreateDatabase<LocalizationServiceTests>();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            DataTestBaseHelper.DropDatabase<LocalizationServiceTests>();
        }

        #endregion
    }
}
