using System.Web;

namespace Noodle.Web
{
    /// <summary>
    /// A HttpModule that ensures that the engine is initialized with a web 
    /// context.
    /// </summary>
    public class InitializerModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            EventBroker.Instance.Attach(context);
            EngineContext.Configure(false);
        }

        public void Dispose()
        {
        }
    }
}
