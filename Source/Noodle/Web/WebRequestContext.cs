using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;

namespace Noodle.Web
{
    /// <summary>
    /// A request context class that interacts with HttpContext.Current.
    /// </summary>
    public class WebRequestContext : IRequestContext, IDisposable
    {
        private string _unknownIP = "0.0.0.0";
        private Regex _ipAddress = new Regex(@"\b([0-9]{1,3}\.){3}[0-9]{1,3}$", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        /// <summary>Provides access to HttpContext.Current.</summary>
        protected virtual HttpContext CurrentHttpContext
        {
            get
            {
                if (HttpContext.Current == null)
                    throw new NoodleException("Tried to retrieve HttpContext.Current but it's null. This may happen when working outside a request or when doing stuff after the context has been recycled.");
                return HttpContext.Current;
            }
        }

        public bool IsWeb
        {
            get { return true; }
        }

        /// <summary>Gets a dictionary of request scoped items.</summary>
        public IDictionary RequestItems
        {
            get { return CurrentHttpContext.Items; }
        }

        public Url Url
        {
            get { return new Url(CurrentHttpContext.Request.Url.Scheme, CurrentHttpContext.Request.Url.Authority, CurrentHttpContext.Request.RawUrl); }
        }

        /// <summary>Maps a virtual path to a physical disk path.</summary>
        /// <param name="path">The path to map. E.g. "~/bin"</param>
        /// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
        public string MapPath(string path)
        {
            return HostingEnvironment.MapPath(path);
        }

        /// <summary>
        /// Get context IP address -- handles proxies and private networks
        /// </summary>
        /// <returns>URL referrer</returns>
        public virtual string GetCurrentIpAddress()
        {
            var ip = CurrentHttpContext.Request.ServerVariables["REMOTE_ADDR"]; // could be a proxy -- beware
            var ipForwarded = CurrentHttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            // check if we were forwarded from a proxy
            if (ipForwarded.HasValue())
            {
                ipForwarded = _ipAddress.Match(ipForwarded).Value;
                if (ipForwarded.HasValue() && !IsPrivateIP(ipForwarded))
                    ip = ipForwarded;
            }

            return ip.HasValue() ? ip : _unknownIP;

            return CurrentHttpContext.Request.UserHostAddress ?? string.Empty;
        }

        public NameValueCollection ServerVariables
        {
            get { return CurrentHttpContext.Request.ServerVariables; }
        }

        public NameValueCollection QueryString
        {
            get { return CurrentHttpContext.Request.QueryString; }
        }

        public NameValueCollection Form
        {
            get { return CurrentHttpContext.Request.Form; }
        }

        public IList<HttpCookie> Cookies
        {
            get
            {
                return CurrentHttpContext.Request.Cookies.AllKeys.Select(x => CurrentHttpContext.Request.Cookies[x]).ToList();
            }
        }

        public string GetReferrerUrl()
        {
            string referrerUrl = string.Empty;

            if (CurrentHttpContext.Request.UrlReferrer != null)
                referrerUrl = CurrentHttpContext.Request.UrlReferrer.ToString();

            return referrerUrl;
        }

        /// <summary>Disposes request items that needs disposing. This method should be called at the end of each request.</summary>
        public virtual void Close()
        {
            var keys = new object[RequestItems.Keys.Count];
            RequestItems.Keys.CopyTo(keys, 0);

            foreach (var value in keys.Select(key => RequestItems[key]).OfType<IClosable>())
            {
                value.Dispose();
            }
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            Close();
        }

        #endregion

        #region Helpers

        /// <summary>
        /// returns true if this is a private network IP  
        /// http://en.wikipedia.org/wiki/Private_network
        /// </summary>
        private static bool IsPrivateIP(string s)
        {
            return (s.StartsWith("192.168.") || s.StartsWith("10.") || s.StartsWith("127.0.0."));
        }

        #endregion

    }
}
