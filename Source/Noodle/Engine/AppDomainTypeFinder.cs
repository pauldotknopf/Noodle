using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Compilation;

namespace Noodle.Engine
{
    /// <summary>
    /// A class that finds types needed by looping assemblies in the
    /// currently executing AppDomain. Only assemblies whose names matches
    /// certain patterns are investigated and an optional list of assemblies
    /// referenced by <see cref="AssemblyNames"/> are always investigated.
    /// </summary>
    /// <remarks></remarks>
    [Serializable]
    public class AppDomainTypeFinder : ITypeFinder
    {
        #region Private Fields

        private string _assemblySkipLoadingPattern = "^System|^mscorlib|^Microsoft|^CppCodeProvider|^VJSharpCodeProvider|^WebDev|^Castle|^Iesi|^log4net|^NHibernate|^nunit|^TestDriven|^MbUnit|^Rhino|^QuickGraph|^TestFu|^Telerik|^ComponentArt|^MvcContrib|^AjaxControlToolkit|^Antlr3|^Remotion|^Recaptcha|^Lucene|^Ionic|^HibernatingRhinos|^Spark|^SharpArch|^CommonServiceLocator|^Newtonsoft|^SMDiagnostics|^App_LocalResources|^AntiXSSLibrary|^dotless|^HtmlSanitizationLibrary|^sqlce|^WindowsBase|^Pandora|^PegBase|^DynamicProxyGenAssembly|^Anonymously Hosted DynamicMethods Assembly|^WebActivator|^Deleporter|^Elmah|^Markdown|^SimpleHttpClient|^StructureMap|^WebDriver";
        private string _assemblyRestrictToLoadingPattern = ".*";
        private IList<string> _assemblyNames = new List<string>();
        private IList<Assembly> _assemblies = null;

        #endregion

        #region Properties

        /// <summary>
        /// The app domain to look for types in.
        /// </summary>
        /// <remarks></remarks>
        public virtual AppDomain App
        {
            get { return AppDomain.CurrentDomain; }
        }

        /// <summary>
        /// Gets or sets assemblies loaded a startup in addition to those loaded in the AppDomain.
        /// </summary>
        /// <value>The assembly names.</value>
        /// <remarks></remarks>
        public IList<string> AssemblyNames
        {
            get { return _assemblyNames; }
            set { _assemblyNames = value; }
        }

        /// <summary>
        /// Gets the pattern for dlls that we know don't need to know about.
        /// </summary>
        /// <value>The assembly skip loading pattern.</value>
        /// <remarks></remarks>
        public string AssemblySkipLoadingPattern
        {
            get { return _assemblySkipLoadingPattern; }
            set { _assemblySkipLoadingPattern = value; }
        }

        /// <summary>
        /// Gets or sets the pattern for dll that will be investigated. For ease of use this defaults to match all but to increase performance you might want to configure a pattern that includes only the core assemblies and your own.
        /// </summary>
        /// <value>The assembly restrict to loading pattern.</value>
        /// <remarks>If you change this so that core assemblies aren't investigated (e.g. by not including something like "^Noodle|..." you may break core functionality.</remarks>
        public string AssemblyRestrictToLoadingPattern
        {
            get { return _assemblyRestrictToLoadingPattern; }
            set { _assemblyRestrictToLoadingPattern = value; }
        }

        #endregion

        /// <summary>
        /// Finds types assignable from of a certain type in the app domain.
        /// </summary>
        /// <param name="requestedType">The type to find.</param>
        /// <param name="assemeblies">A list of assemblies to look through</param>
        /// <param name="concreteTypesOnly">if set to <c>true</c> [concrete types only].</param>
        /// <returns>A list of types found in the app domain.</returns>
        /// <remarks></remarks>
        public virtual IList<Type> Find(Type requestedType, IList<Assembly> assemeblies, bool concreteTypesOnly = true)
        {
            var types = new List<Type>();
            foreach (Assembly a in assemeblies)
            {
                try
                {
                    foreach (var type in a.GetTypes().Where(x => IsAssignable(x, requestedType)))
                    {
                        if(!type.IsInterface)
                        {
                            if (concreteTypesOnly)
                            {
                                if (type.IsClass && !type.IsAbstract)
                                {
                                    types.Add(type);
                                }
                            }
                            else
                            {
                                types.Add(type);
                            }
                        }
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    string loaderErrors = string.Empty;
                    foreach (Exception loaderEx in ex.LoaderExceptions)
                    {
                        Trace.TraceError(loaderEx.ToString());
                        loaderErrors += ", " + loaderEx.Message;
                    }

                    throw new NoodleException("Error getting types from assembly " + a.FullName + loaderErrors, ex);
                }
            }

            return types;
        }

        protected virtual bool IsAssignable(Type from, Type to)
        {
            if(to.IsAssignableFrom(from))
                return true;

            if (to.IsGenericTypeDefinition && DoesTypeImplementOpenGeneric(from, to))
                return true;

            return false;
        }

        /// <summary>
        /// Finds types assignable from of a certain type in the app domain.
        /// </summary>
        /// <typeparam name="T">The type to find.</typeparam>
        /// <param name="assemeblies">A list of assemblies to look through</param>
        /// <param name="concreteTypesOnly">if set to <c>true</c> [concrete types only].</param>
        /// <returns>A list of types found in the app domain.</returns>
        /// <remarks></remarks>
        public IList<Type> Find<T>(IList<Assembly> assemeblies, bool concreteTypesOnly = true)
        {
            return Find(typeof (T),assemeblies, concreteTypesOnly);
        }

        public IList<Type> Find(Type requestedType, bool concreteTypesOnly = true)
        {
            return Find(requestedType, GetAssemblies(), concreteTypesOnly);
        }

        public IList<Type> Find<T>(bool concreteTypesOnly = true)
        {
            return Find<T>(GetAssemblies(), concreteTypesOnly);
        }

        /// <summary>
        /// Gets tne assemblies related to the current implementation.
        /// </summary>
        /// <returns>A list of assemblies that should be loaded.</returns>
        /// <remarks></remarks>
        public virtual IList<Assembly> GetAssemblies()
        {
            if (_assemblies == null || !_assemblies.Any())
            {
                var addedAssemblyNames = new List<string>();
                var assemblies = new List<Assembly>();

                AddAssembliesInAppDomain(addedAssemblyNames, assemblies);
                AddConfiguredAssemblies(addedAssemblyNames, assemblies);

                _assemblies = assemblies;
            }
            return _assemblies;
        }

        /// <summary>
        /// Iterates all assemblies in the AppDomain and if it's name matches the configured patterns add it to our list.
        /// </summary>
        /// <param name="addedAssemblyNames">The added assembly names.</param>
        /// <param name="assemblies">The assemblies.</param>
        /// <remarks></remarks>
        private void AddAssembliesInAppDomain(List<string> addedAssemblyNames, List<Assembly> assemblies)
        {
            try
            {
                foreach (Assembly assembly in BuildManager.GetReferencedAssemblies())
                {
                    if (Matches(assembly.FullName))
                    {
                        if (!addedAssemblyNames.Contains(assembly.FullName))
                        {
                            assemblies.Add(assembly);
                            addedAssemblyNames.Add(assembly.FullName);
                        }
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                // when not in a website, BuildManager.GetReferencedAssemblies throws an error.
                // if we are in a console/service/winforms, we need to manually load all the relevant assemblies.
                var assemblyCheckerType = typeof(AssemblyChecker);
                var temporaryDomain = CreateTemporaryAppDomain();
                List<string> assemblyFullNames;

                try
                {
                    var checker = (AssemblyChecker)temporaryDomain.CreateInstanceAndUnwrap(
                        assemblyCheckerType.Assembly.FullName,
                        assemblyCheckerType.FullName ?? string.Empty);

                    assemblyFullNames = checker.GetAssemblyNames(GetAllLocalAssemblies("*"), (assembly) => Matches(assembly.FullName));
                }
                finally
                {
                    AppDomain.Unload(temporaryDomain);
                }

                // now that we have a list of full assembly names that may not be loaded in the app domain but pass all the filter checks,
                // lets do a dynamically create an instance of Assembly which will load it into the app domain.
                foreach(var assembly in assemblyFullNames)
                {
                    // simply load it into the app domain.
                    // later we will finally inspect the app domain for assemblies to look for.
                    // this is simply a trick to force all assemblies to be loaded for type inspection.
                    Assembly.Load(new AssemblyName(assembly));
                }
            }
            
            foreach (Assembly assembly in App.GetAssemblies())
            {
                if (Matches(assembly.FullName))
                {
                    if (!addedAssemblyNames.Contains(assembly.FullName))
                    {
                        assemblies.Add(assembly);
                        addedAssemblyNames.Add(assembly.FullName);
                    }
                }
            }
        }

        /// <summary>
        /// Adds specifically configured assemblies.
        /// </summary>
        /// <param name="addedAssemblyNames">The added assembly names.</param>
        /// <param name="assemblies">The assemblies.</param>
        /// <remarks></remarks>
        protected virtual void AddConfiguredAssemblies(List<string> addedAssemblyNames, List<Assembly> assemblies)
        {
            foreach (string assemblyName in AssemblyNames)
            {
                var assembly = Assembly.Load(assemblyName);
                if (!addedAssemblyNames.Contains(assembly.FullName))
                {
                    assemblies.Add(assembly);
                    addedAssemblyNames.Add(assembly.FullName);
                }
            }
        }

        /// <summary>
        /// Check if a dll is one of the shipped dlls that we know don't need to be investigated.
        /// </summary>
        /// <param name="assemblyFullName">The name of the assembly to check.</param>
        /// <returns>True if the assembly should be loaded.</returns>
        /// <remarks></remarks>
        public virtual bool Matches(string assemblyFullName)
        {
            return !Matches(assemblyFullName, AssemblySkipLoadingPattern)
            && Matches(assemblyFullName, AssemblyRestrictToLoadingPattern);
        }

        /// <summary>
        /// Check if a dll is one of the shipped dlls that we know don't need to be investigated.
        /// </summary>
        /// <param name="assemblyFullName">The assembly name to match.</param>
        /// <param name="pattern">The regular expression pattern to match against the assembly name.</param>
        /// <returns>True if the pattern matches the assembly name.</returns>
        /// <remarks></remarks>
        protected virtual bool Matches(string assemblyFullName, string pattern)
        {
            return Regex.IsMatch(assemblyFullName, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        /// <summary>
        /// Determines if a type is a representation of an open generic
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="openGeneric">The open generic.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        protected virtual bool DoesTypeImplementOpenGeneric(Type type, Type openGeneric)
        {
            try
            {
                var genericTypeDefinition = openGeneric.GetGenericTypeDefinition();

                if (genericTypeDefinition.IsAssignableFrom(type))
                    return true;

                var baseType = type.BaseType;
                while (baseType != null)
                {
                    if (baseType.IsGenericType)
                    {
                        var isMatch = genericTypeDefinition.IsAssignableFrom(baseType.GetGenericTypeDefinition());
                        if (isMatch)
                            return true;
                    }
                    baseType = baseType.BaseType;
                }

                foreach (var implementedInterface in type.FindInterfaces((objType, objCriteria) => true, null))
                {
                    if (!implementedInterface.IsGenericType)
                        continue;

                    var isMatch = genericTypeDefinition.IsAssignableFrom(implementedInterface.GetGenericTypeDefinition());
                    return isMatch;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Creates a temporary app domain.
        /// </summary>
        /// <returns>The created app domain.</returns>
        private static AppDomain CreateTemporaryAppDomain()
        {
            return AppDomain.CreateDomain(
                "NoodleAssemblyLoader",
                AppDomain.CurrentDomain.Evidence,
                AppDomain.CurrentDomain.SetupInformation);
        }

        /// <summary>
        /// returns a list of file paths to every assembly/dll that is usable by the current app domain
        /// </summary>
        /// <returns></returns>
        private List<string> GetAllLocalAssemblies(string pattern)
        {
            return GetFilesMatchingPattern(pattern).ToList();
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

    /// <summary>
    /// This class is loaded into the temporary appdomain to load and check if the assemblies match the filter.
    /// </summary>
    internal class AssemblyChecker : MarshalByRefObject
    {
        /// <summary>
        /// Gets the assembly names of the assemblies matching the filter.
        /// </summary>
        /// <param name="filenames">The filenames.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>All assembly names of the assemblies matching the filter.</returns>
        public List<string> GetAssemblyNames(IEnumerable<string> filenames, Predicate<Assembly> filter)
        {
            var result = new List<string>();
            foreach (var filename in filenames)
            {
                Assembly assembly;
                if (File.Exists(filename))
                {
                    try
                    {
                        assembly = Assembly.LoadFrom(filename);
                    }
                    catch (BadImageFormatException)
                    {
                        continue;
                    }
                }
                else
                {
                    try
                    {
                        assembly = Assembly.Load(filename);
                    }
                    catch (FileNotFoundException)
                    {
                        continue;
                    }
                }

                if (filter(assembly))
                {
                    result.Add(assembly.FullName);
                }
            }

            return result;
        }
    }
}
