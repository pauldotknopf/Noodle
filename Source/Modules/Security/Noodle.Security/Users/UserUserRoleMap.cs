using MongoDB.Bson;

namespace Noodle.Security.Users
{
    /// <summary>
    /// Maps a user to a user role
    /// </summary>
    public class UserUserRoleMap : BaseEntity<ObjectId>
    {
        /// <summary>
        /// User id
        /// </summary>
        public ObjectId UserId { get; set; }

        /// <summary>
        /// Role id
        /// </summary>
        public ObjectId UserRoleId { get; set; }
    }
}
