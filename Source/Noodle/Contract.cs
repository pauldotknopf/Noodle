using System;
using System.Diagnostics;
using System.Globalization;

namespace Noodle
{
    /// <summary>
    /// Helper class that helps enfore certain things
    /// </summary>
    public static class Contract
    {
        /// <summary>
        /// Determines whether the given argument is not null.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        [DebuggerHidden]
        public static void IsNotNull(object value, string parameterName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName, string.Format(CultureInfo.CurrentUICulture, "Arguement can't be null", parameterName));
            }
        }

        /// <summary>
        /// Determines whether the given argument is not null.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        [DebuggerHidden]
        public static void IsNotNullOrWhitespace(string value, string parameterName)
        {
            if (value.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(parameterName, string.Format(CultureInfo.CurrentUICulture, "Arguement null or whitespace", parameterName));
            }
        }
    }
}
