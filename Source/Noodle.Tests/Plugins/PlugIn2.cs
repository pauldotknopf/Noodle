using Noodle.Plugins;

namespace Noodle.Tests.Plugins
{
	[AutoInitialize]
	public class PlugIn2 : IPluginInitializer
	{
        public static bool WasInitialized { get; set; }

		public void Initialize(TinyIoC.TinyIoCContainer kernel)
		{
            WasInitialized = true;
		}
	}
}
