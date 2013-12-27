using MongoDB.Bson;

namespace Noodle.Security.Activity
{
    /// <summary>
    /// Represents an activity log type provider that has been installed
    /// </summary>
    public class ActivityLogTypeProviderInstalled : BaseEntity<ObjectId>
    {
        /// <summary>
        /// The name of the provider installed
        /// </summary>
        public string Name { get; set; }
    }
}
