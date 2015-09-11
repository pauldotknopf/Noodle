namespace Noodle
{
    /// <summary>
    /// Implementations can use this to execute some code on app start.
    /// </summary>
    public interface IStartupTask
    {
        /// <summary>
        /// Excute is run once on startup of the application
        /// </summary>
        void Execute();

        /// <summary>
        /// The order at which the startup task will run. Smaller numbers run first.
        /// </summary>
        int Order { get; }
    }
}
