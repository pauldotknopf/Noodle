using System;

namespace Noodle.Extensions.Plugins
{
	/// <summary>
	/// Base class for attributes which can create an instance of the plugin 
	/// initializer and execute it.
	/// </summary>
	public abstract class InitializerCreatingAttribute : Attribute, IPluginDefinition
	{
	    public InitializerCreatingAttribute()
	    {
	        InitializerType = null;
	    }

	    public Type InitializerType { get; set; }

        public virtual void Initialize(TinyIoCContainer container)
		{
			if (InitializerType == null) throw new ArgumentNullException("InitializerType");

            CreateInitializer().Initialize(container);
		}

		/// <summary>Creates an instance of the initializer defined by this attribute.</summary>
		/// <returns>A new initializer instance.</returns>
		protected virtual IPluginInitializer CreateInitializer()
		{
			return (IPluginInitializer)Activator.CreateInstance(InitializerType);
		} 

	}
}
