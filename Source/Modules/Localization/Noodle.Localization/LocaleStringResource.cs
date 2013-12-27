using MongoDB.Bson;
using Noodle.Collections;

namespace Noodle.Localization
{
    /// <summary>
    /// Represents a locale string resource
    /// </summary>
    public partial class LocaleStringResource : BaseEntity<ObjectId>, INameable
    {
        /// <summary>
        /// Gets or sets the language identifier
        /// </summary>
        public virtual ObjectId LanguageId { get; set; }

        /// <summary>
        /// Gets or sets the resource name
        /// </summary>
        public virtual string ResourceName { get; set; }

        /// <summary>
        /// Gets or sets the resource value
        /// </summary>
        public virtual string ResourceValue { get; set; }

        /// <summary>
        /// This is for a named collection
        /// </summary>
        string INameable.Name
        {
            get { return ResourceName; }
        }
    }
}
