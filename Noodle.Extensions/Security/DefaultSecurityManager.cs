namespace Noodle.Extensions.Security
{
    /// <summary>
    /// The default/limited security manage implementation
    /// </summary>
    public class DefaultSecurityManager : ISecurityManager
    {
        public bool IsInRole(System.Security.Principal.IPrincipal user, string role)
        {
            return user.IsInRole(role);
        }

        public bool IsAuthorized(System.Security.Principal.IPrincipal user, string permission)
        {
            // No default way to manage permissions, so always return true.
            // It is another libraries responsibility for implementing this and providing more granular security.
            // Currently, Noodle.Security.dll has a more advanced implementation that has a db backed role/permission management
            return true;
        }
    }
}
