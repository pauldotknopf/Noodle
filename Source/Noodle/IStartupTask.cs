namespace Noodle
{
    /// <summary>
    /// Implementations can use this to execute some code on app start.
    /// </summary>
    public interface IStartupTask
    {
        void Execute();
        int Order { get; }
    }
}
