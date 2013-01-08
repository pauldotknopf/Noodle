using System;
using System.Configuration;

namespace Noodle.Configuration
{
    public class ConfigurationManagerWrapper
    {
        readonly string _sectionGroup;

        public ConfigurationManagerWrapper()
            : this("noodle")
        {
        }
        public ConfigurationManagerWrapper(string sectionGroup)
        {
            _sectionGroup = sectionGroup;
        }

        public virtual T GetSection<T>(string sectionName) where T : ConfigurationSection
        {
            var section = ConfigurationManager.GetSection(_sectionGroup + "/" + sectionName);
            if (section == null) throw new ConfigurationErrorsException("Missing configuration section at '" + sectionName + "'");
            var contentSection = section as T;
            if (contentSection == null) throw new ConfigurationErrorsException("The configuration section at '" + sectionName + "' is of type '" + section.GetType().FullName + "' instead of '" + typeof(T).FullName + "' which is required.");
            return contentSection;
        }

        public virtual T GetSectionGroup<T>(string sectionGroup) where T:ConfigurationSectionGroup
        {
            var group = ConfigurationManager.GetSection(sectionGroup);
            
            if(group == null)
                throw new Exception("No section group '" + sectionGroup + "' has been defined.");

            if(!(group is T))
                throw new Exception("Section group '" + sectionGroup + "' exists but it is not of type '" + typeof(T).Name + "'.");

            return group as T;
        }

        public virtual ConnectionStringSettingsCollection GetConnectionStrings()
        {
            return ConfigurationManager.ConnectionStrings;
        }
    }
}
