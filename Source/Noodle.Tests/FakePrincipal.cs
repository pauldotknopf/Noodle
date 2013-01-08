using System;
using System.Linq;
using System.Security.Principal;

namespace Noodle.Tests
{
    public class FakePrincipal : IPrincipal
    {
        private readonly string[] _roles = new string[0];
        private readonly string[] _permissions = new string[0];
        private readonly IIdentity _identity;

        public FakePrincipal(bool isAuthenticated, string name = "", string[] roles = null, string[] permissions = null)
        {
            if (roles == null)
                roles = new string[0];

            if (permissions == null)
                permissions = new string[0];

            _roles = roles;
            _permissions = permissions;

            _identity = new FakeIdentity(isAuthenticated, name);
        }

        public string[] Roles
        {
            get { return _roles; }
        }

        public string[] Permissions
        {
            get { return _permissions; }
        }

        #region IPrincipal
        
        public IIdentity Identity
        {
            get { return _identity; }
        }

        public bool IsInRole(string role)
        {
            return Roles.Any(x => x.Equals(role, StringComparison.InvariantCultureIgnoreCase));
        }

        #endregion
    }
}
