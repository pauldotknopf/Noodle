using System;
using System.Reflection;
using Ninject.Activation;
using Noodle.Configuration;

namespace Noodle.Settings
{
    public class SettingsProvider : IProvider
    {
        private readonly Type _type;

        static readonly MethodInfo BuildSettingsMethod = typeof(SettingsProvider).GetMethod(
            "BuildSettings",
            BindingFlags.Static | BindingFlags.NonPublic);

        static TSettings BuildSettings<TSettings>(IContext context) where TSettings : ISettings, new()
        {
            try
            {
                return context.Kernel.Resolve<IConfigurationProvider<TSettings>>().Settings;
            }
            catch (NoodleException ex)
            {
                // we want to avoid errors if database/connection strings haven't been setup
                if (!ex.Message.StartsWith("The default connection string name was"))
                    throw;

                return new AppSettingsConfigurationProvider<TSettings>(context.Kernel.Resolve<AppSettings>()).Settings;
            }
        }

        public SettingsProvider(Type type)
        {
            _type = type;
        }

        public object Create(IContext context)
        {
            var buildMethod = BuildSettingsMethod.MakeGenericMethod(_type);
            return buildMethod.Invoke(null, new object[] { context });
        }

        public Type Type
        {
            get { return _type; }
        }
    }
}
