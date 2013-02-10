using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SimpleInjector;

namespace Noodle.Logging
{
    /// <summary>
    /// Log store the stores information about log custom data and ignore patterns
    /// </summary>
    public static class LogStore
    {
        private static List<Action<Exception, Container, Dictionary<string, string>>> _customDataActions = new List<Action<Exception, Container, Dictionary<string, string>>>();
        private static object _lockObject = new object();
        private static List<Regex> _ignoreRegex = new List<Regex>();
        private static List<Type> _ignoreExceptions = new List<Type>();

        /// <summary>
        /// Add a delegate to add custom data to the error
        /// </summary>
        /// <param name="action"></param>
        public static void AddCustomDataAction(Action<Exception, Container, Dictionary<string, string>> action)
        {
            lock (_lockObject)
            {
                _customDataActions.Add(action);
            }
        }

        /// <summary>
        /// Clear any delegates that may be wired up to give custom data
        /// </summary>
        public static void ClearCustomDataActions()
        {
            lock (_lockObject)
            {
                _customDataActions.Clear();
            }
        }

        /// <summary>
        /// Clears all the exception types that are configured to be ignored
        /// </summary>
        public static void ClearIgnoredExceptionTypes()
        {
            lock(_lockObject)
            {
                _ignoreExceptions.Clear();
            }
        }

        /// <summary>
        /// Ignores any exceptions of type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void IgnoreException<T>() where T:Exception
        {
            lock(_lockObject)
            {
                if(!_ignoreExceptions.Contains(typeof(T)))
                {
                    _ignoreExceptions.Add(typeof(T));
                }
            }
        }

        /// <summary>
        /// Get custom data associated to an exception
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="kernel">The kernel.</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetCustomData(Exception exception, Container kernel)
        {
            var data = new Dictionary<string, string>();
            foreach(var action in _customDataActions)
            {
                try
                {
                    action(exception, kernel, data);
                }
                catch(Exception ex)
                {
                    data.Clear();
                    data.Add("CustomDataError", ex.ToString());
                }
            }
            return data;
        }

        /// <summary>
        /// Gets the list of exceptions to ignore
        /// </summary>
        public static List<Regex> IgnoreRegexes
        {
            get { return _ignoreRegex; }
        }

        /// <summary>
        /// Determines if the error is log-able, meaning that there is no ignore regex or ignore exception type
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static bool IsErrorLoggable(Exception ex)
        {
            if (_ignoreRegex.Any(re => re.IsMatch(ex.ToString())))
                return false;
            if (_ignoreExceptions.Any(x => x.IsInstanceOfType(ex)))
                return false;
            return true;
        }
    }
}
