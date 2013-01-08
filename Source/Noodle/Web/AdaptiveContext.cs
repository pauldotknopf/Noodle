using System;
using System.Collections;
using System.Collections.Generic;

namespace Noodle.Web
{
    /// <summary>
    /// A web context wrapper that maps to the web request context for calls in
    /// web application scope and thread context when no request has been made
    /// (e.g. when executing code in scheduled action).
    /// </summary>
    /// <remarks></remarks>
    public class AdaptiveContext : IRequestContext, IDisposable
    {
        readonly IRequestContext _thread;
        readonly IRequestContext _web;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        /// <remarks></remarks>
        public AdaptiveContext()
        {
            _thread = new ThreadContext();
            _web = new WebRequestContext();
        }

        /// <summary>
        /// Gets whether there is a web context available.
        /// </summary>
        /// <remarks></remarks>
        public bool IsWeb
        {
            get { return System.Web.HttpContext.Current != null; }
        }

        /// <summary>
        /// Returns either the web or the thread context depending on <see cref="IsWeb"/>.
        /// </summary>
        /// <remarks></remarks>
        protected IRequestContext CurrentContext
        {
            get { return IsWeb ? _web : _thread; }
        }

        /// <summary>
        /// Gets a dictionary of request scoped items.
        /// </summary>
        /// <remarks></remarks>
        public IDictionary RequestItems
        {
            get { return CurrentContext.RequestItems; }
        }

        /// <summary>
        /// The host part of the requested url, e.g. http://method.com/path/to/a/page.aspx?some=query.
        /// </summary>
        /// <remarks></remarks>
        public Url Url
        {
            get { return CurrentContext.Url; }
        }

        /// <summary>
        /// Maps a virtual path to a physical disk path.
        /// </summary>
        /// <param name="path">The path to map. E.g. "~/bin"</param>
        /// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
        /// <remarks></remarks>
        public string MapPath(string path)
        {
            return CurrentContext.MapPath(path);
        }

        /// <summary>
        /// Get context IP address
        /// </summary>
        /// <returns>IP Address</returns>
        /// <remarks></remarks>
        public string GetCurrentIpAddress()
        {
            return CurrentContext.GetCurrentIpAddress();
        }

        public string GetReferrerUrl()
        {
            return CurrentContext.GetReferrerUrl();
        }

        public System.Collections.Specialized.NameValueCollection ServerVariables
        {
            get { return CurrentContext.ServerVariables; }
        }

        public System.Collections.Specialized.NameValueCollection QueryString
        {
            get { return CurrentContext.QueryString; }
        }

        public System.Collections.Specialized.NameValueCollection Form
        {
            get { return CurrentContext.Form; }
        }

        public IList<System.Web.HttpCookie> Cookies
        {
            get { return CurrentContext.Cookies; }
        }

        /// <summary>
        /// Disposes request items that needs disposing. This method should be called at the end of each request.
        /// </summary>
        /// <remarks></remarks>
        public void Close()
        {
            CurrentContext.Close();
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <remarks></remarks>
        void IDisposable.Dispose()
        {
            Close();
        }

        #endregion
    }
}
