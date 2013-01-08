using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Noodle.Engine;

namespace Noodle.Tests
{
    public class FakeTypeFinder : ITypeFinder
    {
        private readonly Type[] _types;

        public FakeTypeFinder(params Type[] types)
        {
            _types = types;
        }

        public IList<Type> Find(Type requestedType, IList<Assembly> assemeblies, bool concreteTypesOnly = true)
        {
            return new AppDomainTypeFinder().Find(requestedType, assemeblies, concreteTypesOnly);
        }

        public IList<Type> Find<T>(IList<Assembly> assemeblies, bool concreteTypesOnly = true)
        {
            return Find(typeof (T), assemeblies, concreteTypesOnly);
        }

        public IList<Type> Find(Type requestedType, bool concreteTypesOnly = true)
        {
            return Find(requestedType, GetAssemblies(), concreteTypesOnly);
        }

        public IList<Type> Find<T>(bool concreteTypesOnly = true)
        {
            return Find(typeof (T), concreteTypesOnly);
        }

        public IList<Assembly> GetAssemblies()
        {
            var assemblies = new List<Assembly>();

            foreach (var type in _types.Where(type => assemblies.All(x => x.FullName != type.Assembly.FullName)))
            {
                assemblies.Add(type.Assembly);
            }

            return assemblies;
        }
    }
}
