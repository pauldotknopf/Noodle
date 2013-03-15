using Noodle.Engine;

namespace Noodle.MongoDB
{
    /// <summary>
    /// Register mongodb service
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(TinyIoCContainer container)
        {
            container.Register<IMongoService, MongoService>();
        }

        public int Importance
        {
            get { return 0; }
        }
    }
}
