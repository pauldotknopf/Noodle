using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Ninject;
using Noodle.Caching;
using Noodle.Data;

namespace Noodle.Settings
{
    /// <summary>
    /// Setting manager
    /// </summary>
    /// <remarks></remarks>
    public partial class SettingService : ISettingService
    {
        #region Constants
        private const string SETTINGS_ALL_KEY = "Method.setting.all";
        #endregion

        #region Fields

        private readonly IKernel _kernel;
        private readonly MongoCollection<Setting> _settingsCollection;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">The cache manager.</param>
        /// <param name="kernel">The kernel.</param>
        /// <param name="settingsCollection">The mongo settings collection</param>
        /// <remarks></remarks>
        public SettingService(ICacheManager cacheManager, 
            IKernel kernel,
            MongoCollection<Setting> settingsCollection)
        {
            _cacheManager = cacheManager;
            _kernel = kernel;
            _settingsCollection = settingsCollection;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get setting value by key
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Setting value</returns>
        /// <remarks></remarks>
        public virtual T GetSettingByKey<T>(string key, T defaultValue = default(T))
        {
            if (string.IsNullOrEmpty(key))
                return defaultValue;

            var settings = GetAllSettings();
            if (settings.ContainsKey(key))
            {
                var setting = settings[key];
                return setting.As<T>();
            }
            return defaultValue;
        }

        /// <summary>
        /// Set setting value
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="settingId">The setting id to update the name/value. Use this when you want to rename a setting</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        /// <remarks></remarks>
        public virtual void SetSetting<T>(string key, T value, int? settingId = null, bool clearCache = true)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            key = key.Trim();

            var valueStr = CommonHelper.GetCustomTypeConverter(typeof(T)).ConvertToInvariantString(value).Trim();

            // upsert
            _settingsCollection.Update(Query.EQ("Name",new BsonString(key)),
                Update.Set("Value", new BsonString(valueStr)), 
                new MongoUpdateOptions { Flags = UpdateFlags.Upsert });

            if(clearCache)
                ClearCache();
        }

        /// <summary>
        /// Deletes a setting by key
        /// </summary>
        /// <param name="key">Key</param>
        /// <remarks></remarks>
        public virtual void DeleteSettingByKey(string key)
        {
            if (key.IsNullOrWhiteSpace())
                throw new ArgumentNullException("key");

            key = key.Trim();

            _settingsCollection.Remove(Query<Setting>.Where(s => s.Name.ToLower() == key.ToLower()));

            ClearCache();
        }

        /// <summary>
        /// Gets all settings
        /// </summary>
        /// <returns>Setting collection</returns>
        /// <remarks></remarks>
        public virtual IDictionary<string, Setting> GetAllSettings()
        {
            //cache
            return _cacheManager.Get(SETTINGS_ALL_KEY, () =>
            {
                var query = _settingsCollection
                    .FindAll()
                    .SetSortOrder(SortBy.Ascending("Name"));
                return query.ToList().ToDictionary(x => x.Name);
            });
        }

        /// <summary>
        /// Save settings object
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="settingInstance">Setting instance</param>
        /// <remarks></remarks>
        public virtual void SaveSetting<T>(T settingInstance) where T : ISettings, new()
        {
            //We should be sure that an appropriate ISettings object will not be cached in IoC tool after updating (by default cached per HTTP request)
            _kernel.Resolve<IConfigurationProvider<T>>().SaveSettings(settingInstance);
        }

        /// <summary>
        /// Clear cache
        /// </summary>
        /// <remarks></remarks>
        public virtual void ClearCache()
        {
            _cacheManager.RemoveByPattern(SETTINGS_ALL_KEY);
        }

        #endregion
    }
}
