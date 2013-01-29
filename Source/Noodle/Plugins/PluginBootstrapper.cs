using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Noodle.Configuration;
using Noodle.Engine;
using SimpleInjector;

namespace Noodle.Plugins
{
	/// <summary>
	/// Finds plugins and calls their initializer.
	/// </summary>
	public class PluginBootstrapper : IPluginBootstrapper
	{
		private readonly ITypeFinder _typeFinder;
        public IEnumerable<PluginElement> AddedInitializers = new PluginElement[0];
        public IEnumerable<PluginElement> RemovedInitializers = new PluginElement[0];

		public PluginBootstrapper(ITypeFinder typeFinder)
		{
			this._typeFinder = typeFinder;
		}
        public PluginBootstrapper(ITypeFinder typeFinder, NoodleCoreConfiguration config)
			: this(typeFinder)
		{
			AddedInitializers = config.PluginInitializers.AllElements;
			RemovedInitializers = config.PluginInitializers.RemovedElements;
		}

		/// <summary>Gets plugins in the current app domain using the type finder.</summary>
		/// <returns>An enumeration of available plugins.</returns>
		public IEnumerable<IPluginDefinition> GetPluginDefinitions()
		{
			var pluginDefinitions = new List<IPluginDefinition>();

			// assembly defined plugins
			foreach (ICustomAttributeProvider assembly in _typeFinder.GetAssemblies())
			{
			    pluginDefinitions.AddRange(assembly.GetCustomAttributes(typeof (PluginAttribute), false).Cast<PluginAttribute>()
                    .Where(plugin => !IsRemoved(plugin.InitializerType)).Cast<IPluginDefinition>());
			}
			
			// autoinitialize plugins
			foreach(var type in _typeFinder.Find(typeof(IPluginInitializer)))
			{
				foreach (AutoInitializeAttribute plugin in type.GetCustomAttributes(typeof(AutoInitializeAttribute), true))
				{
					plugin.InitializerType = type;

					if (!IsRemoved(type))
						pluginDefinitions.Add(plugin);
				}
			}
		
			// configured plugins
			foreach (var configElement in AddedInitializers)
			{
				var pluginType = Type.GetType(configElement.Type);
				if (pluginType == null)
                    throw new NoodleException("Could not find the configured plugin initializer type '{0}'", configElement.Type);
				if (typeof(IPluginDefinition).IsAssignableFrom(pluginType))
                    throw new NoodleException("The configured plugin initializer type '{0}' is not a valid plugin initializer since it doesn't implement the IPluginDefinition interface.", configElement.Type);

			    var plugin = new PluginAttribute
			                     {
			                         Name = configElement.Name,
			                         InitializerType = pluginType,
			                         Title = "Configured plugin " + configElement.Name
			                     };
			    pluginDefinitions.Add(plugin);
			}

			return pluginDefinitions;
		}



		/// <summary>
		/// Invokes the initialize method on the supplied plugins.
		/// </summary>
        public void InitializePlugins(Container container)
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

		private bool IsRemoved(Type pluginType)
		{
			foreach(var configElement in RemovedInitializers)
			{
				if(configElement.Name == pluginType.Name)
					return true;
				if(!string.IsNullOrEmpty(configElement.Type) && Type.GetType(configElement.Type) == pluginType)
					return true;
			}
			return false;
		}
	}
}
