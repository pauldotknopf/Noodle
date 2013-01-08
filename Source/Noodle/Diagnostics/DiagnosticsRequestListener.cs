using System;
using System.Linq;
using System.Web;
using Noodle.Engine;
using Noodle.Web;

namespace Noodle.Diagnostics
{
    [Service]
    public class DiagnosticsRequestListener : IStartupTask
    {
        public void Execute()
        {
            EventBroker.Instance.PostResolveAnyRequestCache += (sender, e) =>
            {
                var httpApplication = sender as HttpApplication;

                if (httpApplication == null)
                    return;

                if (new Url(httpApplication.Request.RawUrl).Path.Equals("/method/diagnostics", StringComparison.InvariantCultureIgnoreCase))
                {
                    var diagnosticsPage = new Diagnostics
                    {
                        DiagnosticsPlugins = EngineContext.Resolve<Plugins.IPluginFinder>()
                          .GetPlugins<DiagnosticsPluginAttribute, DiagnosticsPlugin>().ToList()
                    };
                    httpApplication.Context.RemapHandler(diagnosticsPage);
                }
            };
        }

        public int Order
        {
            get { return 0; }
        }
    }
}
