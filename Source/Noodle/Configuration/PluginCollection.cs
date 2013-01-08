using System.Configuration;
using System.Xml;

namespace Noodle.Configuration
{
    /// <summary>
    /// A configurable collection of plugins.
    /// </summary>
    [ConfigurationCollection(typeof(PluginElement))]
    public class PluginCollection : LazyRemovableCollection<PluginElement>
    {
        protected override void OnDeserializeRemoveElement(PluginElement element, XmlReader reader)
        {
            element.Type = reader.GetAttribute("type");
        }
    }
}
