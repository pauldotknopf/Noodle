using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Noodle.Engine;

namespace Noodle.Tests.Helpers
{
    public class FakeAssemblyFinder : IAssemblyFinder
    {
        private readonly List<Assembly> _assemblies;

        public FakeAssemblyFinder(List<Assembly> assemblies)
        {
            _assemblies = assemblies;
        }

        public List<Assembly> GetAssemblies()
        {
            return _assemblies;
        }
    }
}
