using System;
using System.Linq;
using System.Security.Principal;
using Noodle.Extensions.Security;
using Noodle.Security.Permissions;
using Noodle.Security.Users;

namespace Noodle.Security
{
    public class SecurityManager : ISecurityManager
    {
        private readonly IUserService _userService;
        private readonly UserSettings _userSettings;
        private readonly IPermissionService _permissionService;

        public SecurityManager(IUserService userService, 
            UserSettings userSettings,
            IPermissionService permissionService)
        {
            _userService = userService;
            _userSettings = userSettings;
            _permissionService = permissionService;
        }

        public bool IsInRole(IPrincipal user, string role)
        {
            var dbUser = ConvertPrincipleToUser(user);

            return _userService.IsUserInRole(dbUser.Id, role);
        }

        public bool IsAuthorized(IPrincipal user, string permission)
        {
            var dbUser = ConvertPrincipleToUser(user);

            return dbUser != null 
                && _permissionService.Authorize(dbUser.Id, permission);
        }

        private User ConvertPrincipleToUser(IPrincipal user)
        {
            if (!user.Identity.IsAuthenticated)
                return null;

            var dbUser = _userSettings.UsernamesEnabled
                             ? _userService.GetUserByUsername(user.Identity.Name)
                             : _userService.GetUserByEmail(user.Identity.Name);

            return dbUser;
        }
    }
}
