using Noodle.Extensions.Serialization;

namespace Noodle.Web
{
    /// <summary>
    /// Javascript (json) serializer
    /// </summary>
    public class JavaScriptSerializer : ISerializer
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
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            return serializer.Serialize(item);
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
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            return serializer.Deserialize<T>(content);
        }
    }
}
