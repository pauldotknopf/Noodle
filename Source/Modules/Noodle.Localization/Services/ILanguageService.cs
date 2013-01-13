using System.Collections.Generic;
using MongoDB.Bson;

namespace Noodle.Localization.Services
{
    /// <summary>
    /// Language service interface
    /// </summary>
    public partial interface ILanguageService
    {
        /// <summary>
        /// Deletes a language
        /// </summary>
        /// <param name="languageId">Language</param>
        void DeleteLanguage(ObjectId languageId);

        /// <summary>
        /// Gets all languages
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Language collection</returns>
        IList<Language> GetAllLanguages(bool showHidden = false);

        /// <summary>
        /// Gets a language
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Language</returns>
        Language GetLanguageById(ObjectId languageId);

        /// <summary>
        /// Inserts a language
        /// </summary>
        /// <param name="language">Language</param>
        Language InsertLanguage(Language language);

        /// <summary>
        /// Updates a language
        /// </summary>
        /// <param name="language">Language</param>
        Language UpdateLanguage(Language language);

        /// <summary>
        /// Delete all the languages. It also deletes all referenced localized values, so be careful!
        /// </summary>
        void DeleteAll();

        /// <summary>
        /// Sets the default language to the given id
        /// </summary>
        /// <param name="languageId"></param>
        void SetDefaultLanguage(ObjectId languageId);

        /// <summary>
        /// Sets the defaults language to the given language
        /// </summary>
        /// <param name="language"></param>
        void SetDefaultLanguage(Language language);

        /// <summary>
        /// Gets the current default language id
        /// </summary>
        /// <returns></returns>
        ObjectId GetDefaultLanguageId();
    }
}
