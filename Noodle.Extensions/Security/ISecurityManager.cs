using System.Security.Principal;

namespace Noodle.Security
{
    /// <summary>
    /// A manager resonsible for managing permissions.
    /// This is included in Noodle.dll and the default implementation implemented only IsInRole. IsAuthorized always returns true.
    /// Certain services depend on this to give "support" for filtering based on roles and pemissions.
    /// If you use/include Noodle.Security.dll, another implementation will be used that utilizes database role/permission authorization.
    /// </summary>
    public interface ISecurityManager
    {
        bool IsInRole(IPrincipal user, string role);
        bool IsAuthorized(IPrincipal user, string permission);
    }
}
