using MongoDB.Bson;

namespace Noodle.Settings
{
    /// <summary>
    /// Represents a setting
    /// </summary>
    public partial class Setting : BaseEntity<ObjectId>
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public virtual string Value { get; set; }

        /// <summary>
        /// Returns the setting value as the specified type
        /// </summary>
        public virtual T As<T>()
        {
            return CommonHelper.To<T>(this.Value);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
