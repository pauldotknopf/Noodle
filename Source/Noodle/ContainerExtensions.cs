using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;
using Ninject;
using Ninject.Activation;
using Ninject.Activation.Caching;
using Ninject.Activation.Providers;
using Ninject.Parameters;
using Ninject.Planning;
using Ninject.Planning.Bindings;
using Ninject.Syntax;
using Noodle.Engine;

namespace Noodle
{
    public static class ContainerExtensions
    {
        [DebuggerStepThrough]
        public static IBindingWhenInNamedWithOrOnSyntax<T> InRequestScope<T>(this IBindingWhenInNamedWithOrOnSyntax<T> binding)
        {
            binding.InScope(x => HttpContext.Current);
            return binding;
        }

        [DebuggerStepThrough]
        public static T Resolve<T>(this IKernel kernel) where T : class
        {
            return kernel.GetService(typeof(T)) as T;
        }

        [DebuggerStepThrough]
        public static T ResolveUnregistered<T>(this IKernel kernalBase) where T:class
        {
            return ResolveUnregistered(kernalBase, typeof(T)) as T;
        }

        [DebuggerStepThrough]
        public static object ResolveUnregistered(this IKernel kernelBase, Type type)
        {
            // Create the binding
            IBinding binding = new Binding(type);
            binding.ProviderCallback = StandardProvider.GetCreationCallback(type);
            binding.Target = BindingTarget.Self;

            // Create the request
            var request = kernelBase.CreateRequest(type, null, Enumerable.Empty<IParameter>(), false, true);
            
            // Create the context and resolve the request.
            return kernelBase.CreateContext(request, binding).Resolve();
        }

        public static void RegisterLazy(this IKernel kernel)
        {
            kernel.Rebind(typeof(Lazy<>)).ToMethod(ctx => typeof (ContainerExtensions)
                                                               .GetMethod("GetLazyProvider",
                                                                          BindingFlags.Instance |
                                                                          BindingFlags.NonPublic | BindingFlags.Static)
                                                               .MakeGenericMethod(ctx.GenericArguments[0])
                                                               .Invoke(null, new object[] {ctx.Kernel}));
        }

        private static Lazy<T> GetLazyProvider<T>(IKernel kernel)
        {
            return new Lazy<T>(() => kernel.Get<T>());
        }

        [DebuggerStepThrough]
        private static IContext CreateContext(this IKernel kernelBase, IRequest request, IBinding binding)
        {
            return new Context(kernelBase, request, binding, kernelBase.Components.Get<ICache>(), kernelBase.Components.Get<IPlanner>(), kernelBase.Components.Get<IPipeline>());
        }
    }
}
