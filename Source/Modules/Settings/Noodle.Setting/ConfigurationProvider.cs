using System;
using System.Linq;

namespace Noodle.Settings
{
    public class ConfigurationProvider<TSettings> : IConfigurationProvider<TSettings>, IDisposable where TSettings : ISettings, new()
    {
        readonly ISettingService _settingService;

        public ConfigurationProvider(ISettingService settingService)
        {
            _settingService = settingService;

            // assigned events so we can update our object if needed
            _settingService.CachedCleared += SettingServiceOnCachedCleared;

            BuildConfiguration();
        }

        public TSettings Settings { get; protected set; }

        private void BuildConfiguration()
        {
            Settings = _settingService.GetSetting<TSettings>();
        }

        public void SaveSettings()
        {
            _settingService.SaveSetting(Settings);
        }

        private void SettingServiceOnCachedCleared(object sender, EventArgs eventArgs)
        {
            BuildConfiguration();
        }

        public void Dispose()
        {
            _settingService.CachedCleared -= SettingServiceOnCachedCleared;
        }
    }
}
