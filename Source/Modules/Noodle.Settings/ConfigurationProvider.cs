using System;
using System.Linq;
using Noodle.Web;

namespace Noodle.Settings
{
    public class ConfigurationProvider<TSettings> : IConfigurationProvider<TSettings>, IDisposable where TSettings : ISettings, new()
    {
        readonly ISettingService _settingService;
        private readonly IErrorNotifier _errorNotifier;

        public ConfigurationProvider(ISettingService settingService, 
            IErrorNotifier errorNotifier)
        {
            _settingService = settingService;
            _errorNotifier = errorNotifier;

            // assigned events so we can update our object if needed
            _settingService.CachedCleared += SettingServiceOnCachedCleared;

            BuildConfiguration();
        }

        public TSettings Settings { get; protected set; }

        private void BuildConfiguration()
        {
            UpdateSettings();
        }

        private void UpdateSettings()
        {
            Settings = Activator.CreateInstance<TSettings>();
            foreach (var property in typeof(TSettings).GetProperties()
                .Where(x => x.CanWrite
                && x.CanRead
                && CommonHelper.GetCustomTypeConverter(x.PropertyType).CanConvertFrom(typeof(string))))
            {
                var key = string.Join(".", new[] { typeof(TSettings).Name, property.Name });
                var setting = _settingService.GetSettingByKey<string>(key);
                if (setting != null)
                {
                    try
                    {
                        var value = CommonHelper.GetCustomTypeConverter(property.PropertyType).ConvertFromInvariantString(setting);
                        property.SetValue(Settings, value, null);
                    }
                    catch (Exception ex)
                    {
                        _errorNotifier.Notify(ex);
                    }
                }
            }
        }

        public void SaveSettings(TSettings settings)
        {
            var properties = from prop in typeof(TSettings).GetProperties()
                             where prop.CanWrite && prop.CanRead
                             where CommonHelper.GetCustomTypeConverter(prop.PropertyType).CanConvertFrom(typeof(string))
                             select prop;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            foreach (var prop in properties)
            {
                string key = typeof(TSettings).Name + "." + prop.Name;
                //Duck typing is not supported in C#. That's why we're using dynamic type
                object value = prop.GetValue(settings, null);
                if (value != null)
                    _settingService.SetSetting(key, value, null, false);
                else
                    _settingService.SetSetting(key, "", null, false);
            }

            //and now clear cache
            _settingService.ClearCache();

            Settings = settings;
        }

        private void SettingServiceOnCachedCleared(object sender, EventArgs eventArgs)
        {
            UpdateSettings();
        }

        public void Dispose()
        {
            _settingService.CachedCleared -= SettingServiceOnCachedCleared;
        }
    }
}
