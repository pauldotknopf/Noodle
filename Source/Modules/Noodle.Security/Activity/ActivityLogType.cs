using System.Collections.Generic;
using MongoDB.Bson;

namespace Noodle.Security.Activity
{
    /// <summary>
    /// Represents an activity log type record
    /// </summary>
    public class ActivityLogType : BaseEntity<ObjectId>
    {
        public ActivityLogType()
        {
            Enabled = true;
        }

        #region Properties

        /// <summary>
        /// Gets or sets the system keyword
        /// </summary>
        public virtual string SystemKeyword { get; set; }

        /// <summary>
        /// Gets or sets the display name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the activity log type is enabled
        /// </summary>
        public virtual bool Enabled { get; set; }

        #endregion
    }
}
