using System.Configuration;

namespace Noodle.Configuration
{
    public class ConnectionStringCollection : LazyRemovableCollection<ConnectionStringElement>
    {
        /// <summary>
        /// The default connection string to return if someone asks for a specific one that doesn't exist.
        /// </summary>
        [ConfigurationProperty("default", DefaultValue = "Noodle")]
        public string DefaultConnectionStringName
        {
            get { return (string)base["default"]; }
            set { base["default"] = value; }
        }
    }
}
