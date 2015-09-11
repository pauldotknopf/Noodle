using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Noodle.Extensions.Caching;
using Noodle.Extensions.Collections;

namespace Noodle.Localization.Services
{
    /// <summary>
    /// Provides information about localizable entities
    /// </summary>
    public partial class LocalizedEntityService : ILocalizedEntityService
    {
        #region Constants

        private const string LOCALIZEDPROPERTY_KEY = "Noodle.localizedproperty.{0}-{1}-{2}-{3}";
        private const string LOCALIZEDPROPERTY_PATTERN_KEY = "Noodle.localizedproperty.";

        #endregion

        #region Fields

        private readonly LocalizationSettings _localizationSettings;
        private readonly ICacheManager _cacheManager;
        private readonly MongoCollection<LocalizedProperty> _localizedPropertyCollection;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="localizedPropertyCollection">Localized property collection</param>
        /// <param name="localizationSettings">Localization settings</param>
        public LocalizedEntityService(ICacheManager cacheManager,
            MongoCollection<LocalizedProperty> localizedPropertyCollection,
            LocalizationSettings localizationSettings)
        {
            _cacheManager = cacheManager;
            _localizedPropertyCollection = localizedPropertyCollection;
            _localizationSettings = localizationSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a localized property
        /// </summary>
        /// <param name="localizedPropertyId">Localized property id</param>
        public virtual void DeleteLocalizedProperty(ObjectId localizedPropertyId)
        {
            if (localizedPropertyId == ObjectId.Empty)
                return;

            _localizedPropertyCollection.Remove(Query.EQ("_id", localizedPropertyId));

            _cacheManager.RemoveByPattern(LOCALIZEDPROPERTY_PATTERN_KEY);
        }

        /// <summary>
        /// Gets a localized property
        /// </summary>
        /// <param name="localizedPropertyId">Localized property identifier</param>
        /// <returns>Localized property</returns>
        public virtual LocalizedProperty GetLocalizedPropertyById(ObjectId localizedPropertyId)
        {
            if (localizedPropertyId == ObjectId.Empty)
                return null;

            return _localizedPropertyCollection.Find(Query.EQ("_id", localizedPropertyId)).SingleOrDefault();
        }

        /// <summary>
        /// Find localized value
        /// </summary>
        /// <param name="languageId">Language identifier. Null if using the default language</param>
        /// <param name="entityId">Entity identifier</param>
        /// <param name="localeKeyGroup">Locale key group</param>
        /// <param name="localeKey">Locale key</param>
        /// <returns>Found localized value</returns>
        public virtual string GetLocalizedValue(ObjectId entityId, string localeKeyGroup, string localeKey, ObjectId? languageId = null)
        {
            if (languageId == null)
                languageId = ObjectId.Parse(_localizationSettings.DefaultLanguageId);

            var key = string.Format(LOCALIZEDPROPERTY_KEY, languageId, entityId, localeKeyGroup, localeKey);
            return _cacheManager.Get(key, () =>
            {
                var result = _localizedPropertyCollection.Find(
                    Query.And(
                        Query.EQ("LanguageId", languageId),
                        Query.EQ("EntityId", entityId),
                        Query.EQ("LocaleKeyGroup", localeKeyGroup),
                        Query.EQ("LocaleKey", localeKey))).FirstOrDefault();
                return result != null
                            ? result.LocaleValue
                            : "";
            });
        }

        /// <summary>
        /// Gets localized properties
        /// </summary>
        /// <param name="entityId">Entity identifier</param>
        /// <param name="localeKeyGroup">Locale key group</param>
        /// <returns>Localized properties</returns>
        public virtual NamedCollection<LocalizedProperty> GetLocalizedProperties(ObjectId entityId, string localeKeyGroup)
        {
            if (entityId == ObjectId.Empty || string.IsNullOrEmpty(localeKeyGroup))
                return new NamedCollection<LocalizedProperty>();

            return new NamedCollection<LocalizedProperty>(_localizedPropertyCollection
                .Find(Query.And(
                        Query.EQ("EntityId", entityId),
                        Query.EQ("LocaleKeyGroup", localeKeyGroup))).ToList());
        }

        /// <summary>
        /// Gets localized properties
        /// </summary>
        /// <param name="entityId">Entity identifier</param>
        /// <returns>Localized properties</returns>
        public virtual NamedCollection<LocalizedProperty> GetLocalizedProperties<T>(ObjectId entityId) where T : BaseEntity<ObjectId>, ILocalizedEntity
        {
            return GetLocalizedProperties(entityId, typeof(T).Name);
        }

        /// <summary>
        /// Gets localized properties
        /// </summary>
        /// <param name="entity">entity</param>
        /// <returns>Localized properties</returns>
        public virtual NamedCollection<LocalizedProperty> GetLocalizedProperties<T>(T entity) where T : BaseEntity<ObjectId>, ILocalizedEntity
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

            _localizedPropertyCollection.Insert(localizedProperty);

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

            _localizedPropertyCollection.Update(Query.EQ("_id", localizedProperty.Id),
                                                Update<LocalizedProperty>.Replace(localizedProperty));

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
            ObjectId languageId) where T : BaseEntity<ObjectId>, ILocalizedEntity
        {
            SaveLocalizedValue<T, string>(entity, keySelector, localeValue, languageId);
        }

        public virtual void SaveLocalizedValue<T, TPropType>(T entity,
            Expression<Func<T, TPropType>> keySelector,
            TPropType localeValue,
            ObjectId languageId) where T : BaseEntity<ObjectId>, ILocalizedEntity
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            if (languageId == ObjectId.Empty)
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

            var localeValueStr = CommonHelper.To<string>(localeValue);

            if (prop != null)
            {
                if (string.IsNullOrEmpty(localeValueStr))
                {
                    //delete
                    // TODO
                    //DeleteLocalizedProperty(prop.Id);
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
