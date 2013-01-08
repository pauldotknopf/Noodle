using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Noodle.Serialization
{
    public class BinaryStringSerializer : ISerializer
    {
        public string Serialize<T>(T item)
        {
            using (var stream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(stream, item);
                return Convert.ToBase64String(stream.ToArray());
            }
        }

        public T Deserialize<T>(string content)
        {
            byte[] bytes = Convert.FromBase64String(content);

            using (var stream = new MemoryStream(bytes))
            {
                return (T)(new BinaryFormatter().Deserialize(stream));
            }
        }
    }
}
