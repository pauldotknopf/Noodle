
using SimpleInjector;

namespace Noodle.Plugins
{
	/// <summary>Classes implementing this interface can serve as plug in initializers. 
	/// If one of these classes is referenced by a PlugInAttribute it's initialize methods will be invoked by the Noodle factory during initialization.</summary>
	public interface IPluginInitializer
	{
		/// <summary>Invoked after the factory has been initialized.</summary>
        /// <param name="container">The container that has been initialized.</param>
		void Initialize(Container container);
	}
}
