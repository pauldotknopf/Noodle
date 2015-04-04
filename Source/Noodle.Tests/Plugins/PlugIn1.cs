using Noodle.Extensions.Plugins;
using Noodle.Tests.Plugins;

[assembly:Plugin("Testplugin", "testplugin", typeof(PlugIn1))]

namespace Noodle.Tests.Plugins
{
	public class PlugIn1 : IPluginInitializer
	{
        public static bool WasInitialized { get; set; }

        public void Initialize(TinyIoCContainer kernel)
		{
            WasInitialized = true;
		}
	}
}
