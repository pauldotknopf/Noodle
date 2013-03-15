using Noodle.Plugins;

namespace Noodle.Tests.Plugins
{
    [AutoInitialize]
    public class ThrowingPlugin2 : IPluginInitializer
    {
		public static bool WasInitialized { get; set; }
		public static bool Throw { get; set; }
        public void Initialize(TinyIoCContainer container)
        {
        	WasInitialized = true;
            if (Throw)
                throw new NoodleException("ThrowingPlugin2 is really mad.");
        }
    }
}
