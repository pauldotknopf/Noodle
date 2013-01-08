using System.Security.Principal;

namespace Noodle.Tests
{
    public class FakeIdentity : IIdentity
    {
        private readonly bool _isAuthenticated;
        private readonly string _name;

        public FakeIdentity(bool isAuthenticated, string name)
        {
            _isAuthenticated = isAuthenticated;
            _name = name;
        }

        public string AuthenticationType
        {
            get { return "Fake"; }
        }

        public bool IsAuthenticated
        {
            get { return _isAuthenticated; }
        }

        public string Name
        {
            get { return _name; }
        }
    }
}
