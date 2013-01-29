using Noodle.Engine;
using SimpleInjector;

namespace Noodle.MongoDB
{
    /// <summary>
    /// Register mongodb service
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(Container container, ITypeFinder typeFinder, Configuration.ConfigurationManagerWrapper configuration)
        {
            container.RegisterSingle<IMongoService, MongoService>();
        }

        public int Importance
        {
            get { return 0; }
        }
    }
}
