using Noodle.Collections;

namespace Noodle.Localization
{
    /// <summary>
    /// Represents a localized property
    /// </summary>
    public partial class LocalizedProperty : BaseEntity, INameable
    {
        /// <summary>
        /// Gets or sets the entity identifier
        /// </summary>
        public virtual int EntityId { get; set; }

        /// <summary>
        /// Gets or sets the language identifier
        /// </summary>
        public virtual int? LanguageId { get; set; }

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
        /// Small hack for linq to sql
        /// </summary>
        public new int Id
        {
            get { return base.Id; }
            set { base.Id = value; }
        }

        /// <summary>
        /// Helper for a name collection
        /// </summary>
        string INameable.Name
        {
            get { return LocaleKey; }
        }
    }
}
