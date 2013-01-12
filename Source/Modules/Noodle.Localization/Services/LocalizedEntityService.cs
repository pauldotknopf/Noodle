using System;
using System.Linq.Expressions;
using System.Reflection;
using Noodle.Caching;
using Noodle.Collections;
using Noodle.Data;

namespace Noodle.Localization.Services
{
    /// <summary>
    /// Provides information about localizable entities
    /// </summary>
    public partial class LocalizedEntityService : ILocalizedEntityService
    {
        #region Constants

        private const string LOCALIZEDPROPERTY_KEY = "Nop.localizedproperty.{0}-{1}-{2}-{3}";
        private const string LOCALIZEDPROPERTY_PATTERN_KEY = "Nop.localizedproperty.";

        #endregion

        #region Fields

        private readonly IRepository<LocalizedProperty> _localizedPropertyRepository;
        private readonly LocalizationSettings _localizationSettings;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="localizedPropertyRepository">Localized property repository</param>
        /// <param name="localizationSettings">Localization settings</param>
        public LocalizedEntityService(ICacheManager cacheManager,
            IRepository<LocalizedProperty> localizedPropertyRepository,
            LocalizationSettings localizationSettings)
        {
            _cacheManager = cacheManager;
            _localizedPropertyRepository = localizedPropertyRepository;
            _localizationSettings = localizationSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a localized property
        /// </summary>
        /// <param name="localizedPropertyId">Localized property id</param>
        public virtual void DeleteLocalizedProperty(int localizedPropertyId)
        {
            if (localizedPropertyId == 0)
                return;

            _localizedPropertyRepository.ExecuteMethod<LocalizationDataContext>(context => context.DeleteLocalizedPropertyById(localizedPropertyId));

            _cacheManager.RemoveByPattern(LOCALIZEDPROPERTY_PATTERN_KEY);
        }

        /// <summary>
        /// Gets a localized property
        /// </summary>
        /// <param name="localizedPropertyId">Localized property identifier</param>
        /// <returns>Localized property</returns>
        public virtual LocalizedProperty GetLocalizedPropertyById(int localizedPropertyId)
        {
            if (localizedPropertyId == 0)
                return null;

            var localizedProperty = _localizedPropertyRepository.GetById(localizedPropertyId);
            return localizedProperty;
        }

        /// <summary>
        /// Find localized value
        /// </summary>
        /// <param name="languageId">Language identifier. Null if using the default language</param>
        /// <param name="entityId">Entity identifier</param>
        /// <param name="localeKeyGroup">Locale key group</param>
        /// <param name="localeKey">Locale key</param>
        /// <returns>Found localized value</returns>
        public virtual string GetLocalizedValue(int entityId, string localeKeyGroup, string localeKey, int? languageId = null)
        {
            if (languageId == null)
                languageId = _localizationSettings.DefaultLanguageId;

            string key = string.Format(LOCALIZEDPROPERTY_KEY, languageId, entityId, localeKeyGroup, localeKey);
            return _cacheManager.Get(key, () =>
            {
                var query = from lp in _localizedPropertyRepository.Table
                            where lp.LanguageId == languageId &&
                            lp.EntityId == entityId &&
                            lp.LocaleKeyGroup == localeKeyGroup &&
                            lp.LocaleKey == localeKey
                            select lp.LocaleValue;
                var localeValue = query.FirstOrDefault() ?? "";
                return localeValue;
            });
        }

        /// <summary>
        /// Gets localized properties
        /// </summary>
        /// <param name="entityId">Entity identifier</param>
        /// <param name="localeKeyGroup">Locale key group</param>
        /// <returns>Localized properties</returns>
        public virtual NamedCollection<LocalizedProperty> GetLocalizedProperties(int entityId, string localeKeyGroup)
        {
            if (entityId == 0 || string.IsNullOrEmpty(localeKeyGroup))
                return new NamedCollection<LocalizedProperty>();

            var query = from lp in _localizedPropertyRepository.Table
                        orderby lp.Id
                        where lp.EntityId == entityId &&
                              lp.LocaleKeyGroup == localeKeyGroup
                        select lp;

            return new NamedCollection<LocalizedProperty>(query.ToList());
        }

        /// <summary>
        /// Gets localized properties
        /// </summary>
        /// <param name="entityId">Entity identifier</param>
        /// <returns>Localized properties</returns>
        public virtual NamedCollection<LocalizedProperty> GetLocalizedProperties<T>(int entityId) where T : BaseEntity, ILocalizedEntity
        {
            return GetLocalizedProperties(entityId, typeof(T).Name);
        }

        /// <summary>
        /// Gets localized properties
        /// </summary>
        /// <param name="entity">entity</param>
        /// <returns>Localized properties</returns>
        public virtual NamedCollection<LocalizedProperty> GetLocalizedProperties<T>(T entity) where T : BaseEntity, ILocalizedEntity
        {
            return GetLocalizedProperties(entity.Id, typeof(T).Name);
        }

        /// <summary>
        /// Inserts a localized property
        /// </summary>
        /// <param name="localizedProperty">Localized property</param>
        public virtual void InsertLocalizedProperty(LocalizedProperty localizedProperty)
        {
            if (localizedProperty == null)
                throw new ArgumentNullException("localizedProperty");

            _localizedPropertyRepository.Insert(localizedProperty);

            //cache
            _cacheManager.RemoveByPattern(LOCALIZEDPROPERTY_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the localized property
        /// </summary>
        /// <param name="localizedProperty">Localized property</param>
        public virtual void UpdateLocalizedProperty(LocalizedProperty localizedProperty)
        {
            if (localizedProperty == null)
                throw new ArgumentNullException("localizedProperty");

            _localizedPropertyRepository.Update(localizedProperty);

            //cache
            _cacheManager.RemoveByPattern(LOCALIZEDPROPERTY_PATTERN_KEY);
        }

        /// <summary>
        /// Save localized value
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="keySelector">Ley selector</param>
        /// <param name="localeValue">Locale value</param>
        /// <param name="languageId">Language ID</param>
        public virtual void SaveLocalizedValue<T>(T entity,
            Expression<Func<T, string>> keySelector,
            string localeValue,
            int languageId) where T : BaseEntity, ILocalizedEntity
        {
            SaveLocalizedValue<T, string>(entity, keySelector, localeValue, languageId);
        }

        public virtual void SaveLocalizedValue<T, TPropType>(T entity,
            Expression<Func<T, TPropType>> keySelector,
            TPropType localeValue,
            int languageId) where T : BaseEntity, ILocalizedEntity
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            if (languageId == 0)
                throw new ArgumentOutOfRangeException("languageId", "Language ID should not be 0");

            var member = keySelector.Body as MemberExpression;
            if (member == null)
            {
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    keySelector));
            }

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
            {
                throw new ArgumentException(string.Format(
                       "Expression '{0}' refers to a field, not a property.",
                       keySelector));
            }

            string localeKeyGroup = typeof(T).Name;
            string localeKey = propInfo.Name;

            var props = GetLocalizedProperties(entity.Id, localeKeyGroup);
            var prop = props.FirstOrDefault(lp => lp.LanguageId == languageId &&
                lp.LocaleKey.Equals(localeKey, StringComparison.InvariantCultureIgnoreCase)); //should be culture invariant

            string localeValueStr = CommonHelper.To<string>(localeValue);

            if (prop != null)
            {
                if (string.IsNullOrEmpty(localeValueStr))
                {
                    //delete
                    DeleteLocalizedProperty(prop.Id);
                }
                else
                {
                    //update
                    prop.LocaleValue = localeValueStr;
                    UpdateLocalizedProperty(prop);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(localeValueStr))
                {
                    //insert
                    prop = new LocalizedProperty()
                    {
                        EntityId = entity.Id,
                        LanguageId = languageId,
                        LocaleKey = localeKey,
                        LocaleKeyGroup = localeKeyGroup,
                        LocaleValue = localeValueStr
                    };
                    InsertLocalizedProperty(prop);
                }
            }
        }

        #endregion
    }
}
