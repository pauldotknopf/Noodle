using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Noodle.Caching;

namespace Noodle.Settings
{
    /// <summary>
    /// Setting manager
    /// </summary>
    public partial class SettingService : ISettingService
    {
        #region Constants
        private const string SETTINGS_ALL_KEY = "Noodle.setting.all";
        #endregion

        #region Fields

        private readonly TinyIoCContainer _container;
        private readonly MongoCollection<Setting> _settingsCollection;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">The cache manager.</param>
        /// <param name="container">The container.</param>
        /// <param name="settingsCollection">The mongo settings collection</param>
        /// <remarks></remarks>
        public SettingService(ICacheManager cacheManager, 
            TinyIoCContainer container,
            MongoCollection<Setting> settingsCollection)
        {
            _cacheManager = cacheManager;
            _container = container;
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
            var name = typeof(T).FullName + ", " + typeof(T).Assembly.GetName().Name;
            var existing = _settingsCollection.FindOne(Query<Setting>.EQ(x => x.Name, name)) as TypedSettings<T>;

            if(existing == null)
            {
                var typedSetting = new TypedSettings<T>();
                typedSetting.Id = ObjectId.GenerateNewId();
                typedSetting.Settings = settingInstance;
                typedSetting.Name = name;
                _settingsCollection.Insert(typedSetting); 
            }else
            {
                existing.Settings = settingInstance;
                _settingsCollection.Update(Query.EQ("_id", existing.Id), Update.Set("Settings", settingInstance.ToBsonDocument()));
            }

            ClearCache();
        }

        /// <summary>
        /// Get a settings object from the database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public T GetSetting<T>() where T : ISettings, new()
        {
            var name = typeof(T).FullName + ", " + typeof(T).Assembly.GetName().Name;

            var settings = _settingsCollection.FindOne(Query<Setting>.EQ(x => x.Name, name));

            if (settings == null)
                return new T();

            if(!(settings is TypedSettings<T>))
                return new T();

            return (settings as TypedSettings<T>).Settings;
        }

        /// <summary>
        /// Clear cache
        /// </summary>
        /// <remarks></remarks>
        public virtual void ClearCache()
        {
            _cacheManager.RemoveByPattern(SETTINGS_ALL_KEY);
            if (CachedCleared != null)
                CachedCleared(this, EventArgs.Empty);
        }

        /// <summary>
        /// This event gets fire when the cache has been cleared so that the IConfigurationProvider can update there instances
        /// </summary>
        public event EventHandler<EventArgs> CachedCleared;

        #endregion 
    }
}
