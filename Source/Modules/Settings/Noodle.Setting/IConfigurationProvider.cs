using System;

namespace Noodle.Settings
{
    /// <summary>
    /// Used to strongly type/reference a settings object for modification and saving
    /// </summary>
    /// <typeparam name="TSettings"></typeparam>
    public interface IConfigurationProvider<TSettings> where TSettings : ISettings, new()
    {
        /// <summary>
        /// The instance to the settings object
        /// </summary>
        TSettings Settings { get; }

        /// <summary>
        /// Save this settings object to the database
        /// </summary>
        void SaveSettings();

        /// <summary>
        /// Raised when someone calls "SaveSettings", or when someone clears the cache
        /// </summary>
        event EventHandler SettingsChanged;
    }
}
