using Noodle.Plugins;
using SimpleInjector;

namespace Noodle.Tests.Plugins
{
	[AutoInitialize]
	public class PlugIn2 : IPluginInitializer
	{
        public static bool WasInitialized { get; set; }

		public void Initialize(Container container)
		{
            WasInitialized = true;
		}
	}
}
