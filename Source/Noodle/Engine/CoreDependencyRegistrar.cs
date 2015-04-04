

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
            container.Register(typeof(ITypeFinder), (c, overloads) => EngineContext.TypeFinder);
            container.Register(typeof(IDateTimeHelper), typeof(DateTimeHelper));
            container.Register(typeof(IErrorNotifier), typeof(ErrorNotifier));
#endif
        }
    }
}
