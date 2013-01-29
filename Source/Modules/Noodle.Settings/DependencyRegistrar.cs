using System.Reflection;
using MongoDB.Driver;
using Noodle.Configuration;
using Noodle.Data;
using Noodle.Engine;
using Noodle.MongoDB;
using SimpleInjector;
using SimpleInjector.Extensions;
namespace Noodle.Settings
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(Container container)
        {
            container.RegisterSingle<ISettingService, SettingService>();
            container.RegisterOpenGeneric(typeof (IConfigurationProvider<>), typeof (ConfigurationProvider<>));
            container.RegisterSingle(() => GetSettingsDatabase(container).GetCollection<Setting>("Settings"));
            container.ResolveUnregisteredType += (sender, e) =>
            {
                var type = e.UnregisteredServiceType;
                if (typeof(ISettings).IsAssignableFrom(type))
                {
                    e.Register(() =>
                    {
                        var buildMethod = BuildSettingsMethod.MakeGenericMethod(type);
                        return buildMethod.Invoke(null, new object[] { container });
                    });
                }
            };
        }

        public int Importance
        {
            get { return 0; }
        }

        public static MongoDatabase GetSettingsDatabase(Container container)
        {
            return container.GetInstance<IMongoService>().GetDatabase("Localization");
        }

        static readonly MethodInfo BuildSettingsMethod = typeof(DependencyRegistrar).GetMethod(
            "BuildSettings",
            BindingFlags.Static | BindingFlags.NonPublic);

        static TSettings BuildSettings<TSettings>(Container container) where TSettings : ISettings, new()
        {
            try
            {
                return container.GetInstance<IConfigurationProvider<TSettings>>().Settings;
            }
            catch (NoodleException ex)
            {
                // we want to avoid errors if database/connection strings haven't been setup
                if (!ex.Message.StartsWith("The default connection string name was"))
                    throw;

                return new AppSettingsConfigurationProvider<TSettings>(container.GetInstance<AppSettings>()).Settings;
            }
        }
    }
}
