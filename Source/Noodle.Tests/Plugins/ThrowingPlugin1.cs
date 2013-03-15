using Noodle.Plugins;

namespace Noodle.Tests.Plugins
{
    [AutoInitialize]
    public class ThrowingPlugin1 : IPluginInitializer
    {
		public static bool WasInitialized { get; set; }
        public static bool Throw { get; set; }
        public void Initialize(TinyIoCContainer container)
        {
        	WasInitialized = true;
            if (Throw)
                throw new SomeException("ThrowingPlugin1 isn't happy.");
        }
    }
}
