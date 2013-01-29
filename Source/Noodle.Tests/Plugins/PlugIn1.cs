using Noodle.Plugins;
using Noodle.Tests.Plugins;
using SimpleInjector;

[assembly:Plugin("Testplugin", "testplugin", typeof(PlugIn1))]

namespace Noodle.Tests.Plugins
{
	public class PlugIn1 : IPluginInitializer
	{
        public static bool WasInitialized { get; set; }

        public void Initialize(Container kernel)
		{
            WasInitialized = true;
		}
	}
}
