using System;
using System.Collections.Generic;
using Noodle.Caching;
using Noodle.Data;

namespace Noodle.Localization.Services
{
    /// <summary>
    /// Language service
    /// </summary>
    public partial class LanguageService : ILanguageService
    {
        #region Constants
        private const string LANGUAGES_ALL_KEY = "Method.language.all-{0}";
        private const string LANGUAGES_BY_ID_KEY = "Method.language.id-{0}";
        private const string LANGUAGES_PATTERN_KEY = "Method.language.";
        #endregion

        #region Fields

        private readonly IRepository<Language> _languageRepository;
        private readonly ICacheManager _cacheManager;
        private readonly ISettingService _settingService;
        private readonly LocalizationSettings _localizationSettings;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="languageRepository">Language repository</param>
        /// <param name="settingService">Setting service</param>
        /// <param name="localizationSettings">Localization settings</param>
        /// <param name="eventPublisher"></param>
        public LanguageService(ICacheManager cacheManager,
            IRepository<Language> languageRepository,
            ISettingService settingService,
            LocalizationSettings localizationSettings)
        {
            _cacheManager = cacheManager;
            _languageRepository = languageRepository;
            _settingService = settingService;
            _localizationSettings = localizationSettings;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a language
        /// </summary>
        /// <param name="languageId">Language</param>
        public virtual void DeleteLanguage(int languageId)
        {
            if (languageId == 0)
                return;

            _languageRepository.ExecuteMethod<LocalizationDataContext>(context => context.DeleteLanguageById(languageId));

            // the delete sproc might have updated settings, lets clear the settings to be sure
            _settingService.ClearCache();

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
                using(var response = _languageRepository
                    .ExecuteMethodQuery<LocalizationDataContext, ISingleResult<Language>>(context 
                        => context.GetAllLanguages(showHidden)))
                {
                    return response.ToList();
                }
            });
        }

        /// <summary>
        /// Gets a language
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Language</returns>
        public virtual Language GetLanguageById(int languageId)
        {
            if (languageId == 0)
                return null;

            var key = string.Format(LANGUAGES_BY_ID_KEY, languageId);
            return _cacheManager.Get(key, () =>
            {
                using(var response = _languageRepository
                    .ExecuteMethodQuery<LocalizationDataContext, ISingleResult<Language>>(context 
                        => context.GetLanguageById(languageId)))
                {
                    return response.SingleOrDefault();
                }               
            });
        }

        /// <summary>
        /// Inserts a language
        /// </summary>
        /// <param name="language">Language</param>
        public virtual void InsertLanguage(Language language)
        {
            if (language == null)
                throw new ArgumentNullException("language");

            _languageRepository.Insert(language);

            //cache
            _cacheManager.RemoveByPattern(LANGUAGES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(language);
        }

        /// <summary>
        /// Updates a language
        /// </summary>
        /// <param name="language">Language</param>
        public virtual void UpdateLanguage(Language language)
        {
            if (language == null)
                throw new ArgumentNullException("language");

            //update language
            _languageRepository.Update(language);

            //cache
            _cacheManager.RemoveByPattern(LANGUAGES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(language);
        }

        #endregion
    }
}
