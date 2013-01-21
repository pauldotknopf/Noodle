using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noodle.Engine
{
    /// <summary>
    /// A list of the assemblies to exclude
    /// </summary>
    public interface IExcludedAssemblies
    {
        List<string> Assemblies { get; set; }
        IExcludedAssemblies Assembly(string assembly);
        IExcludedAssemblies AndAssembly(string assembly);
        IIncludedAssemblies Including { get; }
        IIncludedOnlyAssemblies IncludingOnly { get; }
    }
}
