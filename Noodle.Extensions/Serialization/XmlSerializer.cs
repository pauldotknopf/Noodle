using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Noodle.Extensions.Serialization
{
    /// <summary>
    /// Serialies to and from xml
    /// </summary>
    public class XmlSerializer : ISerializer
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
            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, new UTF8Encoding());
            var serializer = new System.Xml.Serialization.XmlSerializer(item.GetType());
            writer.Formatting = Formatting.Indented;
            writer.IndentChar = ' ';
            writer.Indentation = 3;
            serializer.Serialize(writer, item);
            string xmlResultString = Encoding.UTF8.GetString(ms.ToArray(), 0, (int)ms.Length);
            ms.Close();
            writer.Close();
            return xmlResultString;
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
            var requiersDeclaration = !content.StartsWith("<?xml", StringComparison.InvariantCultureIgnoreCase);
            var formatted = CommonHelper.FormatXml(requiersDeclaration ? "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + content : content);
            using (var reader = new XmlTextReader(formatted, XmlNodeType.Document, null))
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(reader);
            }
        }
    }
}
