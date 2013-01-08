using System;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Noodle
{
    // TODO
    //public static class ContainerExtensions
    //{
    //    [DebuggerStepThrough]
    //    public static IBindingWhenInNamedWithOrOnSyntax<T> InRequestScope<T>(this IBindingWhenInNamedWithOrOnSyntax<T> binding)
    //    {
    //        binding.InScope(x => HttpContext.Current);
    //        return binding;
    //    }

    //    [DebuggerStepThrough]
    //    public static T Resolve<T>(this IKernel kernel) where T : class
    //    {
    //        return kernel.GetService(typeof(T)) as T;
    //    }

    //    [DebuggerStepThrough]
    //    public static T ResolveUnregistered<T>(this IKernel kernalBase) where T:class
    //    {
    //        return ResolveUnregistered(kernalBase, typeof(T)) as T;
    //    }

    //    [DebuggerStepThrough]
    //    public static object ResolveUnregistered(this IKernel kernelBase, Type type)
    //    {
    //        // Create the binding
    //        IBinding binding = new Binding(type);
    //        binding.ProviderCallback = StandardProvider.GetCreationCallback(type);
    //        binding.Target = BindingTarget.Self;

    //        // Create the request
    //        var request = kernelBase.CreateRequest(type, null, Enumerable.Empty<IParameter>(), false, true);
            
    //        // Create the context and resolve the request.
    //        return kernelBase.CreateContext(request, binding).Resolve();
    //    }

    //    [DebuggerStepThrough]
    //    private static IContext CreateContext(this IKernel kernelBase, IRequest request, IBinding binding)
    //    {
    //        return new Context(kernelBase, request, binding, kernelBase.Components.Get<ICache>(), kernelBase.Components.Get<IPlanner>(), kernelBase.Components.Get<IPipeline>());
    //    }
    //}
}
