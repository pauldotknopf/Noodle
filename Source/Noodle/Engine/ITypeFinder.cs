using System;
using System.Collections.Generic;
using System.Reflection;

namespace Noodle.Engine
{
    /// <summary>
    /// Classes implementing this interface provide information about types
    /// to various services in the engine.
    /// </summary>
    /// <remarks></remarks>
    public interface ITypeFinder
    {
        /// <summary>
        /// Finds types assignable from of a certain type in the app domain.
        /// </summary>
        /// <param name="requestedType">The type to find.</param>
        /// <param name="assemeblies">A list of assemblies to look through</param>
        /// <param name="concreteTypesOnly">if set to <c>true</c> [concrete types only].</param>
        /// <returns>A list of types found in the app domain.</returns>
        /// <remarks></remarks>
        IList<Type> Find(Type requestedType, IList<Assembly> assemeblies, bool concreteTypesOnly = true);

        /// <summary>
        /// Finds types assignable from of a certain type in the app domain.
        /// </summary>
        /// <typeparam name="T">The type to find.</typeparam>
        /// <param name="assemeblies">A list of assemblies to look through</param>
        /// <param name="concreteTypesOnly">if set to <c>true</c> [concrete types only].</param>
        /// <returns>A list of types found in the app domain.</returns>
        /// <remarks></remarks>
        IList<Type> Find<T>(IList<Assembly> assemeblies, bool concreteTypesOnly = true);

        /// <summary>
        /// Finds types assignable from of a certain type in the app domain.
        /// </summary>
        /// <param name="requestedType">The type to find.</param>
        /// <param name="concreteTypesOnly">if set to <c>true</c> [concrete types only].</param>
        /// <returns>A list of types found in the app domain.</returns>
        /// <remarks></remarks>
        IList<Type> Find(Type requestedType, bool concreteTypesOnly = true);

        /// <summary>
        /// Finds types assignable from of a certain type in the app domain.
        /// </summary>
        /// <typeparam name="T">The type to find.</typeparam>
        /// <param name="concreteTypesOnly">if set to <c>true</c> [concrete types only].</param>
        /// <returns>A list of types found in the app domain.</returns>
        /// <remarks></remarks>
        IList<Type> Find<T>(bool concreteTypesOnly = true);

        /// <summary>
        /// Gets the assemblies related to the current implementation.
        /// </summary>
        /// <returns>A list of assemblies that should be loaded by the N2 factory.</returns>
        /// <remarks></remarks>
        IList<Assembly> GetAssemblies();

        ///// <summary>
        ///// Specify the assemblies to exclude
        ///// </summary>
        //IExcludedAssemblies Excluding { get; }

        ///// <summary>
        ///// Specify the assemblies to include
        ///// </summary>
        //IIncludedAssemblies Including { get; }

        ///// <summary>
        ///// Specifiy the ONLY assemblies to include
        ///// </summary>
        //IIncludedOnlyAssemblies IncludingOnly { get; }
    }

    /// <summary>
    /// A list of the only assemblies to include
    /// </summary>
    public interface IIncludedOnlyAssemblies
    {
        List<Assembly> Assemblies { get; set; }
        IIncludedOnlyAssemblies Assembly(Assembly assembly);
        IIncludedOnlyAssemblies AssemblyRange(IEnumerable<Assembly> assemblies);
        IIncludedOnlyAssemblies AndAssembly(Assembly assembly);
        IIncludedOnlyAssemblies AndAssemblyRange(IEnumerable<Assembly> assemblies);
        IIncludedAssemblies Including { get; }
        IExcludedAssemblies Excluding { get; }
    }

    /// <summary>
    /// A list of assemblies to include
    /// </summary>
    public interface IIncludedAssemblies
    {
        List<Assembly> Assemblies { get; set; }
        IIncludedAssemblies Assembly(Assembly assembly);
        IIncludedAssemblies AssemblyRange(IEnumerable<Assembly> assemblies);
        IIncludedAssemblies AndAssembly(Assembly assembly);
        IIncludedAssemblies AndAssemblyRange(IEnumerable<Assembly> assemblies);
        IIncludedOnlyAssemblies IncludingOnly { get; }
        IExcludedAssemblies Excluding { get; }
    }

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
