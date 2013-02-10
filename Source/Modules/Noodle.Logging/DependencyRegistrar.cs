using MongoDB.Driver;
using Noodle.Engine;
using Noodle.MongoDB;
using SimpleInjector;

namespace Noodle.Logging
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(Container container)
        {
            container.RegisterSingle<ILogger, DefaultLogger>();
            container.RegisterSingle(() => GetLocalizationDatabase(container).GetCollection<Log>("Log"));;
        }

        public int Importance
        {
            get { return 0; }
        }

        public static MongoDatabase GetLocalizationDatabase(Container container)
        {
            return container.GetInstance<IMongoService>().GetDatabase("Logging");
        }
    }
}
