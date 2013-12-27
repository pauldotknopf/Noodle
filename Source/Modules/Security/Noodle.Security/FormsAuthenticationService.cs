using System;
using System.Web;
using System.Web.Security;
using Noodle.Security.Users;

namespace Noodle.Security
{
    /// <summary>
    /// Authentication service
    /// </summary>
    /// <remarks></remarks>
    public class FormsAuthenticationService : IAuthenticationService
    {
        private readonly IUserService _userService;
        private readonly HttpContextWrapper _httpContext;
        private readonly UserSettings _userSettings;
        private readonly TimeSpan _expirationTimeSpan;

        private User _cachedUser;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="userService">The user service.</param>
        /// <param name="httpContext"> </param>
        /// <param name="userSettings"> </param>
        /// <remarks></remarks>
        public FormsAuthenticationService(IUserService userService,
            HttpContextWrapper httpContext,
            UserSettings userSettings)
        {
            _userService = userService;
            _httpContext = httpContext;
            _userSettings = userSettings;
            _expirationTimeSpan = TimeSpan.FromHours(1);
        }


        /// <summary>
        /// Signs the in.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="createPersistentCookie">if set to <c>true</c> [create persistent cookie].</param>
        /// <remarks></remarks>
        public virtual void SignIn(User user, bool createPersistentCookie)
        {
            var now = CommonHelper.CurrentTime().ToLocalTime();

            var ticket = new FormsAuthenticationTicket(
                1 /*version*/,
                _userSettings.UsernamesEnabled ? user.Username : user.Email,
                now,
                now.Add(_expirationTimeSpan),
                createPersistentCookie,
                _userSettings.UsernamesEnabled ? user.Username : user.Email,
                FormsAuthentication.FormsCookiePath);

            var encryptedTicket = FormsAuthentication.Encrypt(ticket);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            cookie.HttpOnly = true;
            cookie.Expires = now.Add(_expirationTimeSpan);
            cookie.Secure = FormsAuthentication.RequireSSL;
            cookie.Path = FormsAuthentication.FormsCookiePath;
            if (FormsAuthentication.CookieDomain != null)
            {
                cookie.Domain = FormsAuthentication.CookieDomain;
            }

            _httpContext.Response.Cookies.Add(cookie);

            _cachedUser = user;
        }

        /// <summary>
        /// Signs the out.
        /// </summary>
        /// <remarks></remarks>
        public virtual void SignOut()
        {
            _cachedUser = null;
            FormsAuthentication.SignOut();
        }

        /// <summary>
        /// Gets the authenticated user.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public virtual User GetAuthenticatedUser()
        {
            if (_cachedUser != null)
                return _cachedUser;

            var request = _httpContext.Request;
            if (request == null ||
                !request.IsAuthenticated ||
                !(_httpContext.User.Identity is FormsIdentity))
            {
                return null;
            }

            var formsIdentity = (FormsIdentity)_httpContext.User.Identity;
            var user = GetAuthenticatedUserFromTicket(formsIdentity.Ticket);
            if (user != null && user.Active && !user.Deleted)
                _cachedUser = user;
            return _cachedUser;
        }

        /// <summary>
        /// Gets the authenticated user from ticket.
        /// </summary>
        /// <param name="ticket">The ticket.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public virtual User GetAuthenticatedUserFromTicket(FormsAuthenticationTicket ticket)
        {
            if (ticket == null)
                throw new ArgumentNullException("ticket");

            var usernameOrEmail = ticket.UserData;

            if (usernameOrEmail.IsNullOrWhiteSpace())
                return null;
            var user = _userSettings.UsernamesEnabled
                ? _userService.GetUserByUsername(usernameOrEmail)
                : _userService.GetUserByEmail(usernameOrEmail);
            return user;
        }
    }
}
