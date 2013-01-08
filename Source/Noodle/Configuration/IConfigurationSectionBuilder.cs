using System.Configuration;

namespace Noodle.Configuration
{
    public interface IConfigurationSectionBuilder
    {
        TSection BuildSection<TSection>(string xml) where TSection : ConfigurationSection;
    }
}
