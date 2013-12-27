using System.Collections.Generic;
using Noodle.Security.Activity;

namespace Noodle.Security.Tests.Fakes
{
    public class FakeActivityLogTypeProvider : IActivityLogTypeProvider
    {
        private readonly IList<ActivityLogType> _activityLogTypes;

        public FakeActivityLogTypeProvider(IList<ActivityLogType> activityLogTypes)
        {
            _activityLogTypes = activityLogTypes;
        }

        public IEnumerable<ActivityLogType> GetActivityLogTypes()
        {
            return _activityLogTypes;
        }
    }
}
