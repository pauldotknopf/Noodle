using System.Collections.Generic;

namespace Noodle.Security.Activity
{
    public interface IActivityLogTypeProvider
    {
        IEnumerable<ActivityLogType> GetActivityLogTypes();
    }
}
