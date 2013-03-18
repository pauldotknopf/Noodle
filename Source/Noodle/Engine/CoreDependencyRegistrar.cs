using Noodle.Caching;
using Noodle.Data;
using Noodle.Email;
using Noodle.Plugins;
using Noodle.Scheduling;
using Noodle.Security;
using Noodle.Serialization;
using Noodle;
using Noodle.Documentation;

namespace Noodle.Engine
{
    public static class CoreDependencyRegistrar
    {
        public static void Register(TinyIoCContainer container)
        {
            container.Register<IWorker, AsyncWorker>();
            container.Register<ITypeFinder, AppDomainTypeFinder>();
            container.Register<IAssemblyFinder, AssemblyFinder>();
            container.Register<ISerializer, BinaryStringSerializer>();
            container.Register<IEncryptionService, EncryptionService>();
            container.Register<IDateTimeHelper, DateTimeHelper>();
            container.Register<IDatabaseService, DatabaseService>();
            container.Register<IEmailSender, EmailSender>();
            container.Register<ISecurityManager, DefaultSecurityManager>();
            container.Register<IPluginBootstrapper, PluginBootstrapper>();
            container.Register<IPluginFinder, PluginFinder>();
            container.Register<IHeart, Heart>();
            container.Register<IErrorNotifier, ErrorNotifier>();
            container.Register<IConnectionProvider, NoConnectionProvider>();
            container.Register<ICacheManager, InMemoryCache>();
            container.Register<Scheduler>();
            container.Register<IDocumentationService, DocumentationService>();
        }
    }
}
