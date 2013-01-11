
using Ninject;

namespace Noodle.Plugins
{
	/// <summary>
	/// Finds plugins and calls their initializer.
	/// </summary>
	public interface IPluginBootstrapper
	{
        void InitializePlugins(IKernel kernel);
	}
}