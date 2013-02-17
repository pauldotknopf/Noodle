using System.Configuration;

namespace Noodle.Configuration
{
    /// <summary>
    /// Builds a configuration section from xml
    /// </summary>
    public interface IConfigurationSectionBuilder
    {
        /// <summary>
        /// Build a configuration section from the given xml. This is useful for storing configuration in a database somewhere.
        /// </summary>
        /// <typeparam name="TSection"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        TSection BuildSection<TSection>(string xml) where TSection : ConfigurationSection;
    }
}
