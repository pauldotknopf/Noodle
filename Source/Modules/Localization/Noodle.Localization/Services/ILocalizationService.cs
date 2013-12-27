using System;
using System.Linq.Expressions;
using MongoDB.Bson;
using Noodle.Collections;

namespace Noodle.Localization.Services
{
    /// <summary>
    /// Localization manager interface
    /// </summary>
    public interface ILocalizationService
    {
        /// <summary>
        /// Deletes a locale string resource
        /// </summary>
        /// <param name="localeStringResourceId">Locale string resource</param>
        void DeleteLocaleStringResource(ObjectId localeStringResourceId);

        /// <summary>
        /// Gets a locale string resource
        /// </summary>
        /// <param name="localeStringResourceId">Locale string resource identifier</param>
        /// <returns>Locale string resource</returns>
        LocaleStringResource GetLocaleStringResourceById(ObjectId localeStringResourceId);

        /// <summary>
        /// Gets a locale string resource
        /// </summary>
        /// <param name="resourceName">A string representing a resource name</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="logIfNotFound">A value indicating whether to log error if locale string resource is not found</param>
        /// <returns>Locale string resource</returns>
        LocaleStringResource GetLocaleStringResourceByName(string resourceName, ObjectId languageId,
            bool? logIfNotFound = null);

        /// <summary>
        /// Gets all locale string resources by language identifier
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Locale string resource collection</returns>
        NamedCollection<LocaleStringResource> GetAllResourcesByLanguageId(ObjectId languageId);

        /// <summary>
        /// Inserts a locale string resource
        /// </summary>
        /// <param name="localeStringResource">Locale string resource</param>
        LocaleStringResource InsertLocaleStringResource(LocaleStringResource localeStringResource);

        /// <summary>
        /// Inserts a locale string resources in a batch-like manner (for performance). The language installer uses this.
        /// </summary>
        /// <param name="localeStringResources">Locale string resources</param>
        void InsertLocaleStringResources(params LocaleStringResource[] localeStringResources);

        /// <summary>
        /// Updates the locale string resource
        /// </summary>
        /// <param name="localeStringResource">Locale string resource</param>
        LocaleStringResource UpdateLocaleStringResource(LocaleStringResource localeStringResource);

        /// <summary>
        /// Gets a resource string based on the specified ResourceKey property.
        /// </summary>
        /// <param name="resourceKey">A string representing a ResourceKey.</param>
        /// <param name="languageId">Language identifier. If null, default language will be used.</param>
        /// <param name="logIfNotFound">A value indicating whether to log error if locale string resource is not found</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="returnEmptyIfNotFound">A value indicating whether to empty string will be returned if a resource is not found and default value is set to empty string</param>
        /// <returns>A string representing the requested resource string.</returns>
        string GetResource(string resourceKey, ObjectId? languageId = null,
            bool? logIfNotFound = null, string defaultValue = "", bool returnEmptyIfNotFound = false);

        /// <summary>
        /// Gets a resource string based on the specified expression.
        /// </summary>
        /// <param name="expression">The expression used to build the resource key</param>
        /// <param name="languageId">Language identifier. If null, default language will be used.</param>
        /// <param name="logIfNotFound">A value indicating whether to log error if locale string resource is not found</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="returnEmptyIfNotFound">A value indicating whether to empty string will be returned if a resource is not found and default value is set to empty string</param>
        /// <returns>A string representing the requested resource string.</returns>
        /// <returns></returns>
        string GetResource(Expression<Func<string>> expression, ObjectId? languageId = null,
            bool? logIfNotFound = null, string defaultValue = "", bool returnEmptyIfNotFound = false);

        /// <summary>
        /// Clear cache
        /// </summary>
        void ClearCache();
    }
}
