using System.Collections.Generic;
using System.Reflection;
using Noodle.Engine;

namespace Noodle.Management.Library
{
    public class PluginAssemblyFinder : IAssemblyFinder
    {
        /// <summary>
        /// Gets all the assemblies in the app domain
        /// </summary>
        /// <returns></returns>
        public List<Assembly> GetAssemblies()
        {
            return new List<Assembly>();
        }
    }
}
