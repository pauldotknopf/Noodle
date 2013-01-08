namespace Noodle.Serialization
{
    public class JavaScriptSerializer : ISerializer
    {
        public string Serialize<T>(T item)
        {
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            return serializer.Serialize(item);
        }

        public T Deserialize<T>(string content)
        {
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            return serializer.Deserialize<T>(content);
        }
    }
}
