using Noodle.Configuration;
using SimpleInjector;

namespace Noodle.Engine
{
    /// <summary>
    /// This class allows you to register dependencies dynamically while avoiding circular dependencies.
    /// </summary>
    public interface IDependencyRegistrar
    {
        /// <summary>
        /// Register your services with the container. You are given a type finder to help you find anything you need.
        /// </summary>
        void Register(Container container, ITypeFinder typeFinder, ConfigurationManagerWrapper configuration);

        /// <summary>
        /// The lower numbers will be registered first. Higher numbers the latest.
        /// If you are registering fakes, give a high integer (int.Max ?) so that that it will be registered last,
        ///     and the container will use it instead of the previously registered services.
        /// </summary>
        int Importance { get; }
    }
}
