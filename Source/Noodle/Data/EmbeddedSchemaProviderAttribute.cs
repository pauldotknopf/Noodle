using Noodle.Plugins;

namespace Noodle.Data
{
    /// <summary>
    /// Embedded schema providers decorated with this can be discovered
    /// </summary>
    public class EmbeddedSchemaProviderAttribute : BasePluginAttribute
    {
        private string _displayName = string.Empty;
        public string DisplayName
        {
            get
            {
                return string.IsNullOrEmpty(_displayName) ? Name : _displayName;
            }
            set
            {
                _displayName = value;
            }
        }
    }
}
