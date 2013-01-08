namespace Noodle.Events
{
    /// <summary>
    /// Implementations can handle events of type "T" which can be any .net type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IConsumer<T>
    {
        void Handle(T eventMessage);
    }
}
