using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Noodle.Engine
{
    /// <summary>
    /// Responsible for getting and caching all the assemblies in the current app domain
    /// </summary>
    public interface IAssemblyFinder
    {
        /// <summary>
        /// Gets all the assemblies in the app domain
        /// </summary>
        /// <returns></returns>
        List<Assembly> GetAssemblies();
    }
}
