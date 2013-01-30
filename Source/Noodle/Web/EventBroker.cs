using System;
using System.Diagnostics;
using System.Web;
using Noodle.Engine;

namespace Noodle.Web
{
    /// <summary>
    /// A broker for events from the http application.
    /// </summary>
    public class EventBroker
    {
        static EventBroker()
        {
            Instance = new EventBroker();
        }

        /// <summary>Accesses the event broker singleton instance.</summary>
        public static EventBroker Instance
        {
            get { return Singleton<EventBroker>.Instance; }
            protected set { Singleton<EventBroker>.Instance = value; }
        }

        /// <summary>Attaches to events from the application instance.</summary>
        public virtual void Attach(HttpApplication application)
        {
            Trace.WriteLine("EventBroker: Attaching to " + application);

            application.BeginRequest += ApplicationBeginRequest;
            application.AuthorizeRequest += ApplicationAuthorizeRequest;

            application.PostResolveRequestCache += ApplicationPostResolveRequestCache;

            application.AcquireRequestState += ApplicationAcquireRequestState;
            application.Error += ApplicationError;
            application.EndRequest += ApplicationEndRequest;

            application.Disposed += Application_Disposed;
        }

        public EventHandler<EventArgs> BeginRequest;
        public EventHandler<EventArgs> AuthorizeRequest;
        public EventHandler<EventArgs> PostResolveRequestCache;
        public EventHandler<EventArgs> PostResolveAnyRequestCache;
        public EventHandler<EventArgs> AcquireRequestState;
        public EventHandler<EventArgs> Error;
        public EventHandler<EventArgs> EndRequest;

        protected void ApplicationBeginRequest(object sender, EventArgs e)
        {
            if (BeginRequest != null && !IsStaticResource(sender))
            {
                Debug.WriteLine("BeginRequest");
                BeginRequest(sender, e);
            }
        }

        protected void ApplicationAuthorizeRequest(object sender, EventArgs e)
        {
            if (AuthorizeRequest != null && !IsStaticResource(sender))
            {
                Debug.WriteLine("AuthorizeRequest");
                AuthorizeRequest(sender, e);
            }
        }

        private void ApplicationPostResolveRequestCache(object sender, EventArgs e)
        {
            if (PostResolveRequestCache != null && !IsStaticResource(sender))
            {
                Debug.WriteLine("PostResolveRequestCache");
                PostResolveRequestCache(sender, e);
            }
            if (PostResolveAnyRequestCache != null)
            {
                Debug.WriteLine("PostResolveAnyRequestCache");
                PostResolveAnyRequestCache(sender, e);
            }
        }

        protected void ApplicationAcquireRequestState(object sender, EventArgs e)
        {
            if (AcquireRequestState != null && !IsStaticResource(sender))
            {
                Debug.WriteLine("AcquireRequestState");
                AcquireRequestState(sender, e);
            }
        }

        protected void ApplicationError(object sender, EventArgs e)
        {
            if (Error != null && !IsStaticResource(sender))
                Error(sender, e);
        }

        protected void ApplicationEndRequest(object sender, EventArgs e)
        {
            if (EndRequest != null && !IsStaticResource(sender))
                EndRequest(sender, e);
        }

        /// <summary>Detaches events from the application instance.</summary>
        void Application_Disposed(object sender, EventArgs e)
        {
            Trace.WriteLine("EventBroker: Disposing " + sender);
        }

        /// <summary>Returns true if the requested resource is one of the typical resources that needn't be processed by the cms engine.</summary>
        /// <param name="sender">The event sender, probably a http application.</param>
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
        protected static bool IsStaticResource(object sender)
        {
            var application = sender as HttpApplication;
            if (application != null)
            {
                string path = application.Request.Path;
                return CommonHelper.IsStaticResource(path);
            }
            return false;
        }
    }
}
