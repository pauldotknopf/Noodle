using MongoDB.Bson;
using Noodle.Security.Users;

namespace Noodle.Security.Permissions
{
    /// <summary>
    /// Used to map a permission to a role
    /// </summary>
    public class RolePermissionMap : BaseEntity<ObjectId>
    {
        /// <summary>
        /// Permission record id
        /// </summary>
        public ObjectId PermissionRecordId { get; set; }

        /// <summary>
        /// User role id
        /// </summary>
        public ObjectId UserRoleId { get; set; }
    }
}
