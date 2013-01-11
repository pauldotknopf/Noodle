using Ninject;
using Noodle.Plugins;

namespace Noodle.Tests.Plugins
{
	[AutoInitialize]
	public class PlugIn2 : IPluginInitializer
	{
        public static bool WasInitialized { get; set; }

		public void Initialize(IKernel kernel)
		{
            WasInitialized = true;
		}
	}
}
