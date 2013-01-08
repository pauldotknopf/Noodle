namespace Noodle.Data
{
    public abstract class ConnectionInitializer
    {
        public abstract void Initialize(string connection, string provider);
    }
}
