using System;
using System.Linq;

namespace Noodle.Settings
{
    /// <summary>
    /// See IConfigurationProvider
    /// </summary>
    /// <typeparam name="TSettings"></typeparam>
    public class ConfigurationProvider<TSettings> : IConfigurationProvider<TSettings>, IDisposable where TSettings : ISettings, new()
    {
        #region Fields

        readonly ISettingService _settingService;

        #endregion

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationProvider{TSettings}"/> class.
        /// </summary>
        /// <param name="settingService">The setting service.</param>
        public ConfigurationProvider(ISettingService settingService)
        {
            _settingService = settingService;

            // assigned events so we can update our object if needed
            _settingService.CachedCleared += OnSettingsCachedCleared;

            BuildConfiguration();
        }

        #endregion

        #region IConfigurationProvider

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        /// <value>
        /// The settings.
        /// </value>
        public TSettings Settings { get; protected set; }

        /// <summary>
        /// Builds the configuration.
        /// </summary>
        private void BuildConfiguration()
        {
            Settings = _settingService.GetSetting<TSettings>();
        }

        /// <summary>
        /// Saves the settings.
        /// </summary>
        public void SaveSettings()
        {
            _settingService.SaveSetting(Settings);

            var settingsChangedHandler = SettingsChanged;
            if (settingsChangedHandler != null)
                settingsChangedHandler(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when [settings changed].
        /// </summary>
        public event EventHandler SettingsChanged;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _settingService.CachedCleared -= OnSettingsCachedCleared;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Raised when someone has clear the settings cache
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void OnSettingsCachedCleared(object sender, EventArgs eventArgs)
        {
            BuildConfiguration();

            var settingsChangedHandler = SettingsChanged;
            if (settingsChangedHandler != null)
                settingsChangedHandler(this, EventArgs.Empty);
        }

        #endregion
    }
}
