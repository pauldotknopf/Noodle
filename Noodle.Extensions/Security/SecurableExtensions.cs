using System.Linq;
using System.Security.Principal;

namespace Noodle.Security
{
    public static class SecurableExtensions
    {
        /// <summary>Checks whether the user is authorized to use a editable and has sufficient permissions.</summary>
        /// <param name="security">The security manager to query permissions on.</param>
        /// <param name="permittableOrSecurable">The object containing security information.</param>
        /// <param name="user">The user to check permissions for.</param>
        /// <returns>True if the user is authorized for the given editable.</returns>
        public static bool IsAuthorized(this ISecurityManager security, ISecurableBase permittableOrSecurable, IPrincipal user)
        {
            return user != null && IsAuthorized(security, (object)permittableOrSecurable, user) && IsPermitted(security, permittableOrSecurable, user);
        }

        private static bool IsPermitted(ISecurityManager security, object possiblyPermittable, IPrincipal user)
        {
            var permittable = possiblyPermittable as IPermittable;

            if (permittable == null)
                return true;

            if (permittable.RequiredPermissions == null)
                return true;

            if (permittable.RequiredPermissions.Length == 0)
                return true;

            // only authenticated users can be permitted
            if (!user.Identity.IsAuthenticated)
                return false;

            return permittable.RequiredPermissions.Any(permission => security.IsAuthorized(user, permission));
        }

        private static bool IsAuthorized(ISecurityManager security, object possiblySecurable, IPrincipal user)
        {
            var securable = possiblySecurable as ISecurable;

            if (securable == null)
                return true;

            if (securable.RequiredRoles == null)
                return true;

            if (securable.RequiredRoles.Length == 0)
                return true;

            // only authenticated users can be permitted
            if (!user.Identity.IsAuthenticated)
                return false;

            return securable.RequiredRoles.Any(role => security.IsInRole(user, role));
        }
    }
}
