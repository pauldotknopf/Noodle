using System;
using System.Collections.Generic;
using System.Linq;
using Ninject.Activation;
using Ninject.Infrastructure;
using Ninject.Planning.Bindings;
using Ninject.Planning.Bindings.Resolvers;

namespace Noodle.Settings
{
    public class SettingsBindingResolver : IMissingBindingResolver
    {
        public IEnumerable<IBinding> Resolve(Multimap<Type, IBinding> bindings, IRequest request)
        {
            if (typeof(ISettings).IsAssignableFrom(request.Service))
            {
                var binding = new Binding(request.Service)
                {
                    ProviderCallback = ctx => new SettingsProvider(request.Service),
                    Target = BindingTarget.Method
                };
                return new List<IBinding> { binding };
            }
            return Enumerable.Empty<IBinding>();
        }

        public Ninject.INinjectSettings Settings { get; set; }

        public void Dispose()
        {
        }
    }
}
