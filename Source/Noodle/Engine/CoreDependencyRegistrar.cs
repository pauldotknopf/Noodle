#if SIMPLEINJECTOR
using SimpleInjector;
#endif

namespace Noodle.Engine
{
    public static class CoreDependencyRegistrar
    {
#if SIMPLEINJECTOR
        public static void Register(Container container)
#else
        public static void Register(TinyIoCContainer container)
#endif
        {
#if IOS
			container.Register(typeof(ITypeFinder), typeof(AppDomainTypeFinder));
			container.Register(typeof(IAssemblyFinder), typeof(AssemblyFinder));
#elif ANDROID
			container.Register(typeof(ITypeFinder), typeof(AppDomainTypeFinder));
			container.Register(typeof(IAssemblyFinder), typeof(AssemblyFinder));
#else

#if SIMPLEINJECTOR
            container.RegisterSingle<IWorker, AsyncWorker>();
            container.RegisterSingle(() => EngineContext.TypeFinder);
            container.RegisterSingle<IDateTimeHelper, DateTimeHelper>();
            container.RegisterSingle<IErrorNotifier, ErrorNotifier>();
#else
            container.Register(typeof(IWorker), typeof(AsyncWorker));
            container.Register(typeof(ITypeFinder), (c, overloads) => EngineContext.TypeFinder);
            container.Register(typeof(IDateTimeHelper), typeof(DateTimeHelper));
            container.Register(typeof(IErrorNotifier), typeof(ErrorNotifier));
#endif


#endif
        }
    }
}
