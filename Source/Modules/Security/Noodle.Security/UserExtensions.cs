using System;
using System.Linq;
using Noodle.Security.Users;

namespace Noodle.Security
{
    public static class UserExtentions
    {
        /// <summary>
        /// Return the full name for the given user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static string GetFullName(this User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            var firstName = user.FirstName;
            var lastName = user.LastName;

            var fullName = "";
            if (!firstName.IsNullOrWhiteSpace() && !lastName.IsNullOrWhiteSpace())
                fullName = string.Format("{0} {1}", firstName, lastName);
            else
            {
                if (!firstName.IsNullOrWhiteSpace())
                    fullName = firstName;

                if (!lastName.IsNullOrWhiteSpace())
                    fullName = lastName;
            }
            return fullName;
        }

        /// <summary>
        /// Formats the user name
        /// </summary>
        /// <param name="user">Source</param>
        /// <returns>Formatted text</returns>
        public static string FormatUserName(this User user)
        {
            return FormatUserName(user, false);
        }

        /// <summary>
        /// Formats the user name
        /// </summary>
        /// <param name="user">Source</param>
        /// <param name="stripTooLong">Strip too long user name</param>
        /// <returns>Formatted text</returns>
        public static string FormatUserName(this User user, bool stripTooLong)
        {
            if (user == null)
                return string.Empty;

            string result = string.Empty;
            switch (EngineContext.Resolve<UserSettings>().UserNameFormat)
            {
                case UserNameFormat.ShowEmails:
                    result = user.Email;
                    break;
                case UserNameFormat.ShowFullNames:
                    result = user.GetFullName();
                    break;
                case UserNameFormat.ShowUsernames:
                    result = user.Username;
                    break;
            }

            if (stripTooLong)
            {
                int maxLength = 0; // EngineContext.Current.Resolve<UserSettings>().FormatNameMaxLength;
                if (maxLength > 0 && result.Length > maxLength)
                {
                    result = result.Substring(0, maxLength);
                }
            }

            return result;
        }

    }
}
