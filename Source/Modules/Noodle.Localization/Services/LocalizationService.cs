using System;
using Noodle.Caching;
using Noodle.Data;
using Noodle.Localization.CodeFirst;

namespace Noodle.Localization.Services
{
    /// <summary>
    /// Provides information about localization
    /// </summary>
    /// <remarks></remarks>
    public partial class LocalizationService : ILocalizationService
    {
        #region Constants
        private const string LOCALSTRINGRESOURCES_ALL_KEY = "Nop.lsr.all-{0}";
        private const string LOCALSTRINGRESOURCES_BY_RESOURCENAME_KEY = "Nop.lsr.{0}-{1}";
        private const string LOCALSTRINGRESOURCES_PATTERN_KEY = "Nop.lsr.";
        #endregion

        #region Fields

        private readonly IRepository<LocaleStringResource> _lsrRepository;
        private readonly ICacheManager _cacheManager;
        private readonly LocalizationSettings _localizationSettings;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">The cache manager.</param>
        /// <param name="lsrRepository">The LSR repository.</param>
        /// <param name="localizationSettings">The localization settings.</param>
        /// <remarks></remarks>
        public LocalizationService(ICacheManager cacheManager,
            IRepository<LocaleStringResource> lsrRepository, LocalizationSettings localizationSettings)
        {
            _cacheManager = cacheManager;
            _lsrRepository = lsrRepository;
            _localizationSettings = localizationSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a locale string resource
        /// </summary>
        /// <param name="localeStringResourceId">Locale string resource</param>
        /// <remarks></remarks>
        public virtual void DeleteLocaleStringResource(int localeStringResourceId)
        {
            if (localeStringResourceId == 0)
                return;

            _lsrRepository.ExecuteMethod<LocalizationDataContext>(context => context.DeleteLocaleStringResourceById(localeStringResourceId));

            //cache
            _cacheManager.RemoveByPattern(LOCALSTRINGRESOURCES_PATTERN_KEY);
        }

        /// <summary>
        /// Gets a locale string resource
        /// </summary>
        /// <param name="localeStringResourceId">Locale string resource identifier</param>
        /// <returns>Locale string resource</returns>
        /// <remarks></remarks>
        public virtual LocaleStringResource GetLocaleStringResourceById(int localeStringResourceId)
        {
            if (localeStringResourceId == 0)
                return null;

            var localeStringResource = _lsrRepository.GetById(localeStringResourceId);

            return localeStringResource;
        }


        /// <summary>
        /// Gets a locale string resource
        /// </summary>
        /// <param name="resourceName">A string representing a resource name</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="logIfNotFound">A value indicating whether to log error if locale string resource is not found</param>
        /// <returns>Locale string resource</returns>
        /// <remarks></remarks>
        public virtual LocaleStringResource GetLocaleStringResourceByName(string resourceName, int languageId,
            bool? logIfNotFound = null)
        {
            LocaleStringResource localeStringResource = null;

            if (logIfNotFound == null)
                logIfNotFound = _localizationSettings.LogResourcesNotFound;

            if (_localizationSettings.LoadAllLocaleRecordsOnStartup)
            {
                //load all records

                // using an empty string so the request can still be logged
                if (string.IsNullOrEmpty(resourceName))
                    resourceName = string.Empty;
                resourceName = resourceName.Trim().ToLowerInvariant();

                var resources = GetAllResourcesByLanguageId(languageId);
                if (resources.ContainsKey(resourceName))
                {
                    var localeStringResourceId = resources[resourceName].Id;
                    localeStringResource = _lsrRepository.GetById(localeStringResourceId);
                }
            }
            else
            {
                //gradual loading
                var query = from lsr in _lsrRepository.Table
                            orderby lsr.ResourceName
                            where lsr.LanguageId == languageId && lsr.ResourceName == resourceName
                            select lsr;
                localeStringResource = query.FirstOrDefault();
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
        public virtual NamedCollection<LocaleStringResource> GetAllResourcesByLanguageId(int languageId)
        {
            string key = string.Format(LOCALSTRINGRESOURCES_ALL_KEY, languageId);
            return _cacheManager.Get(key, () =>
            {
                var query = from l in _lsrRepository.Table
                            orderby l.ResourceName
                            where l.LanguageId == languageId
                            select l;
                var collection = new NamedCollection<LocaleStringResource>(query.ToList());
                return collection;
            });
        }

        /// <summary>
        /// Inserts a locale string resource
        /// </summary>
        /// <param name="localeStringResource">Locale string resource</param>
        /// <remarks></remarks>
        public virtual void InsertLocaleStringResource(LocaleStringResource localeStringResource)
        {
            if (localeStringResource == null)
                throw new ArgumentNullException("localeStringResource");

            _lsrRepository.Insert(localeStringResource);

            //cache
            _cacheManager.RemoveByPattern(LOCALSTRINGRESOURCES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(localeStringResource);
        }

        /// <summary>
        /// Updates the locale string resource
        /// </summary>
        /// <param name="localeStringResource">Locale string resource</param>
        /// <remarks></remarks>
        public virtual void UpdateLocaleStringResource(LocaleStringResource localeStringResource)
        {
            if (localeStringResource == null)
                throw new ArgumentNullException("localeStringResource");

            _lsrRepository.Update(localeStringResource);

            //cache
            _cacheManager.RemoveByPattern(LOCALSTRINGRESOURCES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(localeStringResource);
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
            int? languageId = null, 
            bool? logIfNotFound = null, 
            string defaultValue = "", 
            bool returnEmptyIfNotFound = false)
        {
            if (!languageId.HasValue)
                languageId = _localizationSettings.DefaultLanguageId;

            string result = string.Empty;
            var resourceKeyValue = resourceKey;
            if (resourceKeyValue == null)
                resourceKeyValue = string.Empty;
            resourceKeyValue = resourceKeyValue.Trim().ToLowerInvariant();

            if (logIfNotFound == null)
                logIfNotFound = _localizationSettings.LogResourcesNotFound;

            if (_localizationSettings.LoadAllLocaleRecordsOnStartup)
            {
                //load all records
                var resources = GetAllResourcesByLanguageId(languageId.Value);

                if (resources.ContainsKey(resourceKeyValue))
                {
                    var lsr = resources[resourceKeyValue];
                    if (lsr != null)
                        result = lsr.ResourceValue;
                }
            }
            else
            {
                //gradual loading
                string key = string.Format(LOCALSTRINGRESOURCES_BY_RESOURCENAME_KEY, languageId.Value, resourceKeyValue);
                string lsr = _cacheManager.Get(key, () =>
                {
                    var query = from l in _lsrRepository.Table
                                where l.ResourceName == resourceKeyValue
                                && l.LanguageId == languageId.Value
                                select l.ResourceValue;
                    return query.FirstOrDefault();
                });

                if (lsr != null)
                    result = lsr;
            }
            if (string.IsNullOrEmpty(result))
            {
                // TODO: Error notifier
                //if (logIfNotFound.Value)
                //    _logger.Warning(string.Format("Resource string ({0}) is not found. Language ID = {1}", resourceKey, languageId.Value));

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
            int? languageId = null, 
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
