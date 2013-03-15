using MongoDB.Driver;
using Noodle.Engine;
using Noodle.MongoDB;

namespace Noodle.Logging
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(TinyIoCContainer container)
        {
            container.Register<ILogger, DefaultLogger>();
            container.Register((context, p) => GetLocalizationDatabase(context).GetCollection<Log>("Log"));
            container.Register<ErrorNotifierLogger>().AsSingleton();
        }

        public int Importance
        {
            get { return 0; }
        }

        public static MongoDatabase GetLocalizationDatabase(TinyIoCContainer container)
        {
            return container.Resolve<IMongoService>().GetDatabase("Logging");
        }
    }
}
