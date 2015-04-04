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
    public interface IIncludedAssemblies
    {
        List<AssemblyName> Assemblies { get; set; }
        IIncludedAssemblies Assembly(AssemblyName assembly);
        IIncludedAssemblies AssemblyRange(IEnumerable<AssemblyName> assemblies);
        IIncludedAssemblies AndAssembly(AssemblyName assembly);
        IIncludedAssemblies AndAssemblyRange(IEnumerable<AssemblyName> assemblies);
        IIncludedOnlyAssemblies IncludingOnly { get; }
        IExcludedAssemblies Excluding { get; }
    }
}
