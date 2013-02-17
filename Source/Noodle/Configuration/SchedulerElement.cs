using System.Configuration;

namespace Noodle.Configuration
{
    public class SchedulerElement : ConfigurationElement
    {
        [ConfigurationProperty("enabled", DefaultValue = true)]
        public bool Enabled
        {
            get { return (bool)base["enabled"]; }
            set { base["enabled"] = value; }
        }

        [ConfigurationProperty("interval", DefaultValue = 60)]
        public int Interval
        {
            get { return (int)base["interval"]; }
            set { base["interval"] = value; }
        }
    }
}
