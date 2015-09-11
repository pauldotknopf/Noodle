using System;
using System.Linq;
using System.Security.Principal;
using Noodle.Extensions.Security;

namespace Noodle.Tests
{
    public class FakeSecurityManager : ISecurityManager
    {
        public bool IsInRole(IPrincipal user, string role)
        {
            var fakePrincipal = user as FakePrincipal;

            if (fakePrincipal == null)
                throw new InvalidOperationException("user must be a FakePrinciple");

            return fakePrincipal.IsInRole(role);
        }

        public bool IsAuthorized(IPrincipal user, string permission)
        {
            var fakePrincipal = user as FakePrincipal;

            if (fakePrincipal == null)
                throw new InvalidOperationException("user must be a FakePrinciple");

            return fakePrincipal.Permissions.Any(x => x.Equals(permission, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
