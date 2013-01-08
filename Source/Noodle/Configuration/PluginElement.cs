using System.Configuration;

namespace Noodle.Configuration
{
    public class PluginElement : NamedElement
    {
        [ConfigurationProperty("type", IsRequired = true)]
        public string Type
        {
            get { return (string)base["type"]; }
            set { base["type"] = value; }
        }
    }
}
