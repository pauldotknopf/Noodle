namespace Noodle.Extensions.Serialization
{
    /// <summary>
    /// Service responsible for serializing and deserializing strings to objects
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// Serialize the item to a string
        /// </summary>
        /// <typeparam name="T">The item type to serialize</typeparam>
        /// <param name="item">The item to serialize</param>
        /// <returns>The string representation of the object</returns>
        string Serialize<T>(T item);

        /// <summary>
        /// Deserializes a string to an instance of T
        /// </summary>
        /// <typeparam name="T">The type to deserialize the content to</typeparam>
        /// <param name="content">The content to serialize</param>
        /// <returns>The object with the string deserialized onto it</returns>
        T Deserialize<T>(string content);
    }
}
