using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Web;

namespace Noodle.Web
{
    /// <summary>
    /// A thread local context.
    /// </summary>
    public class ThreadContext : IRequestContext, IDisposable
    {
        private static readonly string BaseDirectory;
        [ThreadStatic]
        static IDictionary _items;
        [ThreadStatic]
        static Url _localUrl = new Url("/");
        [ThreadStatic]
        static Url _url = new Url("http://localhost");

        static ThreadContext()
        {
            BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            int binIndex = BaseDirectory.IndexOf("\\bin\\", StringComparison.Ordinal);
            if (binIndex >= 0)
                BaseDirectory = BaseDirectory.Substring(0, binIndex);
            else if (BaseDirectory.EndsWith("\\bin"))
                BaseDirectory = BaseDirectory.Substring(0, BaseDirectory.Length - 4);
        }


        public virtual IDictionary RequestItems
        {
            get { return _items ?? (_items = new Hashtable()); }
        }

        public virtual bool IsWeb
        {
            get { return false; }
        }

        public virtual void Close()
        {
            var keys = new string[RequestItems.Keys.Count];
            RequestItems.Keys.CopyTo(keys, 0);

            foreach (string key in keys)
            {
                var value = RequestItems[key] as IClosable;
                if (value != null)
                {
                    value.Dispose();
                }
            }
            _items = null;
        }

        public virtual Url Url
        {
            get { return _url; }
            set { _url = value; }
        }

        public virtual string MapPath(string path)
        {
            path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');
            return Path.Combine(BaseDirectory, path);
        }

        public void ClearError()
        {
        }

        public string GetCurrentIpAddress()
        {
            return Environment.MachineName;
        }

        public string GetReferrerUrl()
        {
            return string.Empty;
        }

        public NameValueCollection ServerVariables
        {
            get { return new NameValueCollection(); }
        }

        public NameValueCollection QueryString
        {
            get { return new NameValueCollection(); }
        }

        public NameValueCollection Form
        {
            get { return new NameValueCollection(); }
        }

        public IList<HttpCookie> Cookies
        {
            get { return new List<HttpCookie>(); }
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            Close();
        }

        #endregion


    }
}
