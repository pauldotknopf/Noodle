namespace Noodle.Extensions.Collections
{
    /// <summary>
    /// An item that has a name.
    /// </summary>
    public interface INameable
    {
        /// <summary>The name of the item.</summary>
        string Name { get; }
    }
}
