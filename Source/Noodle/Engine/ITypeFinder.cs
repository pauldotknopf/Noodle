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
        /// Gets tne assemblies related to the current implementation.
        /// </summary>
        /// <returns>A list of assemblies that should be loaded by the N2 factory.</returns>
        /// <remarks></remarks>
        IList<Assembly> GetAssemblies();
    }

    ///// <summary>
    ///// Classes implementing this interface provide information about types 
    ///// to various services in the engine.
    ///// </summary>
    //public interface ITypeFinder
    //{
    //    IList<Assembly> GetAssemblies();

    //    IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, bool onlyConcreteClasses = true);

    //    IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true);

    //    IEnumerable<Type> FindClassesOfType<T>(bool onlyConcreteClasses = true);

    //    IEnumerable<Type> FindClassesOfType<T>(IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true);

    //    IEnumerable<Type> FindClassesOfType<T, TAssemblyAttribute>(bool onlyConcreteClasses = true) where TAssemblyAttribute : Attribute;

    //    IEnumerable<Assembly> FindAssembliesWithAttribute<T>();

    //    IEnumerable<Assembly> FindAssembliesWithAttribute<T>(IEnumerable<Assembly> assemblies);

    //    IEnumerable<Assembly> FindAssembliesWithAttribute<T>(DirectoryInfo assemblyPath);
    //}
}
