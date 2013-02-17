using System;
using System.Configuration;

namespace Noodle.Configuration
{
    /// <summary>
    /// Manages getting configuration sections for a group
    /// </summary>
    public class ConfigurationGroupManager
    {
        readonly string _sectionGroup;

        public ConfigurationGroupManager()
            : this("noodle")
        {
        }
        public ConfigurationGroupManager(string sectionGroup)
        {
            _sectionGroup = sectionGroup;
        }

        public virtual T GetSection<T>(string sectionName, bool createIfNotExists = false) where T : ConfigurationSection
        {
            var section = ConfigurationManager.GetSection(_sectionGroup + "/" + sectionName);
            if (section == null && !createIfNotExists) throw new ConfigurationErrorsException("Missing configuration section at '" + sectionName + "'");
            if (section == null) section = Activator.CreateInstance(typeof(T));
            var contentSection = section as T;
            if (contentSection == null) throw new ConfigurationErrorsException("The configuration section at '" + sectionName + "' is of type '" + section.GetType().FullName + "' instead of '" + typeof(T).FullName + "' which is required.");
            return contentSection;
        }
    }
}
