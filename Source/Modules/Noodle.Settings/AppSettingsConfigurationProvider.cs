using System;
using System.Linq;
using Noodle.Configuration;

namespace Noodle.Settings
{
    public class AppSettingsConfigurationProvider<TSettings> : IConfigurationProvider<TSettings> where TSettings : ISettings, new()
    {
        private readonly AppSettings _appSettings;

        public AppSettingsConfigurationProvider(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public TSettings Settings
        {
            get
            {
                var settings = Activator.CreateInstance<TSettings>();

                foreach (var property in typeof(TSettings).GetProperties()
                    .Where(x => x.CanWrite
                    && x.CanRead
                    && CommonHelper.GetCustomTypeConverter(x.PropertyType).CanConvertFrom(typeof(string))))
                {
                    var key = string.Join(".", new[] { typeof(TSettings).Name, property.Name });
                    var setting = _appSettings[key];
                    if (setting != null)
                    {
                        try
                        {
                            var value = CommonHelper.GetCustomTypeConverter(property.PropertyType).ConvertFromInvariantString(setting);
                            property.SetValue(settings, value, null);
                        }
                        catch (Exception ex)
                        {
                            // TODO: Error notifier
                            //_logger.Error("There was a problem building a ISettings object. Could not set the '{0}' property with a property type of '{1}' and a value  of '{2}'.".F(key, property.PropertyType.Name, setting), ex);
                        }
                    }
                }

                return settings;
            }
        }

        public void SaveSettings(TSettings settings)
        {
            // not supported
        }
    }
}
