using Noodle.Security.Users;

namespace Noodle.Security.Activity
{
    public static class ActivityLogExtensions
    {
        #region Users

        public static void AddedUser(this IUserActivityService userActivityService, User newUser)
        {
            userActivityService.InsertActivity(DefaultActivityLogTypesProvider.AddUser.SystemKeyword, "Added a new user (ID = {0})", null, newUser.Id);
        }

        public static void EditedUser(this IUserActivityService userActivityService, User editedUser)
        {
            userActivityService.InsertActivity(DefaultActivityLogTypesProvider.EditUser.SystemKeyword, "Edited a user (ID = {0})",
                                               null, editedUser.Id);
        }

        public static void DeletedUser(this IUserActivityService userActivityService, User deletedUser)
        {
            userActivityService.InsertActivity(DefaultActivityLogTypesProvider.DeleteUser.SystemKeyword, "Deleted a user (ID = {0})", null, deletedUser.Id);
        }

        #endregion
    }
}
