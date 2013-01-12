using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Noodle.Caching;
using Noodle.Collections;
using Noodle.Data;
using Noodle.Localization.CodeFirst;
using Noodle.Settings;

namespace Noodle.Localization.Services
{
    /// <summary>
    /// Provides information about localization
    /// </summary>
    /// <remarks></remarks>
    public partial class LocalizationService : ILocalizationService
    {
        #region Constants
        private const string LOCALSTRINGRESOURCES_ALL_KEY = "Noodle.lsr.all-{0}";
        private const string LOCALSTRINGRESOURCES_BY_RESOURCENAME_KEY = "Noodle.lsr.{0}-{1}";
        private const string LOCALSTRINGRESOURCES_PATTERN_KEY = "Noodle.lsr.";
        #endregion

        #region Fields

        private readonly ICacheManager _cacheManager;
        private readonly MongoCollection<LocaleStringResource> _localeStringResourceCollection;
        private readonly IConfigurationProvider<LocalizationSettings> _localizationSettings;
        private readonly ILanguageService _languageService;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">The cache manager.</param>
        /// <param name="localeStringResourceCollection"></param>
        /// <param name="localizationSettings">The localization settings.</param>
        /// <param name="languageService"></param>
        /// <remarks></remarks>
        public LocalizationService(ICacheManager cacheManager,
            MongoCollection<LocaleStringResource> localeStringResourceCollection,
            IConfigurationProvider<LocalizationSettings> localizationSettings,
            ILanguageService languageService)
        {
            _cacheManager = cacheManager;
            _localeStringResourceCollection = localeStringResourceCollection;
            _localizationSettings = localizationSettings;
            _languageService = languageService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a locale string resource
        /// </summary>
        /// <param name="localeStringResourceId">Locale string resource</param>
        /// <remarks></remarks>
        public virtual void DeleteLocaleStringResource(ObjectId localeStringResourceId)
        {
            if (localeStringResourceId == ObjectId.Empty)
                return;

            _localeStringResourceCollection.Remove(Query.EQ("_id", localeStringResourceId));

            //cache
            _cacheManager.RemoveByPattern(LOCALSTRINGRESOURCES_PATTERN_KEY);
        }

        /// <summary>
        /// Gets a locale string resource
        /// </summary>
        /// <param name="localeStringResourceId">Locale string resource identifier</param>
        /// <returns>Locale string resource</returns>
        /// <remarks></remarks>
        public virtual LocaleStringResource GetLocaleStringResourceById(ObjectId localeStringResourceId)
        {
            return localeStringResourceId == ObjectId.Empty 
                ? null 
                : _localeStringResourceCollection.FindOneById(localeStringResourceId);
        }


        /// <summary>
        /// Gets a locale string resource
        /// </summary>
        /// <param name="resourceName">A string representing a resource name</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="logIfNotFound">A value indicating whether to log error if locale string resource is not found</param>
        /// <returns>Locale string resource</returns>
        /// <remarks></remarks>
        public virtual LocaleStringResource GetLocaleStringResourceByName(string resourceName, ObjectId languageId,
            bool? logIfNotFound = null)
        {
            //var localeStringResource = 

            LocaleStringResource localeStringResource = null;

            if (logIfNotFound == null)
                logIfNotFound = _localizationSettings.Settings.LogResourcesNotFound;

            if (_localizationSettings.Settings.LoadAllLocaleRecordsOnStartup)
            {
                //load all records
                var resources = GetAllResourcesByLanguageId(languageId);
                if (resources.ContainsKey(resourceName))
                {
                    localeStringResource = resources[resourceName];
                }
            }
            else
            {
                //gradual loading
                var key = string.Format(LOCALSTRINGRESOURCES_BY_RESOURCENAME_KEY, languageId, resourceName);
                localeStringResource = _cacheManager.Get(key, () => _localeStringResourceCollection.Find(
                    Query.And(Query.EQ("LanguageId", languageId),
                              Query.EQ("ResourceName", resourceName)))
                    .SetLimit(1)
                    .FirstOrDefault());
            }

            // TODO: Notifier
            //if (localeStringResource == null && logIfNotFound.Value)
            //_logger.Warning(string.Format("Resource string ({0}) not found. Language ID = {1}", resourceName, languageId));

            return localeStringResource;
        }

        /// <summary>
        /// Gets all locale string resources by language identifier
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Locale string resource collection</returns>
        public virtual NamedCollection<LocaleStringResource> GetAllResourcesByLanguageId(ObjectId languageId)
        {
            var key = string.Format(LOCALSTRINGRESOURCES_ALL_KEY, languageId);
            return _cacheManager.Get(key, () => new NamedCollection<LocaleStringResource>(
                _localeStringResourceCollection.Find(Query.EQ("LanguageId", languageId)).SetSortOrder(SortBy.Ascending("ResourceName"))));
        }

        /// <summary>
        /// Inserts a locale string resource
        /// </summary>
        /// <param name="localeStringResource">Locale string resource</param>
        /// <remarks></remarks>
        public virtual LocaleStringResource InsertLocaleStringResource(LocaleStringResource localeStringResource)
        {
            if (localeStringResource == null)
                throw new ArgumentNullException("localeStringResource");

            _localeStringResourceCollection.Insert(localeStringResource);

            //cache
            _cacheManager.RemoveByPattern(LOCALSTRINGRESOURCES_PATTERN_KEY);

            return localeStringResource;
        }

        /// <summary>
        /// Updates the locale string resource
        /// </summary>
        /// <param name="localeStringResource">Locale string resource</param>
        /// <remarks></remarks>
        public virtual LocaleStringResource UpdateLocaleStringResource(LocaleStringResource localeStringResource)
        {
            if (localeStringResource == null)
                throw new ArgumentNullException("localeStringResource");

            _localeStringResourceCollection.Update(Query.EQ("_id", localeStringResource.Id), Update<LocaleStringResource>.Replace(localeStringResource));

            //cache
            _cacheManager.RemoveByPattern(LOCALSTRINGRESOURCES_PATTERN_KEY);

            return localeStringResource;
        }

        /// <summary>
        /// Gets a resource string based on the specified ResourceKey property.
        /// </summary>
        /// <param name="resourceKey">A string representing a ResourceKey.</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="logIfNotFound">A value indicating whether to log error if locale string resource is not found</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="returnEmptyIfNotFound">A value indicating whether to empty string will be returned if a resource is not found and default value is set to empty string</param>
        /// <returns>A string representing the requested resource string.</returns>
        /// <remarks></remarks>
        public virtual string GetResource(string resourceKey, 
            ObjectId? languageId = null, 
            bool? logIfNotFound = null, 
            string defaultValue = "", 
            bool returnEmptyIfNotFound = false)
        {
            if (!languageId.HasValue)
                languageId = _languageService.GetDefaultLanguageId();

            var result = string.Empty;

            if (logIfNotFound == null)
                logIfNotFound = _localizationSettings.Settings.LogResourcesNotFound;

            if (_localizationSettings.Settings.LoadAllLocaleRecordsOnStartup)
            {
                //load all records
                var resources = GetAllResourcesByLanguageId(languageId.Value);

                if (resources.ContainsKey(resourceKey))
                {
                    var lsr = resources[resourceKey];
                    if (lsr != null)
                        result = lsr.ResourceValue;
                }else
                {
                    // TODO: Error notifier
                    //if (logIfNotFound.Value)
                    //    _logger.Warning(string.Format("Resource string ({0}) is not found. Language ID = {1}", resourceKey, languageId.Value));
                }
            }
            else
            {
                //gradual loading
                var lsr = GetLocaleStringResourceByName(resourceKey, languageId.Value, logIfNotFound);

                // note that we don't 'log if not found' as we do with 'load all records' because the method directly above already logged if the resource wasn't found

                if (lsr != null)
                    result = lsr.ResourceValue;
            }

            if (string.IsNullOrEmpty(result))
            {
                if (!String.IsNullOrEmpty(defaultValue))
                {
                    result = defaultValue;
                }
                else
                {
                    if (!returnEmptyIfNotFound)
                        result = resourceKey;
                }
            }
            return result;
        }

        /// <summary>
        /// Gets a resource string based on the specified expression.
        /// </summary>
        /// <param name="expression">The expression used to build the resource key</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="logIfNotFound">A value indicating whether to log error if locale string resource is not found</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="returnEmptyIfNotFound">A value indicating whether to empty string will be returned if a resource is not found and default value is set to empty string</param>
        /// <returns>A string representing the requested resource string.</returns>
        /// <remarks></remarks>
        public string GetResource(System.Linq.Expressions.Expression<Func<string>> expression, 
            ObjectId? languageId = null, 
            bool? logIfNotFound = null, 
            string defaultValue = "", 
            bool returnEmptyIfNotFound = false)
        {
            var expressionVisitor = new LocalizationNodeExpressionVisitor(expression);
            return GetResource(expressionVisitor.ResourceName, languageId, logIfNotFound, expressionVisitor.DefaultValue, returnEmptyIfNotFound);
        }

        /// <summary>
        /// Clear cache
        /// </summary>
        /// <remarks></remarks>
        public virtual void ClearCache()
        {
            _cacheManager.RemoveByPattern(LOCALSTRINGRESOURCES_PATTERN_KEY);
        }

        #endregion
    }
}
