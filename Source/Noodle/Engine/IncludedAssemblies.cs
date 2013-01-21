using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Noodle.Engine
{
    /// <summary>
    /// A list of assemblies to include
    /// </summary>
    public class IncludedAssemblies : IIncludedAssemblies
    {
        public List<AssemblyName> Assemblies { get; set; }

        public IncludedAssemblies()
        {
            Assemblies = new List<AssemblyName>();
        }

        public IIncludedAssemblies Assembly(AssemblyName assembly)
        {
            Assemblies.Add(assembly);
            return this;
        }

        public IIncludedAssemblies AssemblyRange(IEnumerable<AssemblyName> assemblies)
        {
            Assemblies.AddRange(assemblies);
            return this;
        }

        public IIncludedAssemblies AndAssembly(AssemblyName assembly)
        {
            return Assembly(assembly);
        }

        public IIncludedAssemblies AndAssemblyRange(IEnumerable<AssemblyName> assemblies)
        {
            return AssemblyRange(assemblies);
        }

        public IIncludedOnlyAssemblies IncludingOnly
        {
            get { return EngineContext.IncludingOnly; }
        }

        public IExcludedAssemblies Excluding
        {
            get { return EngineContext.Excluding; }
        }
    }
}
