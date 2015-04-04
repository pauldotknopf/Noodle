using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Noodle.Engine
{
    /// <summary>
    /// Responsible for getting and caching all the assemblies provided.
    /// </summary>
    public class PreloadedAssemblyFinder : IAssemblyFinder
    {
        private readonly List<Assembly> _assemblies;

        /// <summary>
        /// Initializes a new instance of the <see cref="PreloadedAssemblyFinder"/> class.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        public PreloadedAssemblyFinder(params Assembly[] assemblies)
        {
            _assemblies = assemblies != null ? assemblies.ToList() : new List<Assembly>();
        }

        /// <summary>
        /// Gets all the assemblies
        /// </summary>
        /// <returns></returns>
        public List<Assembly> GetAssemblies()
        {
            return _assemblies;
        }

        /// <summary>
        /// Helper to build a preloaded assembly finder
        /// </summary>
        public static IAssemblyFinder From(params Assembly[] assemblies)
        {
            return new PreloadedAssemblyFinder(assemblies);
        }
    }
}
