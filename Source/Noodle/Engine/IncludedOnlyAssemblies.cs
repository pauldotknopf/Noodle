using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Noodle.Engine
{
    /// <summary>
    /// A list of the only assemblies to include
    /// </summary>
    public class IncludedOnlyAssemblies : IIncludedOnlyAssemblies
    {
        public List<AssemblyName> Assemblies { get; set; }

        public IncludedOnlyAssemblies()
        {
            Assemblies = new List<AssemblyName>();
        }

        public IIncludedOnlyAssemblies Assembly(AssemblyName assembly)
        {
            if (Assemblies.Count == 0) Assemblies.Add(GetNoodleAssembly());
            if (!Assemblies.Contains(assembly)) Assemblies.Add(assembly);
            return this;
        }

        public IIncludedOnlyAssemblies AssemblyRange(IEnumerable<AssemblyName> assemblies)
        {
            Assemblies.AddRange(assemblies);
            return this;
        }

        public IIncludedOnlyAssemblies AndAssembly(AssemblyName assembly)
        {
            return Assembly(assembly);
        }

        public IIncludedOnlyAssemblies AndAssemblyRange(IEnumerable<AssemblyName> assemblies)
        {
            return AssemblyRange(assemblies);
        }

        public IIncludedAssemblies Including
        {
            get { return EngineContext.Including; }
        }

        public IExcludedAssemblies Excluding
        {
            get { return EngineContext.Excluding; }
        }

        private static AssemblyName GetNoodleAssembly()
        {
            return typeof (EngineContext).Assembly.GetName();
        }
    }
}
