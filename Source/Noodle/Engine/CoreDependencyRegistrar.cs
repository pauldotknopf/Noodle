

namespace Noodle.Engine
{
    public static class CoreDependencyRegistrar
    {
        public static void Register(TinyIoCContainer container)
        {
#if IOS
			container.Register(typeof(ITypeFinder), typeof(AppDomainTypeFinder));
			container.Register(typeof(IAssemblyFinder), typeof(AssemblyFinder));
#else
            container.Register(typeof(IWorker), typeof(AsyncWorker));
            container.Register(typeof(ITypeFinder), typeof(AppDomainTypeFinder));
            container.Register(typeof(IAssemblyFinder), typeof(AssemblyFinder));
            container.Register(typeof(ISerializer), typeof(BinaryStringSerializer));
            container.Register(typeof(IEncryptionService), typeof(EncryptionService));
            container.Register(typeof(IDateTimeHelper), typeof(DateTimeHelper));
            container.Register(typeof(IDatabaseService), typeof(DatabaseService));
            container.Register(typeof(IEmailSender), typeof(EmailSender));
            container.Register(typeof(ISecurityManager), typeof(DefaultSecurityManager));
            container.Register(typeof(IPluginBootstrapper), typeof(PluginBootstrapper));
            container.Register(typeof(IPluginFinder), typeof(PluginFinder));
            container.Register(typeof(IHeart), typeof(Heart));
            container.Register(typeof(IErrorNotifier), typeof(ErrorNotifier));
            container.Register(typeof(IConnectionProvider), typeof(NoConnectionProvider));
            container.Register(typeof(ICacheManager), typeof(InMemoryCache));
            container.Register(typeof(Scheduler));
            container.Register(typeof(IDocumentationService), typeof(DocumentationService));
#endif
        }
    }
}
