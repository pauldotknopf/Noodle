using System;
using System.Collections.Generic;
using System.Linq;
using Ninject.Planning.Bindings;
using Ninject.Planning.Bindings.Resolvers;

namespace Noodle.Engine
{
    // TODO:
    public class AutoStartBindingResolver : IBindingResolver
    {
        public IEnumerable<IBinding> Resolve(Ninject.Infrastructure.Multimap<Type, IBinding> bindings, Type service)
        {
            if (typeof(AutoStartBindingService).IsAssignableFrom(service))
            {
                var result = bindings.Where(x => typeof(IStartupTask).IsAssignableFrom(x.Key)).SelectMany(x => x.Value);
                return result;
            }
            return Enumerable.Empty<IBinding>();
        }

        public Ninject.INinjectSettings Settings { get; set; }

        public void Dispose()
        {
        }

        public class AutoStartBindingService{}
    }
}
