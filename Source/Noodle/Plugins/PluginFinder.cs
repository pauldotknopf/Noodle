using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using Noodle.Configuration;
using Noodle.Engine;
using Noodle.Security;

namespace Noodle.Plugins
{
	/// <summary>
	/// Investigates the execution environment to find plugins.
	/// </summary>
    public class PluginFinder : IPluginFinder
    {
        private readonly IList<IPlugin> _plugins;
        private readonly ITypeFinder _typeFinder;
        private readonly TinyIoCContainer _container;
	    private readonly IEnumerable<PluginElement> _removedPlugins = new PluginElement[0];

        public PluginFinder(ITypeFinder typeFinder, NoodleCoreConfiguration config, TinyIoCContainer container)
        {
            _removedPlugins = config.Plugins.RemovedElements;
			_typeFinder = typeFinder;
            _container = container;
            _plugins = FindPlugins();
		}

		/// <summary>Gets plugins found in the environment sorted and filtered by the given user.</summary>
    	/// <typeparam name="T">The type of plugin to get.</typeparam>
    	/// <param name="user">The user that should be authorized for the plugin.</param>
    	/// <returns>An enumeration of plugins.</returns>
    	public IEnumerable<T> GetPlugins<T>(IPrincipal user) where T : class, IPlugin
		{
            return GetPlugins<T>().Where(plugin => _container.Resolve<ISecurityManager>().IsAuthorized(plugin as ISecurableBase, user));
		}

	    public IEnumerable<T> GetPlugins<T>() where T : class, IPlugin
	    {
	        return _plugins.OfType<T>().Select(plugin => plugin);
	    }

        /// <summary>Gets plugins found in the environment sorted and filtered by the given user.</summary>
        /// <typeparam name="T">The type of plugin to get.</typeparam>
        /// <typeparam name="TO">The type expected on the plugin decorations. Instantiated through the kernel.</typeparam>
        /// <param name="user">The user that should be authorized for the plugin.</param>
        /// <returns>An enumeration of plugins.</returns>
        public IEnumerable<TO> GetPlugins<T, TO>(IPrincipal user)
            where T : class, IPlugin
            where TO : class
        {
            return _plugins.OfType<T>().Where(plugin => _container.Resolve<ISecurityManager>().IsAuthorized(plugin as ISecurableBase, user)).Select(CreatePluginTo<TO>);
        }

        /// <summary>Gets plugins found in the environment sorted and filtered by the given user.</summary>
        /// <typeparam name="T">The type of plugin to get.</typeparam>
        /// <typeparam name="TO">The type expected on the plugin decorations. Instantiated through the kernel.</typeparam>
        /// <returns>An enumeration of plugins.</returns>
        public IEnumerable<TO> GetPlugins<T, TO>() where T : class, IPlugin where TO : class
        {
            return _plugins.OfType<T>().Select(CreatePluginTo<TO>);
        }

	    /// <summary>Finds and sorts plugin defined in known assemblies.</summary>
        /// <returns>A sorted list of plugins.</returns>
        protected virtual IList<IPlugin> FindPlugins()
        {
            var foundPlugins = new List<IPlugin>();
            foreach (var assembly in _typeFinder.GetAssemblies())
            {
                foreach (var plugin in FindPluginsIn(assembly))
                {
                    if (plugin.Name == null)
                        throw new NoodleException("A plugin in the assembly '{0}' has no name. The plugin is likely defined on the assembly ([assembly:...]). Try assigning the plugin a unique name and recompiling.", assembly.FullName);
                	if (foundPlugins.Any(x => x.Name.Equals(plugin.Name, StringComparison.InvariantCultureIgnoreCase) 
                        && x.GetType().FullName.Equals(plugin.GetType().FullName, StringComparison.InvariantCultureIgnoreCase)))
                        throw new NoodleException("A plugin of the type '{0}' named '{1}' is already defined, assembly: {2}", plugin.GetType().FullName, plugin.Name, assembly.FullName);

					if(!IsRemoved(plugin))
                		foundPlugins.Add(plugin);
                }
            }
            foundPlugins.Sort((first, second) => first.CompareTo(second));
            return foundPlugins;
        }

		private bool IsRemoved(IPlugin plugin)
		{
		    return _removedPlugins.Any(configElement => plugin.Name == configElement.Name);
		}

	    private IEnumerable<IPlugin> FindPluginsIn(Assembly a)
        {
            foreach (IPlugin attribute in a.GetCustomAttributes(typeof(IPlugin), false))
            {
                yield return attribute;
            }
            foreach (var t in a.GetTypes())
            {
                foreach (IPlugin attribute in t.GetCustomAttributes(typeof(IPlugin), false))
                {
                    if (attribute.Name == null)
                        attribute.Name = t.Name;
                    attribute.Decorates = t;

                    yield return attribute;
                }
            }
        }

        private TO CreatePluginTo<TO>(IPlugin plugin) where TO : class
        {
            // TODO: Resolve unregistered?
            var instance = Activator.CreateInstance(plugin.Decorates);
            var destinationType = instance as TO;
            if (destinationType == null)
                throw new InvalidOperationException(
                    "The plugin '{0}' is trying to be instantied the decorated type {1} to {2} but it is of type {3}. Be sure that the decorated type can be casted to {2}."
                        .F(plugin.Name, plugin.Decorates.Name, typeof(TO).FullName,
                           instance.GetType().Name));
            return destinationType;
        }
    }
}
