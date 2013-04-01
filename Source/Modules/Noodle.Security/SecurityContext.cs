using System;
using Noodle.Security.Users;

namespace Noodle.Security
{
    public class SecurityContext : ISecurityContext
    {
        private readonly IUserService _userService;
        private readonly IAuthenticationService _authenticationService;

        private User _cachedUser;

        public SecurityContext(IUserService userService,
            IAuthenticationService authenticationService)
        {
            _userService = userService;
            _authenticationService = authenticationService;
        }

        protected User GetCurrentUser()
        {
            if (_cachedUser != null)
                return _cachedUser;

            var user = _authenticationService.GetAuthenticatedUser();

            //validation
            if (user != null && !user.Deleted && user.Active)
            {
                //update last activity date
                if (user.LastActivityDateUtc.AddMinutes(1.0) < CommonHelper.CurrentTime())
                {
                    user.LastActivityDateUtc = CommonHelper.CurrentTime();
                    _userService.UpdateUser(user);
                }

                _cachedUser = user;
            }

            return _cachedUser;
        }

        /// <summary>
        /// Gets or sets the current user
        /// </summary>
        public User CurrentUser
        {
            get
            {
                return GetCurrentUser();
            }
            set
            {
                _cachedUser = value;
            }
        }
    }
}
