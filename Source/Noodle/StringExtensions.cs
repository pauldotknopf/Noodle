using System.Linq;

namespace Noodle
{
    public static class StringExtensions
    {
        /// <summary>
        /// This is a method available in .NET but ported back so we can use it in 2.0/3.5
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string value)
        {
            if (value == null)
                return true;

            return value.Count(char.IsWhiteSpace) == value.Length;
        }

        /// <summary>
        /// Formats the string with the given arguments
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arguements"> </param>
        /// <returns></returns>
        public static string F(this string format, params object[] arguements)
        {
            return string.Format(format, arguements);
        }

        /// <summary>
        /// Answers true if this String is neither null or empty.
        /// </summary>
        /// <remarks>I'm also tired of typing !String.IsNullOrEmpty(s)</remarks>
        public static bool HasValue(this string s)
        {
            return !IsNullOrEmpty(s);
        }

        /// <summary>
        /// Answers true if this String is either null or empty.
        /// </summary>
        /// <remarks>I'm so tired of typing String.IsNullOrEmpty(s)</remarks>
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }
    }
}
