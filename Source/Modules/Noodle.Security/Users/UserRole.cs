using System.Collections.Generic;
using MongoDB.Bson;
using Noodle.Security.Permissions;

namespace Noodle.Security.Users
{
    /// <summary>
    /// Represents a user role
    /// </summary>
    public class UserRole : BaseEntity<ObjectId>
    {
        /// <summary>
        /// Gets or sets the customer role name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer role is active
        /// </summary>
        public virtual bool Active { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer role is system
        /// </summary>
        public virtual bool IsSystemRole { get; set; }

        /// <summary>
        /// Gets or sets the customer role system name
        /// </summary>
        public virtual string SystemName { get; set; }
    }

}