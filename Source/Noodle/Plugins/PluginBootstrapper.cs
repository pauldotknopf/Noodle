using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Noodle.Engine;

namespace Noodle.Plugins
{
	/// <summary>
	/// Finds plugins and calls their initializer.
	/// </summary>
	public class PluginBootstrapper : IPluginBootstrapper
	{
		private readonly ITypeFinder _typeFinder;

        public PluginBootstrapper(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
		}

		/// <summary>Gets plugins in the current app domain using the type finder.</summary>
		/// <returns>An enumeration of available plugins.</returns>
		public IEnumerable<IPluginDefinition> GetPluginDefinitions()
		{
			var pluginDefinitions = new List<IPluginDefinition>();

			// assembly defined plugins
			foreach (ICustomAttributeProvider assembly in _typeFinder.GetAssemblies())
			{
			    pluginDefinitions.AddRange(assembly.GetCustomAttributes(typeof (PluginAttribute), false).Cast<IPluginDefinition>());
			}
			
			// autoinitialize plugins
			foreach(var type in _typeFinder.Find(typeof(IPluginInitializer)))
			{
				foreach (AutoInitializeAttribute plugin in type.GetCustomAttributes(typeof(AutoInitializeAttribute), true))
				{
					plugin.InitializerType = type;

					pluginDefinitions.Add(plugin);
				}
			}

			return pluginDefinitions;
		}



		/// <summary>
		/// Invokes the initialize method on the supplied plugins.
		/// </summary>
        public void InitializePlugins(TinyIoCContainer container)
		{
		    var plugins = GetPluginDefinitions();
            var exceptions = new List<Exception>();
            foreach (var plugin in plugins)
            {
                try
                {
                    plugin.Initialize(container);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            if (exceptions.Count > 0)
            {
                var message = "While initializing {0} plugin(s) threw an exception. Please review the stack trace to find out what went wrong.";
                message = string.Format(message, exceptions.Count);
                message = exceptions.Aggregate(message, (current, ex) => current + (Environment.NewLine + Environment.NewLine + "- " + ex.Message));
                throw new PluginInitializationException(message, exceptions.ToArray());
            }
		}
	}
}
