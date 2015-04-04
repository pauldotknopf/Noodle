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
    public interface IIncludedOnlyAssemblies
    {
        List<AssemblyName> Assemblies { get; set; }
        IIncludedOnlyAssemblies Assembly(AssemblyName assembly);
        IIncludedOnlyAssemblies AssemblyRange(IEnumerable<AssemblyName> assemblies);
        IIncludedOnlyAssemblies AndAssembly(AssemblyName assembly);
        IIncludedOnlyAssemblies AndAssemblyRange(IEnumerable<AssemblyName> assemblies);
        IIncludedAssemblies Including { get; }
        IExcludedAssemblies Excluding { get; }
    }
}
