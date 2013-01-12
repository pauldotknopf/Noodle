﻿namespace Noodle.Localization.Tests
{
    [TestClass]
    public class LocalizedEntityServiceTests : LocalizationTestsBase
    {
        [TestMethod]
        public void Can_get_localized_properties()
        {
            // setup
            var language = LanguageRepository.Insert(CreateLanguage());
            var entity = new LocalizedEntity{ Id = 1 };
            LocalizedEntityService.SaveLocalizedValue(entity, (x) => x.Value1, "Value 1...", language.Id);
            LocalizedEntityService.SaveLocalizedValue(entity, (x) => x.Value2, "Value 2...", language.Id);
            entity.Id = 2;
            LocalizedEntityService.SaveLocalizedValue(entity, (x) => x.Value1, "Value 11...", language.Id);
            LocalizedEntityService.SaveLocalizedValue(entity, (x) => x.Value2, "Value 22...", language.Id);

            // act
            var values = LocalizedEntityService.GetLocalizedProperties(entity);

            // assert
            values.Count.ShouldEqual(2);
            values[0].LocaleValue.ShouldEqual("Value 11...");
            values[1].LocaleValue.ShouldEqual("Value 22...");
        }

        [TestMethod]
        public void Can_update_localized_properties()
        {
            // setup
            var language = LanguageRepository.Insert(CreateLanguage());
            var entity = new LocalizedEntity { Id = 1 };
            LocalizedEntityService.SaveLocalizedValue(entity, (x) => x.Value1, "Value 1...", language.Id);
            LocalizedEntityService.SaveLocalizedValue(entity, (x) => x.Value2, "Value 2...", language.Id);
            LocalizedEntityService.SaveLocalizedValue(entity, (x) => x.Value1, "Value 1 updated...", language.Id);
            LocalizedEntityService.SaveLocalizedValue(entity, (x) => x.Value2, "Value 2 updated...", language.Id);

            // act
            var values = LocalizedEntityService.GetLocalizedProperties(entity);

            // assert
            values.Count.ShouldEqual(2);
            values[0].LocaleValue.ShouldEqual("Value 1 updated...");
            values[1].LocaleValue.ShouldEqual("Value 2 updated...");
        }

        [TestMethod]
        public void Can_get_localized_value()
        {
            // setup
            var language = LanguageRepository.Insert(CreateLanguage());
            var entity = new LocalizedEntity { Id = 1 };
            LocalizedEntityService.SaveLocalizedValue(entity, (x) => x.Value1, "Value 1...", language.Id);
            LocalizedEntityService.SaveLocalizedValue(entity, (x) => x.Value2, "Value 2...", language.Id);

            // act
            var value1 = LocalizedEntityService.GetLocalizedValue<LocalizedEntity>(entity.Id, x => x.Value1, language.Id);
            var value2 = LocalizedEntityService.GetLocalizedValue<LocalizedEntity>(entity.Id, x => x.Value2, language.Id);

            // assert
            value1.ShouldEqual("Value 1...");
            value2.ShouldEqual("Value 2...");
        }

        [TestMethod]
        public void Can_get_localized_value_using_default_language()
        {
            // setup
            var language1 = LanguageRepository.Insert(CreateLanguage(1));
            var language2 = LanguageRepository.Insert(CreateLanguage(2));
            var entity = new LocalizedEntity { Id = 1 };
            LocalizedEntityService.SaveLocalizedValue(entity, (x) => x.Value1, "language 1 value 1.", language1.Id);
            LocalizedEntityService.SaveLocalizedValue(entity, (x) => x.Value2, "language 1 value 2", language1.Id);
            LocalizedEntityService.SaveLocalizedValue(entity, (x) => x.Value1, "language 2 value 1", language2.Id);
            LocalizedEntityService.SaveLocalizedValue(entity, (x) => x.Value2, "language 2 value 2", language2.Id);
            var setting = Kernel.Resolve<LocalizationSettings>();
            setting.DefaultLanguageId = language2.Id;
            Kernel.Resolve<ISettingService>().SaveSetting(setting);
            LocalizedEntityService = Kernel.Resolve<ILocalizedEntityService>();

            // act
            var value1 = LocalizedEntityService.GetLocalizedValue<LocalizedEntity>(entity.Id, x => x.Value1);
            var value2 = LocalizedEntityService.GetLocalizedValue<LocalizedEntity>(entity.Id, x => x.Value2);

            // assert
            value1.ShouldEqual("language 2 value 1");
            value2.ShouldEqual("language 2 value 2");
        }

        #region Database

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            DataTestBaseHelper.DropCreateDatabase<LocalizedEntityServiceTests>();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            DataTestBaseHelper.DropDatabase<LocalizedEntityServiceTests>();
        }

        #endregion

        #region Nested types

        public class LocalizedEntity : BaseEntity, ILocalizedEntity
        {
            public string Value1 { get; set; }
            public string Value2 { get; set; }
        }

        #endregion
    }
}
