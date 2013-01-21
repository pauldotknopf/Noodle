using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Compilation;

namespace Noodle.Engine
{
    /// <summary>
    /// Responsible for getting and caching all the assemblies in the current app domain
    /// </summary>
    public class AssemblyFinder : IAssemblyFinder
    {
        private List<Assembly> _assemblies = null;
        private static readonly Object GetAssembliesLockObject = new object();

        /// <summary>
        /// Gets all the assemblies in the app domain
        /// </summary>
        /// <returns>List{System.Reflection.Assembly}.</returns>
        public List<Assembly> GetAssemblies()
        {
            if (_assemblies == null)
            {
                lock (GetAssembliesLockObject)
                {
                    if (_assemblies == null)
                    {
                        _assemblies = new List<Assembly>();
                        _assemblies.AddRange(EngineContext.IncludingOnly.Assemblies.Any()
                            ? EngineContext.IncludingOnly.Assemblies.Select(Assembly.Load)
                            : GetAssembliesInAppDomain()
                                .Where(a => !a.IsDynamic && IsNotExcluded(a)));
                    }
                }
            }

            return _assemblies;
        }

        private IEnumerable<Assembly> GetAssembliesInAppDomain()
        {
            try
            {
                return BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToList();
            }
            catch (InvalidOperationException ex)
            {
                // when not in a website, BuildManager.GetReferencedAssemblies throws an error.
                // if we are in a console/service/winforms, we need to manually load all the relevant assemblies.
                var assemblies = new List<Assembly>();
                RecursivelyLoadReferencedAssemblies(assemblies, CommonHelper.GetEntryAssembly());
                return assemblies;
            }
        } 

        private void RecursivelyLoadReferencedAssemblies(List<Assembly> assemblies, Assembly assembly)
        {
            if(assemblies.All(x => x.FullName != assembly.FullName))
                assemblies.Add(assembly);

            foreach (var referenced in assembly.GetReferencedAssemblies())
            {
                // addtional check for performance reasons
                if(!IsNotExcluded(referenced))
                    continue;

                RecursivelyLoadReferencedAssemblies(assemblies, Assembly.Load(referenced));
            }
        }

        /// <summary>
        /// Ensures that the assembly is not excluded
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        private bool IsNotExcluded(Assembly assembly)
        {
            return EngineContext.Including.Assemblies.Any(e => assembly.FullName == e.FullName) ||
                    !EngineContext.Excluding.Assemblies.Any(e => Regex.IsMatch(assembly.FullName, e));
        }

        /// <summary>
        /// Ensures that the assembly is not excluded
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        private bool IsNotExcluded(AssemblyName assembly)
        {
            return EngineContext.Including.Assemblies.Any(e => assembly.FullName == e.FullName) ||
                    !EngineContext.Excluding.Assemblies.Any(e => Regex.IsMatch(assembly.FullName, e));
        }
    }
}
