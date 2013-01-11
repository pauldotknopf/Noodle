using Ninject;
using Noodle.Plugins;

namespace Noodle.Tests.Plugins
{
	public class PlugIn3 : IPluginInitializer
	{
        public static bool WasInitialized { get; set; }

		public void Initialize(IKernel kernel)
		{
            WasInitialized = true;
		}
	}
}
