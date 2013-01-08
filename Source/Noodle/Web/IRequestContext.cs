using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;

namespace Noodle.Web
{
    /// <summary>
    /// A mock-able interface for operations that targets the the http context.
    /// </summary>
    public interface IRequestContext
    {
        /// <summary>Gets wether there is a web context available.</summary>
        bool IsWeb { get; }

        /// <summary>Gets a dictionary of request scoped items.</summary>
        IDictionary RequestItems { get; }

        /// <summary>The local part of the requested path, e.g. http://noodle.com/path/to/a/page.aspx?some=query.</summary>
        Url Url { get; }

        /// <summary>Closes any end-able resources at the end of the request.</summary>
        void Close();

        /// <summary>Maps a virtual path to a physical disk path.</summary>
        /// <param name="path">The path to map. E.g. "~/bin"</param>
        /// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
        string MapPath(string path);

        /// <summary>
        /// Get context IP address
        /// </summary>
        /// <returns>IP Address</returns>
        string GetCurrentIpAddress();

        /// <summary>
        /// Gets the referring url
        /// </summary>
        /// <returns></returns>
        string GetReferrerUrl();

        /// <summary>
        /// Get all the server variables
        /// </summary>
        NameValueCollection ServerVariables { get; }

        /// <summary>
        /// Get all the query string values
        /// </summary>
        NameValueCollection QueryString { get; }

        /// <summary>
        /// Get all the form posted values
        /// </summary>
        NameValueCollection Form { get; }

        /// <summary>
        /// Get all the cookies assoicated to the request
        /// </summary>
        IList<HttpCookie> Cookies { get; }
    }
}
