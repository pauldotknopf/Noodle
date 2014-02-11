using System;
using MongoDB.Bson;
using Noodle.Security.Users;

namespace Noodle.Security.Activity
{
    /// <summary>
    /// Represents an activity log record
    /// </summary>
    public class ActivityLog : BaseEntity<ObjectId>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the activity log type identifier
        /// </summary>
        public virtual ObjectId? ActivityLogTypeId { get; set; }

        /// <summary>
        /// Gets or sets the user identifier
        /// </summary>
        public virtual ObjectId? UserId { get; set; }

        /// <summary>
        /// Gets or sets the activity comment
        /// </summary>
        public virtual string Comment { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public virtual DateTime CreatedOnUtc { get; set; }

        #endregion
    }
}
