using System.Collections.Generic;
using System.Security.Principal;

namespace Noodle.Extensions.Plugins
{
    public interface IPluginFinder
    {
        /// <summary>Gets plugins found in the environment sorted and filtered by the given user.</summary>
        /// <typeparam name="T">The type of plugin to get.</typeparam>
        /// <param name="user">The user that should be authorized for the plugin.</param>
		/// <returns>An enumeration of plugins.</returns>
        IEnumerable<T> GetPlugins<T>(IPrincipal user) where T : class, IPlugin;

        /// <summary>Gets plugins found in the environment sorted.</summary>
        /// <typeparam name="T">The type of plugin to get.</typeparam>
        /// <returns>An enumeration of plugins.</returns>
        IEnumerable<T> GetPlugins<T>() where T : class, IPlugin;

        /// <summary>Gets plugins found in the environment sorted and filtered by the given user.</summary>
        /// <typeparam name="T">The type of plugin to get.</typeparam>
        /// <typeparam name="TO">The type expected on the plugin decorations. Instantiated through the kernel.</typeparam>
        /// <returns>An enumeration of plugins.</returns>
        IEnumerable<TO> GetPlugins<T, TO>()
            where T : class, IPlugin
            where TO : class;

        /// <summary>Gets plugins found in the environment sorted and filtered by the given user.</summary>
        /// <typeparam name="T">The type of plugin to get.</typeparam>
        /// <typeparam name="TO">The type expected on the plugin decorations. Instantiated through the kernel.</typeparam>
        /// <param name="user">The user that should be authorized for the plugin.</param>
        /// <returns>An enumeration of plugins.</returns>
        IEnumerable<TO> GetPlugins<T, TO>(IPrincipal user)
            where T : class, IPlugin
            where TO : class;
    }
}
