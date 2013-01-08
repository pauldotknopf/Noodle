using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Noodle.Serialization
{
    public class XmlSerializer : ISerializer
    {
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
