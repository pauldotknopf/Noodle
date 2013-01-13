using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Ninject;
using Noodle.Caching;
using Noodle.Data;
using Noodle.Settings;

namespace Noodle.Localization.Services
{
    /// <summary>
    /// Language service
    /// </summary>
    public partial class LanguageService : ILanguageService
    {
        #region Constants
        private const string LANGUAGES_ALL_KEY = "Nop.language.all-{0}";
        private const string LANGUAGES_BY_ID_KEY = "Nop.language.id-{0}";
        private const string LANGUAGES_PATTERN_KEY = "Nop.language.";
        #endregion

        #region Fields

        private readonly ICacheManager _cacheManager;
        private readonly MongoCollection<Language> _languageCollection;
        private readonly MongoCollection<LocaleStringResource> _localeStringResourceCollection;
        private readonly MongoCollection<LocalizedProperty> _localizedPropertyCollection;
        private readonly ISettingService _settingService;
        private readonly IConfigurationProvider<LocalizationSettings> _localizationSettings;
        private readonly IKernel _kernel;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="languageCollection"></param>
        /// <param name="localeStringResourceCollection"></param>
        /// <param name="localizedPropertyCollection"></param>
        /// <param name="settingService">Setting service</param>
        /// <param name="localizationSettings">Localization settings</param>
        public LanguageService(ICacheManager cacheManager,
            MongoCollection<Language> languageCollection,
            MongoCollection<LocaleStringResource> localeStringResourceCollection,
            MongoCollection<LocalizedProperty> localizedPropertyCollection,
            ISettingService settingService,
            IConfigurationProvider<LocalizationSettings> localizationSettings,
            IKernel kernel)
        {
            _cacheManager = cacheManager;
            _languageCollection = languageCollection;
            _localeStringResourceCollection = localeStringResourceCollection;
            _localizedPropertyCollection = localizedPropertyCollection;
            _settingService = settingService;
            _localizationSettings = localizationSettings;
            _kernel = kernel;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a language
        /// </summary>
        /// <param name="languageId">Language</param>
        public virtual void DeleteLanguage(ObjectId languageId)
        {
            if (languageId == ObjectId.Empty)
                return;

            _localeStringResourceCollection.Remove(Query.EQ("LanguageId", languageId));
            _localizedPropertyCollection.Remove(Query.EQ("LanguageId", languageId));
            _languageCollection.Remove(Query.EQ("_id", languageId), RemoveFlags.Single);

            // if language was default, reset the default
            var defaultLanguageId = ObjectId.Parse(_localizationSettings.Settings.DefaultLanguageId);
            if (defaultLanguageId == languageId)
            {
                SetDefaultLanguage(_languageCollection.Find(Query.EQ("Published", true))
                    .SetSortOrder(SortBy.Ascending("Name"))
                    .SetLimit(1).FirstOrDefault());

                // clear settings cache
                _settingService.ClearCache();
            }

            //cache
            _cacheManager.RemoveByPattern(LANGUAGES_PATTERN_KEY);
        }

        /// <summary>
        /// Gets all languages
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Language collection</returns>
        public virtual IList<Language> GetAllLanguages(bool showHidden = false)
        {
            string key = string.Format(LANGUAGES_ALL_KEY, showHidden);
            return _cacheManager.Get(key, () =>
            {

                var sortBy = SortBy.Ascending("DisplayOrder");
                return showHidden
                            ? _languageCollection.FindAll()
                                                    .SetSortOrder(sortBy)
                                                    .ToList()
                            : _languageCollection.Find(Query.EQ("Published", true))
                                                .SetSortOrder(sortBy)
                                                .ToList();
            });
        }

        /// <summary>
        /// Gets a language
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Language</returns>
        public virtual Language GetLanguageById(ObjectId languageId)
        {
            if (languageId == ObjectId.Empty)
                return null;

            var key = string.Format(LANGUAGES_BY_ID_KEY, languageId);
            return _cacheManager.Get(key, () => _languageCollection.FindOneById(languageId));
        }

        /// <summary>
        /// Inserts a language
        /// </summary>
        /// <param name="language">Language</param>
        public virtual Language InsertLanguage(Language language)
        {
            if (language == null)
                throw new ArgumentNullException("language");

            _languageCollection.Insert(language);

            // if there is no default language, set it now
            if (ObjectId.Parse(_localizationSettings.Settings.DefaultLanguageId) == ObjectId.Empty)
            {
                SetDefaultLanguage(_languageCollection.Find(Query.EQ("Published", true))
                    .SetSortOrder(SortBy.Ascending("Name"))
                    .SetLimit(1).FirstOrDefault());

                // clear settings cache
                _settingService.ClearCache();
            }

            //cache
            _cacheManager.RemoveByPattern(LANGUAGES_PATTERN_KEY);

            return language;
        }

        /// <summary>
        /// Updates a language
        /// </summary>
        /// <param name="language">Language</param>
        public virtual Language UpdateLanguage(Language language)
        {
            if (language == null)
                throw new ArgumentNullException("language");

            //update language
            _languageCollection.Update(Query.EQ("_id", language.Id), Update<Language>.Replace(language));

            //cache
            _cacheManager.RemoveByPattern(LANGUAGES_PATTERN_KEY);

            return language;
        }

        /// <summary>
        /// Delete all the languages. It also deletes all referenced localized values, so be careful!
        /// </summary>
        public virtual void DeleteAll()
        {
            _localizedPropertyCollection.RemoveAll();
            _localeStringResourceCollection.RemoveAll();
            _localeStringResourceCollection.RemoveAll();
        }

        /// <summary>
        /// Sets the default language to the given id
        /// </summary>
        /// <param name="languageId"></param>
        public virtual void SetDefaultLanguage(ObjectId languageId)
        {
            _localizationSettings.Settings.DefaultLanguageId = languageId.ToString();
            _localizationSettings.SaveSettings(_localizationSettings.Settings);
        }

        /// <summary>
        /// Sets the defaults language to the given language
        /// </summary>
        /// <param name="language"></param>
        public virtual void SetDefaultLanguage(Language language)
        {
            SetDefaultLanguage(language == null ? ObjectId.Empty : language.Id);
        }

        /// <summary>
        /// Gets the current default language id
        /// </summary>
        /// <returns></returns>
        public virtual ObjectId GetDefaultLanguageId()
        {
            return ObjectId.Parse(_localizationSettings.Settings.DefaultLanguageId);
        }

        #endregion
    }
}
