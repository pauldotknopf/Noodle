using System;

namespace Noodle.Localization.Tests
{
    [TestClass]
    public class LocalizationRepositoryTests : LocalizationTestsBase
    {
        [TestMethod]
        public void Can_insert_update_delete_language()
        {
            Func<Language, int> hash = (language) => language.DisplayOrder.GetHashCode()
                + language.FlagImageFileName.GetHashCode()
                + language.Id.GetHashCode()
                + language.LanguageCulture.GetHashCode()
                + language.Name.GetHashCode()
                + language.Published.GetHashCode()
                + language.Rtl.GetHashCode()
                + language.UniqueSeoCode.GetHashCode();

            new RepositoryTestHelper<Language>(Kernel, hash, CreateLanguage)
                .CanInsertUpdateDelete();
        }

        [TestMethod]
        public void Can_insert_update_delete_resource()
        {
            Func<LocaleStringResource, int> hash = (resource) => resource.Id.GetHashCode()
                + resource.LanguageId.Value.GetHashCode()
                + resource.ResourceName.GetHashCode()
                + resource.ResourceValue.GetHashCode();

            var language1 = Kernel.Resolve<IRepository<Language>>().Insert(CreateLanguage(1));
            var language2 = Kernel.Resolve<IRepository<Language>>().Insert(CreateLanguage(2));

            new RepositoryTestHelper<LocaleStringResource>(Kernel, 
                hash, 
                (index) => new LocaleStringResource
                {
                    ResourceName = "name{0}".F(index),
                    ResourceValue = "value{0}".F(index),
                    LanguageId = (index % 2) == 0 ? language1.Id : language2.Id
                })
                .CanInsertUpdateDelete();
        }

        [TestMethod]
        public void Can_insert_update_delete_entity_resource()
        {
            Func<LocalizedProperty, int> hash = (prop) => prop.EntityId.GetHashCode()
                + prop.Id.GetHashCode()
                + prop.LanguageId.Value.GetHashCode()
                + prop.LocaleKey.GetHashCode()
                + prop.LocaleKeyGroup.GetHashCode()
                + prop.LocaleValue.GetHashCode();

            var language1 = Kernel.Resolve<IRepository<Language>>().Insert(CreateLanguage(1));
            var language2 = Kernel.Resolve<IRepository<Language>>().Insert(CreateLanguage(2));

            new RepositoryTestHelper<LocalizedProperty>(Kernel,
                hash,
                (index) => new LocalizedProperty
                {
                    EntityId = index * 2,
                    LanguageId = (index % 2) == 0 ? language1.Id : language2.Id,
                    LocaleKey = "key{0}".F(index),
                    LocaleKeyGroup = "group{0}".F(index),
                    LocaleValue = "value{0}".F(index)
                })
                .CanInsertUpdateDelete();
        }

        #region Database

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            DataTestBaseHelper.DropCreateDatabase<LocalizationRepositoryTests>();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            DataTestBaseHelper.DropDatabase<LocalizationRepositoryTests>();
        }

        #endregion
    }
}
