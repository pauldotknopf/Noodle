namespace Noodle.Plugins
{
	/// <summary>
	/// Classes implementing this interface define plugins and are responsible of
	/// calling plugin initializers.
	/// </summary>
	public interface IPluginDefinition
	{
		/// <summary>Executes the plugin initializer.</summary>
        /// <param name="container">A reference to the container.</param>
        void Initialize(TinyIoCContainer container);
	}
}