using System;
using System.Web;

namespace Noodle.Web
{
    /// <summary>
    /// Helper cookie extensions
    /// </summary>
    /// <remarks></remarks>
    public static class CookieExtensions
    {
        /// <summary>
        /// Creates a cookie with given parameters.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="cookieName">Name of the cookie.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="expiryTime">The expiry time.</param>
        /// <remarks></remarks>
        public static void CreateCookie(this HttpResponse response, string cookieName, string key, string value, DateTime expiryTime)
        {
            try
            {
                var cookie = new HttpCookie(cookieName);
                cookie.Expires = expiryTime;
                cookie.Values[key] = value;
                response.Cookies.Add(cookie);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Adds the cookie.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="cookie">The cookie.</param>
        /// <remarks></remarks>
        public static void AddCookie(this HttpResponse response, HttpCookie cookie)
        {
            try
            {
                response.Cookies.Add(cookie);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Updates the specified cookie.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="cookieName">Name of the cookie.</param>
        /// <param name="key">The key.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="expiryTime">The expiry time.</param>
        /// <param name="subDomainName">Name of the sub domain.</param>
        /// <remarks></remarks>
        public static void UpdateCookie(this HttpResponse response, string cookieName, string key, string newValue, DateTime? expiryTime = null,
            string subDomainName = "")
        {
            try
            {
                HttpCookie cookie = null;

                if (response.Cookies[cookieName] == null)
                {
                    throw new NullReferenceException("The cookie with name " + cookieName + " does not exist.");
                }
                else
                {
                    cookie = response.Cookies[cookieName];
                }

                cookie.Values[key] = newValue;

                if (expiryTime.HasValue)
                {
                    cookie.Expires = expiryTime.Value;
                }

                if (!string.IsNullOrEmpty(subDomainName.Trim()))
                {
                    cookie.Domain = subDomainName;
                }

                response.Cookies.Set(cookie);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets the cookie value.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cookieName">Name of the cookie.</param>
        /// <param name="valueKey">The key.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string GetCookieValue(this HttpRequest request, string cookieName, string valueKey)
        {
            string cookieValue = string.Empty;
            try
            {
                cookieValue = request.Cookies[cookieName].Values[valueKey];
            }
            catch (Exception ex)
            {
                cookieValue = string.Empty;
                throw ex;
            }
            return cookieValue;
        }

        /// <summary>
        /// Deletes the specified cookie.
        /// </summary>
        /// <param name="resposne">The resposne.</param>
        /// <param name="cookieName">Name of the cookie.</param>
        /// <remarks></remarks>
        public static void DeleteCookie(this HttpResponse resposne, string cookieName)
        {
            try
            {
                resposne.Cookies[cookieName].Expires = DateTime.Today.AddMinutes(-1);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Check whether a cookie exists or not.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cookieName">Name of the cookie.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool CookieExists(this HttpRequest request, string cookieName)
        {
            try
            {
                var cookie = request.Cookies[cookieName];
                if (cookie == null)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
