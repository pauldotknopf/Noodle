using System;
using System.Diagnostics;
using System.IO;
using System.Web;

namespace Noodle.Web
{
    /// <summary>
    /// Represents a common helper
    /// </summary>
    public partial class WebHelper : IWebHelper
    {
        private readonly IRequestContext _requestContext;

        public WebHelper(IRequestContext requestContext)
        {
            _requestContext = requestContext;
        }

        /// <summary>
        /// Gets query string value by name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">Parameter name</param>
        /// <returns>Query string value</returns>
        public virtual T QueryString<T>(string name)
        {
            string queryParam = null;
            if (HttpContext.Current != null && HttpContext.Current.Request.QueryString[name] != null)
                queryParam = HttpContext.Current.Request.QueryString[name];

            if (!String.IsNullOrEmpty(queryParam))
                return CommonHelper.To<T>(queryParam);

            return default(T);
        }

        /// <summary>
        /// Restart application domain
        /// </summary>
        /// <param name="redirectUrl">Redirect URL; empty string if you want to redirect to the current page URL</param>
        public virtual void RestartAppDomain(string redirectUrl = "")
        {
            if (CommonHelper.GetTrustLevel() > AspNetHostingPermissionLevel.Medium)
            {
                //full trust
                HttpRuntime.UnloadAppDomain();
            }
            else
            {
                //medium trust
                bool success = TryWriteWebConfig();

                if (!success)
                {
                    throw new NoodleException("This application needs to be restarted due to a configuration change, but was unable to do so.\r\n" +
                        "To prevent this issue in the future, a change to the web server configuration is required:\r\n" +
                        "- run the application in a full trust environment, or\r\n" +
                        "- give the application write access to the 'web.config' file.");
                }
            }

            // If setting up extensions/modules requires an AppDomain restart, it's very unlikely the
            // current request can be processed correctly.  So, we redirect to the same URL, so that the
            // new request will come to the newly started AppDomain.
            var httpContext = HttpContext.Current;
            if (httpContext != null)
            {
                if (String.IsNullOrEmpty(redirectUrl))
                    redirectUrl = _requestContext.Url.ToString();
                httpContext.Response.Redirect(redirectUrl, true /*endResponse*/);
            }
        }

        private bool TryWriteWebConfig()
        {
            try
            {
                // In medium trust, "UnloadAppDomain" is not supported. Touch web.config
                // to force an AppDomain restart.
                File.SetLastWriteTimeUtc(_requestContext.MapPath("~/web.config"), DateTime.UtcNow);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Get a value indicating whether the request is made by search engine (web crawler)
        /// </summary>
        /// <returns>Result</returns>
        public virtual bool IsSearchEngine()
        {
            if (HttpContext.Current == null)
                return false;

            bool result = false;
            try
            {
                result = HttpContext.Current.Request.Browser.Crawler;
                if (!result)
                {
                    // TODO: Make more accurate
                    //put any additional known crawlers in the Regex below for some custom validation
                    //var regEx = new Regex("Twiceler|twiceler|BaiDuSpider|baduspider|Slurp|slurp|ask|Ask|Teoma|teoma|Yahoo|yahoo");
                    //result = regEx.Match(request.UserAgent).Success;
                }
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc);
            }
            return result;
        }
    }
}
