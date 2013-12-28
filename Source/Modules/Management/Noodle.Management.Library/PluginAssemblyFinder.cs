using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AppDomainToolkit;
using Noodle.Engine;

namespace Noodle.Management.Library
{
    public class PluginAssemblyFinder : IAssemblyFinder
    {
        private object _lock = new object();
        private List<Assembly> _assemblies;
 
        /// <summary>
        /// Gets all the assemblies in the app domain
        /// </summary>
        /// <returns></returns>
        public List<Assembly> GetAssemblies()
        {
            if (_assemblies != null)
                return _assemblies;

            lock (_lock)
            {
                if (_assemblies != null)
                    return _assemblies;

                _assemblies = new List<Assembly>();

                var loader = new AssemblyLoader();
                var alreadyLoadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToDictionary(x => x.CodeBase.Replace(@"file:///", "").Replace("/", @"\").ToLowerInvariant(), x => x);
                foreach (var assemblyPath in GetFilesMatchingPattern("*.dll").Select(x => x.Replace("/", @"\").ToLowerInvariant()))
                {
                    if (alreadyLoadedAssemblies.ContainsKey(assemblyPath))
                    {
                        _assemblies.Add(alreadyLoadedAssemblies[assemblyPath]);
                        continue;
                    }

                    var pdbPath = Path.ChangeExtension(assemblyPath, "pdb");
                    _assemblies.Add(loader.LoadAssembly(LoadMethod.LoadFile, assemblyPath, File.Exists(pdbPath) ? pdbPath : null));
                }
                return _assemblies;
            }
            
        }

        private IEnumerable<string> GetFilesMatchingPattern(string pattern)
        {
            return NormalizePaths(Path.GetDirectoryName(pattern))
                    .SelectMany(path => Directory.GetFiles(path, Path.GetFileName(pattern)));
        }

        private IEnumerable<string> NormalizePaths(string path)
        {
            return Path.IsPathRooted(path)
                        ? new[] { Path.GetFullPath(path) }
                        : GetBaseDirectories().Select(baseDirectory => Path.Combine(baseDirectory, path));
        }

        private IEnumerable<string> GetBaseDirectories()
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var searchPath = AppDomain.CurrentDomain.RelativeSearchPath;

            return String.IsNullOrEmpty(searchPath)
                ? new[] { baseDirectory }
                : searchPath.Split(new[] { Path.PathSeparator }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(path => Path.Combine(baseDirectory, path));
        }
    }
}
