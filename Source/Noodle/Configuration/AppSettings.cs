using System.Collections.Specialized;

namespace Noodle.Configuration
{
    /// <summary>
    /// App settings collection used for dependency injection
    /// </summary>
    public class AppSettings : NameValueCollection
    {
        private readonly NameValueCollection _appSettings;

        public AppSettings(NameValueCollection appSettings)
        {
            _appSettings = appSettings;
        }

        public override void Add(string name, string value)
        {
            _appSettings.Add(name, value);
        }

        public override string[] AllKeys
        {
            get
            {
                return _appSettings.AllKeys;
            }
        }

        public override int Count
        {
            get
            {
                return _appSettings.Count;
            }
        }

        public override void Clear()
        {
            _appSettings.Clear();
        }

        public override bool Equals(object obj)
        {
            return _appSettings.Equals(obj);
        }

        public override string Get(int index)
        {
            return _appSettings.Get(index);
        }

        public override string Get(string name)
        {
            return _appSettings.Get(name);
        }

        public override System.Collections.IEnumerator GetEnumerator()
        {
            return _appSettings.GetEnumerator();
        }

        public override int GetHashCode()
        {
            return _appSettings.GetHashCode();
        }

        public override string GetKey(int index)
        {
            return _appSettings.GetKey(index);
        }

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            _appSettings.GetObjectData(info, context);
        }

        public override string[] GetValues(int index)
        {
            return _appSettings.GetValues(index);
        }

        public override string[] GetValues(string name)
        {
            return _appSettings.GetValues(name);
        }

        public override KeysCollection Keys
        {
            get
            {
                return _appSettings.Keys;
            }
        }

        public override void OnDeserialization(object sender)
        {
            _appSettings.OnDeserialization(sender);
        }

        public override void Remove(string name)
        {
            _appSettings.Remove(name);
        }

        public override void Set(string name, string value)
        {
            _appSettings.Set(name, value);
        }

        public override string ToString()
        {
            return _appSettings.ToString();
        }
    }
}
