using System;

namespace Noodle.Extensions.Plugins
{
	/// <summary>
	/// Marks a plugin initializer as eligible for auto initialization.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class AutoInitializeAttribute : InitializerCreatingAttribute
	{
	}
}