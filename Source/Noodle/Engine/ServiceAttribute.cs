using System;

namespace Noodle.Engine
{
    /// <summary>
    /// Markes a service that is registered in automatically registered in Noodle's container (ninject).
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ServiceAttribute : Attribute
    {
        public ServiceAttribute()
        {
            ContainerScope = ContainerScopeEnum.Default;
        }

        public ServiceAttribute(Type serviceType)
        {
            ServiceType = serviceType;
            ContainerScope = ContainerScopeEnum.Default;
        }

        /// <summary>
        /// The type of service the attributed class represents.
        /// </summary>
        public Type ServiceType { get; protected set; }

        /// <summary>
        /// Optional key to associate with the service.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// A static accessor property used to retrieve the service instance instead of instiatating it.
        /// </summary>
        public string StaticAccessor { get; set; }

        /// <summary>
        /// The scope for the service (singleton, prerequest, etc)
        /// </summary>
        public ContainerScopeEnum ContainerScope { get; set; }

        /// <summary>
        /// A service defined by a <see cref="ServiceAttribute"/> to replace with the attributed implementation.
        /// </summary>
        public Type Replaces { get; set; }
    }
}
