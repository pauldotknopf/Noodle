using System;
using System.Linq.Expressions;
using MongoDB.Bson;
using Noodle.Extensions.Collections;

namespace Noodle.Localization.Services
{
    /// <summary>
    /// Localized entity service interface
    /// </summary>
    public partial interface ILocalizedEntityService
    {
        /// <summary>
        /// Deletes a localized property
        /// </summary>
        /// <param name="localizedPropertyId">Localized property id</param>
        void DeleteLocalizedProperty(ObjectId localizedPropertyId);

        /// <summary>
        /// Gets a localized property
        /// </summary>
        /// <param name="localizedPropertyId">Localized property identifier</param>
        /// <returns>Localized property</returns>
        LocalizedProperty GetLocalizedPropertyById(ObjectId localizedPropertyId);

        /// <summary>
        /// Find localized value
        /// </summary>
        /// <param name="languageId">Language identifier. Null if using the default language</param>
        /// <param name="entityId">Entity identifier</param>
        /// <param name="localeKeyGroup">Locale key group</param>
        /// <param name="localeKey">Locale key</param>
        /// <returns>Found localized value</returns>
        string GetLocalizedValue(ObjectId entityId, string localeKeyGroup, string localeKey, ObjectId? languageId = null);

        /// <summary>
        /// Gets localized properties
        /// </summary>
        /// <param name="entityId">Entity identifier</param>
        /// <param name="localeKeyGroup">Locale key group</param>
        /// <returns>Localized properties</returns>
        NamedCollection<LocalizedProperty> GetLocalizedProperties(ObjectId entityId, string localeKeyGroup);

        /// <summary>
        /// Gets localized properties
        /// </summary>
        /// <param name="entityId">Entity identifier</param>
        /// <returns>Localized properties</returns>
        NamedCollection<LocalizedProperty> GetLocalizedProperties<T>(ObjectId entityId) where T:BaseEntity<ObjectId>, ILocalizedEntity;

        /// <summary>
        /// Gets localized properties
        /// </summary>
        /// <param name="entity">entity</param>
        /// <returns>Localized properties</returns>
        NamedCollection<LocalizedProperty> GetLocalizedProperties<T>(T entity) where T : BaseEntity<ObjectId>, ILocalizedEntity;

        /// <summary>
        /// Inserts a localized property
        /// </summary>
        /// <param name="localizedProperty">Localized property</param>
        void InsertLocalizedProperty(LocalizedProperty localizedProperty);

        /// <summary>
        /// Updates the localized property
        /// </summary>
        /// <param name="localizedProperty">Localized property</param>
        void UpdateLocalizedProperty(LocalizedProperty localizedProperty);

        /// <summary>
        /// Save localized value
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="keySelector">Ley selector</param>
        /// <param name="localeValue">Locale value</param>
        /// <param name="languageId">Language ID</param>
        void SaveLocalizedValue<T>(T entity,
            Expression<Func<T, string>> keySelector,
            string localeValue,
            ObjectId languageId) where T : BaseEntity<ObjectId>, ILocalizedEntity;

        void SaveLocalizedValue<T, TPropType>(T entity,
           Expression<Func<T, TPropType>> keySelector,
           TPropType localeValue,
           ObjectId languageId) where T : BaseEntity<ObjectId>, ILocalizedEntity;
    }
}
