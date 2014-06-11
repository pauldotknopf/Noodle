
namespace Noodle.Localization
{
    /// <summary>
    /// Represents a locale string resource
    /// </summary>
    public class KeyMapping
    {
        /// <summary>
        /// Gets or sets the resource name
        /// </summary>
        public virtual string MappingFrom { get; set; }

        /// <summary>
        /// Gets or sets the resource value
        /// </summary>
        public virtual string MappingTo { get; set; }
    }
}
