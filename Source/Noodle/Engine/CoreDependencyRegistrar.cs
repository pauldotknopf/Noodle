

namespace Noodle.Engine
{
    public static class CoreDependencyRegistrar
    {
        public static void Register(TinyIoCContainer container)
        {
#if IOS
			container.Register(typeof(ITypeFinder), typeof(AppDomainTypeFinder));
			container.Register(typeof(IAssemblyFinder), typeof(AssemblyFinder));
#elif ANDROID
			container.Register(typeof(ITypeFinder), typeof(AppDomainTypeFinder));
			container.Register(typeof(IAssemblyFinder), typeof(AssemblyFinder));
#else
            container.Register(typeof(IWorker), typeof(AsyncWorker));
            container.Register(typeof(ITypeFinder), typeof(AppDomainTypeFinder));
            container.Register(typeof(IAssemblyFinder), typeof(AssemblyFinder));
            container.Register(typeof(Serialization.ISerializer), typeof(Serialization.BinaryStringSerializer));
            container.Register(typeof(Security.IEncryptionService), typeof(Security.EncryptionService));
            container.Register(typeof(IDateTimeHelper), typeof(DateTimeHelper));
            container.Register(typeof(Data.IDatabaseService), typeof(Data.DatabaseService));
            container.Register(typeof(Email.IEmailSender), typeof(Email.EmailSender));
            container.Register(typeof(Security.ISecurityManager), typeof(Security.DefaultSecurityManager));
            container.Register(typeof(Plugins.IPluginBootstrapper), typeof(Plugins.PluginBootstrapper));
            container.Register(typeof(Plugins.IPluginFinder), typeof(Plugins.PluginFinder));
            container.Register(typeof(Scheduling.IHeart), typeof(Scheduling.Heart));
            container.Register(typeof(IErrorNotifier), typeof(ErrorNotifier));
            container.Register(typeof(Data.IConnectionProvider), typeof(Data.NoConnectionProvider));
            container.Register(typeof(Caching.ICacheManager), typeof(Caching.InMemoryCache));
            container.Register(typeof(Scheduling.Scheduler));
            container.Register(typeof(Documentation.IDocumentationService), typeof(Documentation.DocumentationService));
#endif
        }
    }
}
