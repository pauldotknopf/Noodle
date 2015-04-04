using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Noodle.Serialization
{
    /// <summary>
    /// Serializer that serializers to a binary string
    /// </summary>
    public class BinaryStringSerializer : ISerializer
    {
        /// <summary>
        /// Serialize the item to a string
        /// </summary>
        /// <typeparam name="T">The item type to serialize</typeparam>
        /// <param name="item">The item to serialize</param>
        /// <returns>
        /// The string representation of the object
        /// </returns>
        public string Serialize<T>(T item)
        {
            using (var stream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(stream, item);
                return Convert.ToBase64String(stream.ToArray());
            }
        }

        /// <summary>
        /// Deserializes a string to an instance of T
        /// </summary>
        /// <typeparam name="T">The type to deserialize the content to</typeparam>
        /// <param name="content">The content to serialize</param>
        /// <returns>
        /// The object with the string deserialized onto it
        /// </returns>
        public T Deserialize<T>(string content)
        {
            var bytes = Convert.FromBase64String(content);
            using (var stream = new MemoryStream(bytes))
                return (T)(new BinaryFormatter().Deserialize(stream));
        }
    }
}
