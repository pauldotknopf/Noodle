using Noodle.TinyIoC;

namespace Noodle.Plugins
{
	/// <summary>
	/// Classes implementing this interface define plugins and are responsible of
	/// calling plugin initializers.
	/// </summary>
	public interface IPluginDefinition
	{
		/// <summary>Executes the plugin initializer.</summary>
        /// <param name="kernel">A reference to the kernel.</param>
		void Initialize(TinyIoCContainer kernel);
	}
}