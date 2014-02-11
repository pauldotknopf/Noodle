using MongoDB.Bson.Serialization.Attributes;

namespace Noodle.Settings
{
    [BsonIgnoreExtraElements]
    public class TypedSettings<T> : Setting
        where T : ISettings, new()
    {
        public TypedSettings()
        {
            Settings = new T();
        }

        public T Settings { get; set; }
    }
}
