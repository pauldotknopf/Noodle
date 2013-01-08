using System.Configuration;

namespace Noodle.Configuration
{
    /// <summary>
    /// A connection string (pointer) element
    /// </summary>
    public class ConnectionStringElement : NamedElement
    {
        /// <summary>
        /// The name of the connection string this pointer is refering to.
        /// </summary>
        [ConfigurationProperty("connectionStringName", IsRequired=true)]
        public string ConnectionStringName
        {
            get { return (string)base["connectionStringName"]; }
            set { base["connectionStringName"] = value; }
        }
    }
}
