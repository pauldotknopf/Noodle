namespace Noodle.Extensions.Plugins
{
	/// <summary>
	/// Finds plugins and calls their initializer.
	/// </summary>
	public interface IPluginBootstrapper
	{
        void InitializePlugins(TinyIoCContainer container);
	}
}