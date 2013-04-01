using MongoDB.Bson;

namespace Noodle.Security.Permissions
{
    /// <summary>
    /// Represents a permission provider that was installed into the database
    /// </summary>
    public class PermissionInstalled : BaseEntity<ObjectId>
    {
        /// <summary>
        /// The name of the permission provider that has been installed
        /// </summary>
        public virtual string Name { get; set; }
    }
}
