using MongoDB.Bson;
using Noodle.Extensions.Collections;

namespace Noodle.Localization
{
    /// <summary>
    /// Represents a localized property
    /// </summary>
    public partial class LocalizedProperty : BaseEntity<ObjectId>, INameable
    {
        /// <summary>
        /// Gets or sets the entity identifier
        /// </summary>
        public virtual ObjectId EntityId { get; set; }

        /// <summary>
        /// Gets or sets the language identifier
        /// </summary>
        public virtual ObjectId LanguageId { get; set; }

        /// <summary>
        /// Gets or sets the locale key group
        /// </summary>
        public virtual string LocaleKeyGroup { get; set; }

        /// <summary>
        /// Gets or sets the locale key
        /// </summary>
        public virtual string LocaleKey { get; set; }

        /// <summary>
        /// Gets or sets the locale value
        /// </summary>
        public virtual string LocaleValue { get; set; }

        /// <summary>
        /// Gets the language
        /// </summary>
        public virtual Language Language { get; set; }

        /// <summary>
        /// Helper for a name collection
        /// </summary>
        string INameable.Name
        {
            get { return LocaleKey; }
        }
    }
}
