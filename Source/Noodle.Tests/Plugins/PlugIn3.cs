using Noodle.Plugins;
using SimpleInjector;

namespace Noodle.Tests.Plugins
{
	public class PlugIn3 : IPluginInitializer
	{
        public static bool WasInitialized { get; set; }

		public void Initialize(Container container)
		{
            WasInitialized = true;
		}
	}
}
