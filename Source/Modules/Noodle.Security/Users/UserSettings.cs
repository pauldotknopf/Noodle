using System.Web.Configuration;

namespace Noodle.Security.Users
{
    public class UserSettings : ISettings
    {
        public UserSettings()
        {
            PasswordFormat = PasswordFormat.Encrypted;
            HashedPasswordFormat = EncryptionFormat.SHA1;
        }

        /// <summary>
        /// Gets or sets a value indicating whether usernames are used instead of emails
        /// </summary>
        public bool UsernamesEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether users are allowed to change their usernames
        /// </summary>
        public bool AllowUsersToChangeUsernames { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether users are allowed to set their timezone.
        /// </summary>
        public bool AllowUsersToSetTimeZone { get; set; }

        /// <summary>
        /// Gets or sets a customer password format (SHA1, MD5) when passwords are hashed
        /// </summary>
        public EncryptionFormat HashedPasswordFormat { get; set; }

        /// <summary>
        /// Gets or sets the default time zone
        /// </summary>
        public string DefaultTimeZoneId { get; set; }

        /// <summary>
        /// Customer name formatting
        /// </summary>
        public UserNameFormat UserNameFormat { get; set; }

        /// <summary>
        /// A value (1-5) indicating the required strenth for the passwords.
        ///     Blank = 0,
        ///     VeryWeak = 1,
        ///     Weak = 2,
        ///     Medium = 3,
        ///     Strong = 4,
        ///     VeryStrong = 5
        /// </summary>
        public int PasswordStrength { get; set; }

        /// <summary>
        /// The default password format for all users
        /// </summary>
        public PasswordFormat PasswordFormat { get; set; }
    }
}
