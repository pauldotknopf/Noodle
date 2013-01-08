
namespace Noodle.Plugins
{
	/// <summary>Classes implementing this interface can serve as plug in initializers. 
	/// If one of these classes is referenced by a PlugInAttribute it's initialize methods will be invoked by the Noodle factory during initialization.</summary>
	public interface IPluginInitializer
	{
		/// <summary>Invoked after the factory has been initialized.</summary>
		/// <param name="kernel">The kernel that has been initialized.</param>
		void Initialize(TinyIoC.TinyIoCContainer kernel);
	}
}
