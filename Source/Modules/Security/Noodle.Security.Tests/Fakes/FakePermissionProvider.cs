using System.Collections.Generic;
using Noodle.Security.Permissions;

namespace Noodle.Security.Tests.Fakes
{
    public class FakePermissionProvider : IPermissionProvider
    {
        private readonly IList<PermissionRecord> _permissionRecords;
        private readonly IList<DefaultPermissionRecord> _defaults;

        public FakePermissionProvider(IList<PermissionRecord> permissionRecords, IList<DefaultPermissionRecord> defaults)
        {
            _permissionRecords = permissionRecords;
            _defaults = defaults;
        }

        public IEnumerable<PermissionRecord> GetPermissions()
        {
            return _permissionRecords;
        }

        public IEnumerable<DefaultPermissionRecord> GetDefaultPermissions()
        {
            return _defaults;
        }
    }
}
