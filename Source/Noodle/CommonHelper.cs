using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Noodle.ComponentModel;

namespace Noodle
{
    /// <summary>
    /// Common helper methods used through the tiers.
    /// </summary>
    /// <remarks></remarks>
    public static class CommonHelper
    {
        /// <summary>Ampersand string.</summary>
        public const string Amp = "&";
        public static readonly string[] QuerySplitter = new[] { "&amp;", Amp };

        #region Type converting

        /// <summary>
        /// This is here for unit tests.
        /// GetEntryAssembly returns null in unit tests
        /// </summary>
        /// <returns></returns>
        public static Func<Assembly> GetEntryAssembly = () => Assembly.GetEntryAssembly();

        /// <summary>
        /// Converts a value to a destination type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="destinationType">The type to convert the value to.</param>
        /// <returns>The converted value.</returns>
        /// <remarks></remarks>
        public static object To(object value, Type destinationType)
        {
            return To(value, destinationType, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts a value to a destination type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="destinationType">The type to convert the value to.</param>
        /// <param name="culture">Culture</param>
        /// <returns>The converted value.</returns>
        /// <remarks></remarks>
        public static object To(object value, Type destinationType, CultureInfo culture)
        {
            if (value != null)
            {
                if (destinationType == typeof(bool))
                {
                    if (value is string)
                    {
                        if (value.ToString() == "0")
                        {
                            value = "False";
                        }
                        else if (value.ToString() == "1")
                        {
                            value = "True";
                        }
                    }
                }
                TypeConverter destinationConverter = TypeDescriptor.GetConverter(destinationType);
                TypeConverter sourceConverter = TypeDescriptor.GetConverter(value.GetType());
                if (destinationConverter != null && destinationConverter.CanConvertFrom(value.GetType()))
                    return destinationConverter.ConvertFrom(null, culture, value);
                if (sourceConverter != null && sourceConverter.CanConvertTo(destinationType))
                    return sourceConverter.ConvertTo(null, culture, value, destinationType);
                if (destinationType.IsEnum && value is int)
                    return Enum.ToObject(destinationType, (int)value);
                if (!destinationType.IsAssignableFrom(value.GetType()))
                    return Convert.ChangeType(value, destinationType, culture);
            }
            return value;
        }

        /// <summary>
        /// Converts a value to a destination type.
        /// </summary>
        /// <typeparam name="T">The type to convert the value to.</typeparam>
        /// <param name="value">The value to convert.</param>
        /// <param name="defaultValue">The value to return if no conversion exists</param>
        /// <returns>The converted value.</returns>
        /// <remarks></remarks>
        public static T To<T>(object value, T defaultValue = default(T))
        {
            try
            {
                return (T)To(value, typeof(T));
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// Gets the custom type converter.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static TypeConverter GetCustomTypeConverter(Type type)
        {
            if (type == typeof(List<int>))
                return new GenericListTypeConverter<int>();
            if (type == typeof(List<decimal>))
                return new GenericListTypeConverter<decimal>();
            if (type == typeof(List<string>))
                return new GenericListTypeConverter<string>();
            if (type == typeof(bool))
                return new BooleanTypCeonverter();

            return TypeDescriptor.GetConverter(type);
        }

        /// <summary>
        /// There is no Guid.TryParse in 2.0-3.5 so we have this here. It is in 4.0 though (go figuire).
        /// </summary>
        /// <param name="s"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool GuidTryParse(string s, out Guid result)
        {
            if (s == null)
                throw new ArgumentNullException("s");
            var format = new Regex(
                "^[A-Fa-f0-9]{32}$|" +
                "^({|\\()?[A-Fa-f0-9]{8}-([A-Fa-f0-9]{4}-){3}[A-Fa-f0-9]{12}(}|\\))?$|" +
                "^({)?[0xA-Fa-f0-9]{3,10}(, {0,1}[0xA-Fa-f0-9]{3,6}){2}, {0,1}({)([0xA-Fa-f0-9]{3,4}, {0,1}){7}[0xA-Fa-f0-9]{3,4}(}})$");
            var match = format.Match(s);
            if (match.Success)
            {
                result = new Guid(s);
                return true;
            }
            result = Guid.Empty;
            return false;
        }

        #endregion

        #region Validation

        /// <summary>
        /// Verifies that a string is in valid e-mail format
        /// </summary>
        /// <param name="email">Email to verify</param>
        /// <returns>true if the string is a valid e-mail address and false if it's not</returns>
        /// <remarks></remarks>
        public static bool IsValidEmail(string email)
        {
            bool result = false;
            if (String.IsNullOrEmpty(email))
                return result;
            email = email.Trim();
            result = Regex.IsMatch(email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
            return result;
        }

        /// <summary>
        /// Ensure that a string is not null
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>Result</returns>
        /// <remarks></remarks>
        public static string EnsureNotNull(string str)
        {
            if (str == null)
                return String.Empty;

            return str;
        }

        /// <summary>
        /// Ensure that a string doesn't exceed maximum allowed length
        /// </summary>
        /// <param name="str">Input string</param>
        /// <param name="maxLength">Maximum length</param>
        /// <returns>Input string if its length is OK; otherwise, truncated input string</returns>
        /// <remarks></remarks>
        public static string EnsureMaximumLength(string str, int maxLength)
        {
            if (String.IsNullOrEmpty(str))
                return str;

            return str.Length > maxLength ? str.Substring(0, maxLength) : str;
        }

        #endregion

        #region Evaluation/Setting

        /// <summary>
        /// Tries to find a property matching the supplied expression, returns null if no property is found with the first part of the expression.
        /// </summary>
        /// <param name="item">The object to query.</param>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static object Evaluate(object item, string expression)
        {
            if (item == null) return null;

            var info = item.GetType().GetProperty(expression);
            if (info != null)
                return info.GetValue(item, new object[0]);
            if (expression.IndexOf('.') > 0)
            {
                var dotIndex = expression.IndexOf('.');
                var obj = Evaluate(item, expression.Substring(0, dotIndex));
                if (obj != null)
                    return Evaluate(obj, expression.Substring(dotIndex + 1, expression.Length - dotIndex - 1));
            }
            return null;
        }

        /// <summary>
        /// Evaluates an expression and applies a format string.
        /// </summary>
        /// <param name="item">The object to query.</param>
        /// <param name="expression">The expression to evaluate.</param>
        /// <param name="format">The format string to apply.</param>
        /// <returns>The formatted result ov the evaluation.</returns>
        /// <remarks></remarks>
        public static string Evaluate(object item, string expression, string format)
        {
            return String.Format(format, Evaluate(item, expression));
        }

        /// <summary>
        /// Sets a property on an object to a valuae.
        /// </summary>
        /// <param name="instance">The object whose property to set.</param>
        /// <param name="propertyName">The name of the property to set.</param>
        /// <param name="value">The value to set the property to.</param>
        /// <remarks></remarks>
        public static void SetProperty(object instance, string propertyName, object value)
        {
            if (instance == null) throw new ArgumentNullException("instance");
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            var instanceType = instance.GetType();
            var pi = instanceType.GetProperty(propertyName);
            if (pi == null)
                throw new NoodleException("No property '{0}' found on the instance of type '{1}'.", propertyName, instanceType);
            if (!pi.CanWrite)
                throw new NoodleException("The property '{0}' on the instance of type '{1}' does not have a setter.", propertyName, instanceType);
            if (value != null && !value.GetType().IsAssignableFrom(pi.PropertyType))
                value = To(value, pi.PropertyType);
            pi.SetValue(instance, value, new object[0]);
        }

        #endregion

        #region Xml

        /// <summary>
        /// Converts an XML type into a .NET compatible type - roughly
        /// </summary>
        /// <param name="xmlType"></param>
        /// <returns></returns>
        public static Type MapXmlTypeToType(string xmlType)
        {
            xmlType = xmlType.ToLower();

            if (xmlType == "string")
                return typeof(string);
            if (xmlType == "integer")
                return typeof(int);
            if (xmlType == "long")
                return typeof(long);
            if (xmlType == "boolean")
                return typeof(bool);
            if (xmlType == "datetime")
                return typeof(DateTime);
            if (xmlType == "float")
                return typeof(float);
            if (xmlType == "decimal")
                return typeof(decimal);
            if (xmlType == "double")
                return typeof(Double);
            if (xmlType == "single")
                return typeof(Single);

            if (xmlType == "byte")
                return typeof(byte);
            if (xmlType == "base64binary")
                return typeof(byte[]);


            // return null if no match is found
            // don't throw so the caller can decide more efficiently what to do 
            // with this error result
            return null;
        }

        /// <summary>
        /// Converts a .NET type into an XML compatible type - roughly
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string MapTypeToXmlType(Type type)
        {
            if (type == typeof(string) || type == typeof(char))
                return "string";
            if (type == typeof(int) || type == typeof(Int32))
                return "integer";
            if (type == typeof(long) || type == typeof(Int64))
                return "long";
            if (type == typeof(bool))
                return "boolean";
            if (type == typeof(DateTime))
                return "datetime";

            if (type == typeof(float))
                return "float";
            if (type == typeof(decimal))
                return "decimal";
            if (type == typeof(double))
                return "double";
            if (type == typeof(Single))
                return "single";

            if (type == typeof(byte))
                return "byte";

            if (type == typeof(byte[]))
                return "base64Binary";

            return null;

            // *** hope for the best
            //return type.ToString().ToLower();
        }

        /// <summary>
        /// Format XML
        /// </summary>
        /// <param name="sUnformattedXml"></param>
        /// <returns></returns>
        public static string FormatXml(string sUnformattedXml)
        {
            //load unformatted xml into a dom
            var xd = new XmlDocument();
            xd.LoadXml(sUnformattedXml);

            //will hold formatted xml
            var sb = new StringBuilder();

            //pumps the formatted xml into the StringBuilder above
            var sw = new StringWriter(sb);

            //does the formatting
            XmlTextWriter xtw = null;

            try
            {
                //point the xtw at the StringWriter
                xtw = new XmlTextWriter(sw) {Formatting = Formatting.Indented};

                //we want the output formatted

                //get the dom to dump its contents into the xtw 
                xd.WriteTo(xtw);
            }
            finally
            {
                //clean up even if error
                if (xtw != null)
                    xtw.Close();
            }

            //return the formatted xml
            return sb.ToString();
        }

        #endregion

        #region General

        /// <summary>
        /// Cleans the string for URL.
        /// </summary>
        /// <param name="strThis">The STR this.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string CleanStringForUrl(this string strThis)
        {
            if (strThis == null)
                return null;

            strThis = strThis.Replace(" ", "-").Replace("'", "").Replace(",", "").Trim();

            var sb = new StringBuilder();

            foreach (char c in strThis.Normalize(NormalizationForm.FormD))
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Copies the stream to another stream. It doesn't close either.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="output">The output.</param>
        /// <remarks></remarks>
        public static void CopyStream(Stream input, Stream output)
        {
            var buffer = new byte[8 * 1024];
            int len;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
        }

        /// <summary>
        /// Convert a text to a pascal/camel case string
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string PascalCamelToFriendly(string text)
        {
            if (text.IsNullOrWhiteSpace())
                return "";

            // if all upper case, then don't pascal case.
            if (text == text.ToUpper())
                return text;

            var newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (var i = 1; i < text.Length; i++)
            {
                if (Char.IsUpper(text[i]) && text[i - 1] != ' ')
                    newText.Append(' ');
                newText.Append(text[i]);
            }
            return newText.ToString();
        }

        /// <summary>
        /// Get the current time (not utc). 
        /// This is used by certain services for unit testing as this delegate can be changed easily (as opposed to calling DateTime.UtcNow directly)
        /// </summary>
        public static Func<DateTime> CurrentTime = () => DateTime.UtcNow;

        /// <summary>Returns true if the requested resource is one of the typical resources that don't need to be worries about.</summary>
        /// <param name="path">The path to determine if it is a static resource.</param>
        /// <returns>True if the request targets a static resource file.</returns>
        /// <remarks>
        /// These are the file extensions considered to be static resources:
        /// .css
        ///	.gif
        /// .png 
        /// .jpg
        /// .jpeg
        /// .js
        /// .axd
        /// .ashx
        /// </remarks>
        public static bool IsStaticResource(string path)
        {
            if(String.IsNullOrEmpty(path))
            {
                return false;
            }

            string extension = Path.GetExtension(path);

            if (extension == null) return false;

            switch (extension.ToLower())
            {
                case ".gif":
                case ".png":
                case ".jpg":
                case ".jpeg":
                case ".swf":
                case ".js":
                case ".css":
                case ".axd":
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Convert an interval to a timespan
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static TimeSpan CalculateInterval(this int interval, TimeUnit unit)
        {
            switch (unit)
            {
                case TimeUnit.Seconds:
                    return new TimeSpan(0, 0, interval);
                case TimeUnit.Minutes:
                    return new TimeSpan(0, interval, 0);
                case TimeUnit.Hours:
                    return new TimeSpan(interval, 0, 0);
                default:
                    throw new NotSupportedException("Unknown time unit: " + unit);
            }
        }

        /// <summary>
        /// Convert a unit of time to a timespan based on an interval
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static TimeSpan CalculateInterval(this TimeUnit unit, int interval)
        {
            switch (unit)
            {
                case TimeUnit.Seconds:
                    return new TimeSpan(0, 0, interval);
                case TimeUnit.Minutes:
                    return new TimeSpan(0, interval, 0);
                case TimeUnit.Hours:
                    return new TimeSpan(interval, 0, 0);
                default:
                    throw new NotSupportedException("Unknown time unit: " + unit);
            }
        }

        /// <summary>Converts a text query string to a dictionary.</summary>
        /// <param name="query">The query string</param>
        /// <returns>A dictionary of the query parts.</returns>
        public static IDictionary<string, string> ParseQueryString(string query)
        {
            var dictionary = new Dictionary<string, string>();
            if (query == null)
                return dictionary;

            string[] queries = query.Split(QuerySplitter, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < queries.Length; i++)
            {
                string q = queries[i];
                int eqIndex = q.IndexOf("=");
                if (eqIndex >= 0)
                    dictionary[q.Substring(0, eqIndex)] = q.Substring(eqIndex + 1);
            }
            return dictionary;
        }

        /// <summary>
        /// Parses a query string as a name value collection
        /// </summary>
        /// <param name="query">The query string</param>
        /// <returns>A namevaluecollection of the query parts.</returns>
        public static NameValueCollection ParseQueryStringAsNameValueCollection(string query)
        {
            var nameValueColleciton = new NameValueCollection();
            foreach (var keyValue in ParseQueryString(query))
            {
                nameValueColleciton[keyValue.Key] = keyValue.Value;
            }
            return nameValueColleciton;
        }

        #endregion
    }
}
