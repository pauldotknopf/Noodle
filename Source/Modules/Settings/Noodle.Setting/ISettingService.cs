using System;
using System.Collections.Generic;

namespace Noodle.Settings
{
    /// <summary>
    /// Setting service interface
    /// </summary>
    public interface ISettingService
    {
        /// <summary>
        /// Get setting value by key
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Setting value</returns>
        T GetSettingByKey<T>(string key, T defaultValue = default(T));

        /// <summary>
        /// Set setting value
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="settingId">The setting id to update the name/value. Use this when you want to rename a setting</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        void SetSetting<T>(string key, T value, int? settingId = null, bool clearCache = true);

        /// <summary>
        /// Deletes a setting by key
        /// </summary>
        /// <param name="key">Key</param>
        void DeleteSettingByKey(string key);

        /// <summary>
        /// Gets all settings
        /// </summary>
        /// <returns>Setting collection</returns>
        IDictionary<string, Setting> GetAllSettings();

        /// <summary>
        /// Save settings object
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="settingInstance">Setting instance</param>
        void SaveSetting<T>(T settingInstance) where T : ISettings, new();

        /// <summary>
        /// Get a settings object from the database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        T GetSetting<T>() where T : ISettings, new();

        /// <summary>
        /// Clear cache
        /// </summary>
        void ClearCache();

        /// <summary>
        /// This event gets fire when the cache has been cleared so that the IConfigurationProvider can update there instances
        /// </summary>
        event EventHandler<EventArgs> CachedCleared;
    }
}
