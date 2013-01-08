namespace Noodle.Engine
{
    /// <summary>
    /// The scope to register the service as
    /// </summary>
    public enum ContainerScopeEnum
    {
        /// <summary>
        /// Let the container handle it the default wawy
        /// </summary>
        Default,
        /// <summary>
        /// The service is created once pre request
        /// </summary>
        PerRequest,
        /// <summary>
        /// This service is created once and shared throughout the life time of the application
        /// </summary>
        Singleton,
        /// <summary>
        /// This service will be created once per thread
        /// </summary>
        Thread,
        /// <summary>
        /// This service will be transient
        /// </summary>
        Transient
    }
}
