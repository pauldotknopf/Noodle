using System.Configuration;

namespace Noodle.Configuration
{
    /// <summary>
    /// The core noodle configuration section
    /// </summary>
    public class NoodleCoreConfiguration : ConfigurationSection
    {
        /// <summary>
        /// Add or remove plugin initializers. 
        /// This is most commonly used to remove automatic plugin initializers in an external assembly.
        /// </summary>
        [ConfigurationProperty("pluginInitializers")]
        public virtual PluginCollection PluginInitializers
        {
            get { return (PluginCollection)base["pluginInitializers"]; }
            set { base["pluginInitializers"] = value; }
        }

        /// <summary>
        /// Add or remove plugins. 
        /// This is most commonly used to remove automatic plugins in an external assembly.
        /// </summary>
        [ConfigurationProperty("plugins")]
        public virtual PluginCollection Plugins
        {
            get { return (PluginCollection)base["plugins"]; }
            set { base["plugins"] = value; }
        }

        /// <summary>
        /// Scheduler related configuration.
        /// </summary>
        [ConfigurationProperty("scheduler")]
        public virtual SchedulerElement Scheduler
        {
            get { return (SchedulerElement)base["scheduler"]; }
            set { base["scheduler"] = value; }
        }

        /// <summary>
        /// The connection string pointers
        /// </summary>
        [ConfigurationProperty("connectionStrings")]
        public virtual ConnectionStringCollection ConnectionStrings
        {
            get { return (ConnectionStringCollection)base["connectionStrings"]; }
            set { base["connectionStrings"] = value; }
        }
    }
}
