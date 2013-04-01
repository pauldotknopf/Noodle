using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace Noodle.Security.Users
{
    /// <summary>
    /// Handles all interactions with users/roles
    /// </summary>
    public interface IUserService
    {
        #region Users

        /// <summary>
        /// Gets all Users
        /// </summary>
        /// <param name="registrationFromUtc">User registration from; null to load all users</param>
        /// <param name="registrationToUtc">User registration to; null to load all users</param>
        /// <param name="userRoleIds">A list of user role identifiers to filter by (at least one match); pass null or empty list in order to load all users; </param>
        /// <param name="email">Email; null to load all users</param>
        /// <param name="username">Username; null to load all users</param>
        /// <param name="showDeleted">Show deleted users</param>
        /// <param name="firstName">First name; null to load all users</param>
        /// <param name="lastName">Last name; null to load all users</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>User collection</returns>
        IPagedList<User> GetAllUsers(DateTime? registrationFromUtc = null,
           DateTime? registrationToUtc = null, ObjectId[] userRoleIds = null, string email = null, string username = null, bool showDeleted = false,
           string firstName = null, string lastName = null, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="userId">User id to delete</param>
        void DeleteUser(ObjectId userId);

        /// <summary>
        /// Gets a user
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <returns>A user</returns>
        User GetUserById(ObjectId userId);

        /// <summary>
        /// Get user by email
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns>User</returns>
        User GetUserByEmail(string email);

        /// <summary>
        /// Get user by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>User</returns>
        User GetUserBySystemName(string systemName);

        /// <summary>
        /// Get user by username
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>User</returns>
        User GetUserByUsername(string username);

        /// <summary>
        /// Validate user
        /// </summary>
        /// <param name="usernameOrEmail">Username or email</param>
        /// <param name="password">Password</param>
        /// <returns>Result</returns>
        bool ValidateUser(string usernameOrEmail, string password);

        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Result</returns>
        PasswordChangeResult ChangePassword(ChangePasswordRequest request);

        /// <summary>
        /// Sets a user email
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="newEmail">New email</param>
        void SetEmail(User user, string newEmail);

        /// <summary>
        /// Sets a username
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="newUsername">New Username</param>
        void SetUsername(User user, string newUsername);

        /// <summary>
        /// Insert a user
        /// </summary>
        /// <param name="user">User</param>
        void InsertUser(User user);

        /// <summary>
        /// Updates the user
        /// </summary>
        /// <param name="user">User</param>
        void UpdateUser(User user);

        #endregion

        #region User roles

        /// <summary>
        /// Gets all user roles
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>User role collection</returns>
        IList<UserRole> GetAllUserRoles(bool showHidden = false);

        /// <summary>
        /// Adds a user to a role
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="systemRoleName"></param>
        /// <param name="roleId"></param>
        void AddUserToRole(ObjectId userId, string systemRoleName = null, ObjectId? roleId = null);

        /// <summary>
        /// Adds a user to a role
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="systemRoleName"></param>
        /// <param name="roleId"></param>
        void RemoveUserFromRole(ObjectId userId, string systemRoleName = null, ObjectId? roleId = null);

        /// <summary>
        /// Gets all roles assigned to the given user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="showHidden"></param>
        /// <returns></returns>
        IList<UserRole> GetUserRolesByUserId(ObjectId userId, bool showHidden = false);

        /// <summary>
        /// Determines whether the given user is in the given system role
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="systemRoleName">Name of the system role.</param>
        /// <returns></returns>
        bool IsUserInRole(ObjectId userId, string systemRoleName);
        
        /// <summary>
        /// Determines if the given user is in the give role
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userRoleId"></param>
        /// <returns></returns>
        bool IsUserInRole(ObjectId userId, ObjectId userRoleId);

        #endregion
    }
}
