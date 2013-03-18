using System.Web;

namespace Noodle.Web
{
    /// <summary>
    /// A HttpModule that ensures that the engine is initialized with a web 
    /// context.
    /// </summary>
    public class InitializerModule : IHttpModule
    {
        /// <summary>
        /// Initializes a module and prepares it to handle requests.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpApplication" /> that provides access to the methods, properties, and events common to all application objects within an ASP.NET application</param>
        public void Init(HttpApplication context)
        {
            EventBroker.Instance.Attach(context);
            EngineContext.Configure(false);
        }

        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule" />.
        /// </summary>
        public void Dispose()
        {
        }
    }
}
