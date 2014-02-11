using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace Noodle.Security.Activity
{
    /// <summary>
    /// User activity service interface
    /// </summary>
    public interface IUserActivityService
    {
        /// <summary>
        /// Gets all activity log type items
        /// </summary>
        /// <returns>Activity log type collection</returns>
        /// <remarks></remarks>
        IList<ActivityLogType> GetAllActivityTypes();

        /// <summary>
        /// Inserts an activity log item
        /// </summary>
        /// <param name="systemKeyword">The system keyword</param>
        /// <param name="comment">The activity comment</param>
        /// <param name="userId">The user to associate the activity with. If none is used, it will be gotten from security context if available</param>
        /// <param name="commentParams">The activity comment parameters for string.Format() function.</param>
        /// <returns>Activity log item</returns>
        /// <remarks></remarks>
        void InsertActivity(string systemKeyword, string comment, ObjectId? userId = null, params object[] commentParams);

        /// <summary>
        /// Deletes an activity log item
        /// </summary>
        /// <param name="activityLogId">Activity log id to delete</param>
        /// <remarks></remarks>
        void DeleteActivity(ObjectId activityLogId);

        /// <summary>
        /// Disable an activity log type
        /// </summary>
        /// <param name="systemKeyword">The system keyword of the activity log type.</param>
        void DisableActivityLogType(string systemKeyword);

        /// <summary>
        /// Enabled an activity log type
        /// </summary>
        /// <param name="systemKeyword">The system keyword of the activity log type.</param>
        void EnabledActivityLogType(string systemKeyword);

        /// <summary>
        /// Gets all activity log items
        /// </summary>
        /// <param name="createdOnFromUtc">Log item creation from; null to load all users</param>
        /// <param name="createdOnToUtc">Log item creation to; null to load all users</param>
        /// <param name="email">User Email</param>
        /// <param name="username">User username</param>
        /// <param name="activityLogType">Activity log type system keyword</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Activity log collection</returns>
        /// <remarks></remarks>
        PagedList<ActivityLog> GetAllActivities(DateTime? createdOnFromUtc = null,
            DateTime? createdOnToUtc = null, string email = "", string username = "", string activityLogType = "",
            int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Install all activity types specified by the activity log type provider
        /// </summary>
        /// <param name="activityLogTypeProvider">The activity log type provider.</param>
        /// <param name="reInstall">Should the provider be reinstalled (add new types or update existing)</param>
        /// <remarks></remarks>
        void InstallActivityLogTypes(IActivityLogTypeProvider activityLogTypeProvider, bool reInstall = false);

        /// <summary>
        /// Uninstall the activity types specified by the activity log type provider
        /// </summary>
        /// <param name="activityLogTypeProvider">The activity log type provider.</param>
        /// <remarks></remarks>
        void UninstallActivityLogTypes(IActivityLogTypeProvider activityLogTypeProvider);

        /// <summary>
        /// Install all the activity log type providers found in the app domain
        /// </summary>
        /// <remarks></remarks>
        void InstallFoundActivityLogTypeProviders();

        /// <summary>
        /// Clears activity log
        /// </summary>
        /// <remarks></remarks>
        void ClearAllActivities();
    }
}
