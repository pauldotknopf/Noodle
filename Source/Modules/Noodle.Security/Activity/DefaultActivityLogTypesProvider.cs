using System.Collections.Generic;

namespace Noodle.Security.Activity
{
    public class DefaultActivityLogTypesProvider : IActivityLogTypeProvider
    {
        public static ActivityLogType EditUser = new ActivityLogType {Name = "Edit user", SystemKeyword = "EditUser"};
        public static ActivityLogType AddUser = new ActivityLogType { Name = "Add user", SystemKeyword = "AddUser" };
        public static ActivityLogType DeleteUser = new ActivityLogType { Name = "Delete user", SystemKeyword = "DeeteUser" };

        public IEnumerable<ActivityLogType> GetActivityLogTypes()
        {
            return new[]
            {
                EditUser,
                AddUser,
                DeleteUser
            };
        }
    }
}
