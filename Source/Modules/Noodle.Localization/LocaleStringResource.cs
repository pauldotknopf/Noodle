using Noodle.Collections;

namespace Noodle.Localization
{
    /// <summary>
    /// Represents a locale string resource
    /// </summary>
    public partial class LocaleStringResource : BaseEntity, INameable
    {
        /// <summary>
        /// Gets or sets the language identifier
        /// </summary>
        public virtual int? LanguageId { get; set; }

        /// <summary>
        /// Gets or sets the resource name
        /// </summary>
        public virtual string ResourceName { get; set; }

        /// <summary>
        /// Gets or sets the resource value
        /// </summary>
        public virtual string ResourceValue { get; set; }

        /// <summary>
        /// Gets or sets the language
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
        /// This is for a named collection
        /// </summary>
        string Collections.INameable.Name
        {
            get { return ResourceName; }
        }
    }
}
