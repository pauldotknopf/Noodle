using System;
using System.Web;

namespace Noodle.Web
{
    /// <summary>
    /// This is a helper class used to do quick/minor handling of http requests without requiring you to implement a full http handler.
    /// </summary>
    public class DelegateHttpHandler : IHttpHandler
    {
        private readonly Action<HttpContext> _action;
        private readonly bool _isReusable;

        public DelegateHttpHandler(Action<HttpContext> action, bool isReusable = true)
        {
            _action = action;
            _isReusable = isReusable;
        }

        public bool IsReusable
        {
            get { return _isReusable; }
        }

        public void ProcessRequest(HttpContext context)
        {
            _action(context);
        }
    }
}
