using Noodle.Plugins;

namespace Noodle.Tests.Plugins
{
	public class PlugIn3 : IPluginInitializer
	{
        public static bool WasInitialized { get; set; }

		public void Initialize(TinyIoC.TinyIoCContainer kernel)
		{
            WasInitialized = true;
		}
	}
}
