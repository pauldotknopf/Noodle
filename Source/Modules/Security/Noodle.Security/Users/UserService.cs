using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Noodle.Caching;

namespace Noodle.Security.Users
{
    /// <summary>
    /// User service
    /// </summary>
    public partial class UserService : IUserService
    {
        #region Fields

        private readonly IEncryptionService _encryptionService;
        private readonly MongoCollection<User> _userCollection;
        private readonly MongoCollection<UserUserRoleMap> _userRoleMapCollection;
        private readonly MongoCollection<UserRole> _userRoleCollection;
        private readonly ICacheManager _cacheManager;
        private readonly UserSettings _userSettings;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">The cache manager.</param>
        /// <param name="userSettings">The user settings.</param>
        /// <param name="encryptionService">The encryption service.</param>
        /// <param name="userCollection">The user collection.</param>
        /// <param name="userRoleMapCollection">The user role map collection.</param>
        /// <param name="userRoleCollection">The user role collection.</param>
        public UserService(ICacheManager cacheManager,
            UserSettings userSettings,
            IEncryptionService encryptionService,
            MongoCollection<User> userCollection,
            MongoCollection<UserUserRoleMap> userRoleMapCollection,
            MongoCollection<UserRole> userRoleCollection)
        {
            _cacheManager = cacheManager;
            _userSettings = userSettings;
            _encryptionService = encryptionService;
            _userCollection = userCollection;
            _userRoleMapCollection = userRoleMapCollection;
            _userRoleCollection = userRoleCollection;
        }

        #endregion

        #region Methods

        #region Users

        /// <summary>
        /// Gets all Users
        /// </summary>
        /// <param name="registrationFromUtc">User registration from; null to load all users</param>
        /// <param name="registrationToUtc">User registration to; null to load all users</param>
        /// <param name="userRoleIds">A list of user role identifiers to filter by (at least one match); pass null or empty list in order to load all users;</param>
        /// <param name="email">Email; null to load all users</param>
        /// <param name="username">Username; null to load all users</param>
        /// <param name="firstName">First name; null to load all users</param>
        /// <param name="lastName">Last name; null to load all users</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showDeleted">Show the deleted users</param>
        /// <returns>User collection</returns>
        /// <remarks></remarks>
        public virtual IPagedList<User> GetAllUsers(DateTime? registrationFromUtc = null,
            DateTime? registrationToUtc = null, ObjectId[] userRoleIds = null, string email = null, string username = null, bool showDeleted = false,
            string firstName = null, string lastName = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var filters = new List<IMongoQuery>();

            if(!showDeleted)
                filters.Add(Query<User>.EQ(x => x.Deleted, false));

            if(!string.IsNullOrEmpty(email))
            {
                var emailRegex = new Regex(email, RegexOptions.IgnoreCase);
                filters.Add(Query<User>.EQ(x => x.Email, BsonRegularExpression.Create(emailRegex)));
            }

            if (!string.IsNullOrEmpty(username))
            {
                var usernameRegex = new Regex(username, RegexOptions.IgnoreCase);
                filters.Add(Query<User>.EQ(x => x.Username, BsonRegularExpression.Create(usernameRegex)));
            }

            if (!string.IsNullOrEmpty(firstName))
            {
                var firstNameRegex = new Regex(firstName, RegexOptions.IgnoreCase);
                filters.Add(Query<User>.EQ(x => x.FirstName, BsonRegularExpression.Create(firstNameRegex)));
            }

            if (!string.IsNullOrEmpty(lastName))
            {
                var lastNameRegex = new Regex(lastName, RegexOptions.IgnoreCase);
                filters.Add(Query<User>.EQ(x => x.LastName, BsonRegularExpression.Create(lastNameRegex)));
            }

            if (registrationFromUtc.HasValue)
                filters.Add(Query<User>.GTE(x => x.CreatedOnUtc, registrationFromUtc.Value));

            if (registrationToUtc.HasValue)
                filters.Add(Query<User>.LTE(x => x.CreatedOnUtc, registrationToUtc.Value));

            if(userRoleIds != null && userRoleIds.Length > 0)
            {
                // TODO: Optimize
                var validUsers = _userRoleMapCollection.Find(Query<UserUserRoleMap>.In(x => x.UserRoleId, userRoleIds)).SetFields(Fields<UserUserRoleMap>.Include(x => x.UserId)).Select(x => x.UserId).Distinct();
                filters.Add(Query<User>.In(x => x.Id, validUsers));
            }

            var query = filters.Any() ? Query.And(filters) : null;

            var total = query != null ? _userCollection.Count(query) : _userCollection.Count();

            var logs = (query != null ? _userCollection.Find(query) : _userCollection.FindAll())
                .SetSkip(pageIndex * pageSize)
                .SetLimit(pageSize)
                .ToList();

            return new PagedList<User>(logs, pageIndex, pageSize, (int)total);
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="userId">User id to delete</param>
        /// <remarks></remarks>
        public virtual void DeleteUser(ObjectId userId)
        {
            _userCollection.Remove(Query<User>.EQ(x => x.Id, userId));
        }

        /// <summary>
        /// Gets a user
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <returns>A user</returns>
        /// <remarks></remarks>
        public virtual User GetUserById(ObjectId userId)
        {
            return _userCollection.FindOne(Query<User>.EQ(x => x.Id, userId));
        }

        /// <summary>
        /// Get user by email
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns>User</returns>
        /// <remarks></remarks>
        public virtual User GetUserByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return null;

            var emailRegex = new Regex(email, RegexOptions.IgnoreCase);
            return _userCollection.FindOne(Query<User>.EQ(x => x.Email, BsonRegularExpression.Create(emailRegex)));
        }

        /// <summary>
        /// Get user by email
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>User</returns>
        /// <remarks></remarks>
        public virtual User GetUserBySystemName(string systemName)
        {
            if (systemName.IsNullOrWhiteSpace())
                return null;

            return _userCollection.FindOne(Query<User>.EQ(x => x.SystemName, systemName));
        }

        /// <summary>
        /// Get user by username
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>User</returns>
        /// <remarks></remarks>
        public virtual User GetUserByUsername(string username)
        {
            if (username.IsNullOrWhiteSpace())
                return null;

            var usernameRegex = new Regex(username, RegexOptions.IgnoreCase);
            return _userCollection.FindOne(Query<User>.EQ(x => x.Username, BsonRegularExpression.Create(usernameRegex)));
        }

        /// <summary>
        /// Validate user
        /// </summary>
        /// <param name="usernameOrEmail">Username or email</param>
        /// <param name="password">Password</param>
        /// <returns>Result</returns>
        /// <remarks></remarks>
        public virtual bool ValidateUser(string usernameOrEmail, string password)
        {
            var user = _userSettings.UsernamesEnabled 
                            ? GetUserByUsername(usernameOrEmail) 
                            : GetUserByEmail(usernameOrEmail);

            if (user == null || user.Deleted || !user.Active)
                return false;

            string pwd;
            switch (user.PasswordFormat)
            {
                case PasswordFormat.Encrypted:
                    pwd = _encryptionService.EncryptText(password);
                    break;
                case PasswordFormat.Hashed:
                    pwd = _encryptionService.CreatePasswordHash(password, user.PasswordSalt, _userSettings.HashedPasswordFormat);
                    break;
                default:
                    pwd = password;
                    break;
            }

            var isValid = pwd == user.Password;

            //save last login date and last activity date
            if (isValid)
            {
                var currentTime = CommonHelper.CurrentTime();
                _userCollection.Update(Query<User>.EQ(x => x.Id, user.Id), Update<User>.Set(x => x.LastLoginDateUtc, currentTime).Set(x => x.LastActivityDateUtc, currentTime));
            }

            return isValid;
        }

        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Result</returns>
        /// <remarks></remarks>
        public virtual PasswordChangeResult ChangePassword(ChangePasswordRequest request)
        {
            var result = new PasswordChangeResult();
            if (request == null)
            {
                result.AddError("The change password request was not valid.");
                return result;
            }
            if (request.Email.IsNullOrWhiteSpace())
            {
                result.AddError("The email is not entered");
                return result;
            }
            if (request.NewPassword.IsNullOrWhiteSpace())
            {
                result.AddError("The password is not entered");
                return result;
            }

            var user = GetUserByEmail(request.Email);
            if (user == null)
            {
                result.AddError("The specified email could not be found");
                return result;
            }

            var requestIsValid = false;
            if (request.ValidateRequest)
            {
                //password
                string oldPwd;
                switch (user.PasswordFormat)
                {
                    case PasswordFormat.Encrypted:
                        oldPwd = _encryptionService.EncryptText(request.OldPassword);
                        break;
                    case PasswordFormat.Hashed:
                        oldPwd = _encryptionService.CreatePasswordHash(request.OldPassword, user.PasswordSalt, _userSettings.HashedPasswordFormat);
                        break;
                    default:
                        oldPwd = request.OldPassword;
                        break;
                }

                bool oldPasswordIsValid = oldPwd == user.Password;
                if (!oldPasswordIsValid)
                    result.AddError("Old password doesn't match");

                if (oldPasswordIsValid)
                    requestIsValid = true;
            }
            else
                requestIsValid = true;


            //at this point request is valid
            if (requestIsValid)
            {
                if (!request.NewPasswordFormat.HasValue)
                    request.NewPasswordFormat = _userSettings.PasswordFormat;

                switch (request.NewPasswordFormat)
                {
                    case PasswordFormat.Clear:
                        {
                            user.Password = request.NewPassword;
                        }
                        break;
                    case PasswordFormat.Encrypted:
                        {
                            user.Password = _encryptionService.EncryptText(request.NewPassword);
                        }
                        break;
                    case PasswordFormat.Hashed:
                        {
                            string saltKey = _encryptionService.CreateSaltKey(5);
                            user.PasswordSalt = saltKey;
                            user.Password = _encryptionService.CreatePasswordHash(request.NewPassword, saltKey, _userSettings.HashedPasswordFormat);
                        }
                        break;
                }
                user.PasswordFormat = request.NewPasswordFormat.Value;
                UpdateUser(user);
            }

            return result;
        }

        /// <summary>
        /// Sets a user email
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="newEmail">New email</param>
        /// <remarks></remarks>
        public virtual void SetEmail(User user, string newEmail)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            newEmail = newEmail.Trim();

            if (!CommonHelper.IsValidEmail(newEmail))
                throw new NoodleException("New email is not valid");

            if (newEmail.Length > 100)
                throw new NoodleException("E-mail address is too long.");

            var user2 = GetUserByEmail(newEmail);
            if (user2 != null && user.Id != user2.Id)
                throw new NoodleException("The e-mail address is already in use.");

            user.Email = newEmail;

            _userCollection.Update(Query<User>.EQ(x => x.Id, user.Id), Update<User>.Set(x => x.Email, user.Email));
        }

        /// <summary>
        /// Sets a user username
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="newUsername">New Username</param>
        /// <remarks></remarks>
        public virtual void SetUsername(User user, string newUsername)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (!_userSettings.UsernamesEnabled)
                throw new NoodleException("Usernames are disabled");

            if (!_userSettings.AllowUsersToChangeUsernames)
                throw new NoodleException("Changing usernames is not allowed");

            newUsername = newUsername.Trim();

            if (newUsername.Length > 100)
                throw new NoodleException("Username is too long.");

            var user2 = GetUserByUsername(newUsername);
            if (user2 != null && user.Id != user2.Id)
                throw new NoodleException("This username is already in use.");

            user.Username = newUsername;
            UpdateUser(user);
        }


        /// <summary>
        /// Insert a user
        /// </summary>
        /// <param name="user">User</param>
        /// <remarks></remarks>
        public virtual void InsertUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            _userCollection.Insert(user);
        }

        /// <summary>
        /// Updates the user
        /// </summary>
        /// <param name="user">User</param>
        /// <remarks></remarks>
        public virtual void UpdateUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            _userCollection.Update(Query<User>.EQ(x => x.Id, user.Id), Update<User>.Replace(user));
        }

        #endregion

        #region User roles

        /// <summary>
        /// Gets all user roles
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>User role collection</returns>
        /// <remarks></remarks>
        public virtual IList<UserRole> GetAllUserRoles(bool showHidden = false)
        {
            return (showHidden ? _userRoleCollection.FindAll() : _userRoleCollection.Find(Query<UserRole>.EQ(x => x.Active, true)))
                .SetSortOrder(SortBy<UserRole>.Ascending(x => x.Name))
                .ToList();
        }

        /// <summary>
        /// Gets user roles by user identifier
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>User role collection</returns>
        /// <remarks></remarks>
        public virtual IList<UserRole> GetUserRolesByUserId(ObjectId userId, bool showHidden = false)
        {
            if(userId == ObjectId.Empty)
                return new List<UserRole>();

            var roleIds = _userRoleMapCollection.Find(Query<UserUserRoleMap>.EQ(x => x.UserId, userId))
                .SetFields(Fields<UserUserRoleMap>.Include(x => x.UserRoleId)).Select(x => x.UserRoleId)
                .Distinct()
                .ToList();

            if(roleIds.Count == 0)
                return new List<UserRole>();

            var query = new List<IMongoQuery>();

            query.Add(Query<UserRole>.In(x => x.Id, roleIds));

            if(!showHidden)
                query.Add(Query<UserRole>.EQ(x => x.Active, true));

            return _userRoleCollection.Find(Query.And(query.ToArray()))
                .SetSortOrder(SortBy<UserRole>.Ascending(x => x.Name))
                .ToList();
        }

        /// <summary>
        /// Determines whether the given user is in the given system role
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="systemRoleName">Name of the system role.</param>
        /// <returns></returns>
        public virtual bool IsUserInRole(ObjectId userId, string systemRoleName)
        {
            if (userId == ObjectId.Empty)
                return false;

            var userRole = _userRoleCollection.FindOne(Query<UserRole>.EQ(x => x.SystemName, systemRoleName));

            if (userRole == null)
                return false;

            return IsUserInRole(userId, userRole.Id);
        }

        /// <summary>
        /// Determines if the given user is in the give role
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userRoleId"></param>
        /// <returns></returns>
        public virtual bool IsUserInRole(ObjectId userId, ObjectId userRoleId)
        {
            if (userId == ObjectId.Empty)
                return false;

            return _userRoleMapCollection.Count(Query.And(Query<UserUserRoleMap>.EQ(x => x.UserId, userId), Query<UserUserRoleMap>.EQ(x => x.UserRoleId, userRoleId))) > 0;
        }

        #endregion

        #endregion

        /// <summary>
        /// Adds a user to a role
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="systemRoleName"></param>
        /// <param name="roleId"></param>
        public void AddUserToRole(ObjectId userId, string systemRoleName = null, ObjectId? roleId = null)
        {
            if ((systemRoleName == null || systemRoleName.IsNullOrWhiteSpace()) && !roleId.HasValue)
            {
                return;
            }

            if(!roleId.HasValue)
            {
                var role = _userRoleCollection.FindOne(Query<UserRole>.EQ(x => x.SystemName, systemRoleName));
                if(role == null)
                    throw new InvalidOperationException("The role " + systemRoleName + " doesn't exist.");
                roleId = role.Id;
            }

            _userRoleMapCollection.Update(Query.And(Query<UserUserRoleMap>.EQ(x => x.UserId, userId), Query<UserUserRoleMap>.EQ(x => x.UserRoleId, roleId.Value)), 
                Update<UserUserRoleMap>.Replace(new UserUserRoleMap
                {
                    UserId = userId,
                    UserRoleId = roleId.Value
                }), 
                UpdateFlags.Upsert);
        }

        /// <summary>
        /// Adds a user to a role
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="systemRoleName"></param>
        /// <param name="roleId"></param>
        public void RemoveUserFromRole(ObjectId userId, string systemRoleName = null, ObjectId? roleId = null)
        {
            if ((systemRoleName == null || systemRoleName.IsNullOrWhiteSpace()) && !roleId.HasValue)
            {
                return;
            }

            if (!roleId.HasValue)
            {
                var role = _userRoleCollection.FindOne(Query<UserRole>.EQ(x => x.SystemName, systemRoleName));
                if (role == null)
                    throw new InvalidOperationException("The role " + systemRoleName + " doesn't exist.");
                roleId = role.Id;
            }

            _userRoleMapCollection.Remove(Query.And(Query<UserUserRoleMap>.EQ(x => x.UserId, userId), Query<UserUserRoleMap>.EQ(x => x.UserRoleId, roleId.Value)));
        }
    }
}
