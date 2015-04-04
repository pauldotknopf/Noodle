using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Noodle.Engine
{
    /// <summary>
    /// A class that finds types needed by looping assemblies in the
    /// currently executing AppDomain. Only assemblies whose names matches
    /// certain patterns are investigated.
    /// </summary>
    /// <remarks></remarks>
    [Serializable]
    public class AppDomainTypeFinder : ITypeFinder
    {
        #region Private Fields

        private readonly IAssemblyFinder _assemblyFinder;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="assemblyFinder"></param>
        public AppDomainTypeFinder(IAssemblyFinder assemblyFinder)
        {
            _assemblyFinder = assemblyFinder;
        }

        #endregion

        #region ITypeFinder

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
                        loaderErrors += ", " + loaderEx.Message;
                    }

                    throw new NoodleException("Error getting types from assembly " + a.FullName + loaderErrors, ex);
                }
            }

            return types;
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

        /// <summary>
        /// Finds types assignable from of a certain type in the app domain.
        /// </summary>
        /// <param name="requestedType">The type to find.</param>
        /// <param name="concreteTypesOnly">if set to <c>true</c> [concrete types only].</param>
        /// <returns>A list of types found in the app domain.</returns>
        public IList<Type> Find(Type requestedType, bool concreteTypesOnly = true)
        {
            return Find(requestedType, GetAssemblies(), concreteTypesOnly);
        }

        /// <summary>
        /// Finds types assignable from of a certain type in the app domain.
        /// </summary>
        /// <typeparam name="T">The type to find.</typeparam>
        /// <param name="concreteTypesOnly">if set to <c>true</c> [concrete types only].</param>
        /// <returns>A list of types found in the app domain.</returns>
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
            return _assemblyFinder.GetAssemblies();
        }

        #endregion

        #region Methods

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

        protected virtual bool IsAssignable(Type from, Type to)
        {
            if (to.IsAssignableFrom(from))
                return true;

            if (to.IsGenericTypeDefinition && DoesTypeImplementOpenGeneric(from, to))
                return true;

            return false;
        }

        #endregion
    }
}
