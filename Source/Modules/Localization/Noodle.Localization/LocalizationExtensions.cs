using System;
using System.Linq.Expressions;
using System.Reflection;
using MongoDB.Bson;
using Noodle.Localization.Services;

namespace Noodle.Localization
{
    /// <summary>
    /// Some extensions for localization services
    /// </summary>
    public static class LocalizationExtensions
    {
        /// <summary>
        /// Get localized value of enum
        /// </summary>
        /// <typeparam name="T">Enum</typeparam>
        /// <param name="enumValue">Enum value</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Localized value</returns>
        public static string GetLocalizedEnum<T>(this ILocalizationService localizationService, T enumValue, ObjectId? languageId = null)
            where T : struct
        {
            if (!typeof(T).IsEnum) throw new ArgumentException("T must be an enumerated type");

            //localized value
            string resourceName = string.Format("Enums.{0}.{1}",typeof(T),enumValue.ToString());
            string result = localizationService.GetResource(resourceName, languageId, false, CommonHelper.PascalCamelToFriendly(enumValue.ToString()));

            return result;
        }

        /// <summary>
        /// Find localized value
        /// </summary>
        /// <typeparam name="T">The entity type</typeparam>
        /// <param name="localizedEntityService">The localized entity service</param>
        /// <param name="entityId">The entity id</param>
        /// <param name="keySelector">The key selector to get a localized value for</param>?
        /// <param name="languageId">The language to look for</param>
        /// <returns></returns>
        public static string GetLocalizedValue<T>(this ILocalizedEntityService localizedEntityService, ObjectId entityId, Expression<Func<T, string>> keySelector, ObjectId? languageId = null)
        {
            return localizedEntityService.GetLocalizedValue<T, string>(entityId, keySelector, languageId);
        }

        /// <summary>
        /// Find localized value
        /// </summary>
        /// <typeparam name="T">The entity type</typeparam>
        /// <typeparam name="TPropType">The property type of the key</typeparam>
        /// <param name="localizedEntityService">the localized entity service</param>
        /// <param name="entityId">The entity id</param>
        /// <param name="keySelector">The key selector to get a localized value for</param>
        /// <param name="languageId">The language to look for</param>
        /// <returns></returns>
        public static string GetLocalizedValue<T, TPropType>(this ILocalizedEntityService localizedEntityService, ObjectId entityId, Expression<Func<T, TPropType>> keySelector, ObjectId? languageId = null)
        {
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

            return localizedEntityService.GetLocalizedValue(entityId, typeof(T).Name, propInfo.Name, languageId);
        }
    }
}
