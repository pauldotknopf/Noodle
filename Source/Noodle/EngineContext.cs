using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Noodle.Configuration;
using Noodle.Engine;
using SimpleInjector;

namespace Noodle
{
    /// <summary>
    /// This manages the kernel globally across an application.
    /// It runs startup tasks.
    /// It registers IDependencyRegistrars.
    /// </summary>
    public class EngineContext
    {
        private static readonly object ContainerCreationLockObject = new object();
        private static readonly FieldInfo RegistrationsFieldInfo = typeof(Container).GetField("registrations",
                                                                     BindingFlags.Public | BindingFlags.NonPublic |
                                                                     BindingFlags.Instance);

        #region Current

        /// <summary>
        /// Return the singleton kernel
        /// </summary>
        public static Container Current
        {
            get
            {
                Configure(false);
                return Singleton<Container>.Instance;
            }
        }

        /// <summary>
        /// Gets the type finder used throughout the system
        /// </summary>
        public static ITypeFinder TypeFinder
        {
            get 
            {
                return Singleton<ITypeFinder>.Instance 
                    ?? (Singleton<ITypeFinder>.Instance = new AppDomainTypeFinder(new AssemblyFinder()));
            }
        }

        #endregion

        #region Configure

        /// <summary>
        /// Ensure the container is configured. 
        /// Optionally force it to be re-configured.
        /// </summary>
        /// <param name="force"></param>
        public static void Configure(bool force)
        {
            // If the kernel hasn't been created or the call is forcing a new one do something, otherwise, just exit
            if (Singleton<Container>.Instance == null || force)
            {
                lock (ContainerCreationLockObject)
                {
                    // someone may have waited for the lock, but it has been built for them, check one more time.
                    if (Singleton<Container>.Instance == null || force)
                    {
                        var container = new Container();

                        CoreDependencyRegistrar.Register(container);

                        // register everything!
                        RegisterDependencyRegistrar(TypeFinder, container);

                        // set the kernel to the static accessor
                        Singleton<Container>.Instance = container;

                        // run all startup tasks
                        RunStartupTasks(container);
                    }
                }
            }
        }

        #endregion

        #region Shortcuts

        public static T Resolve<T>() where T : class
        {
            return Current.GetInstance<T>();
        }

        public static IEnumerable<T> ResolveAll<T>() where T : class
        {
            return Current.GetAllInstances<T>();
        }

        #endregion

        #region AssemblyExclude/Includes

        public static IExcludedAssemblies Excluding { get; private set; }
        public static IIncludedAssemblies Including { get; private set; }
        public static IIncludedOnlyAssemblies IncludingOnly { get; private set; }

        private static void InitializeExcludedAndIncludedAssemblies()
        {
            Excluding = new ExcludedAssemblies();
            Including = new IncludedAssemblies();
            IncludingOnly = new IncludedOnlyAssemblies();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the configuration group manager that will be used throughout the application.
        /// This is so that unit tests may setup other configuration sections for testing
        /// </summary>
        public static Func<ConfigurationGroupManager> ConfigurationGroupManager = () => new ConfigurationGroupManager("noodle");

        #endregion

        #region Methods

        private static void RegisterDependencyRegistrar(ITypeFinder typeFinder, Container container)
        {
            var dependencyRegistrarTypes = new List<IDependencyRegistrar>();
            foreach (var dependencyRegistrarType in typeFinder.Find<IDependencyRegistrar>())
            {
                IDependencyRegistrar instance;
                try
                {
                    instance = Activator.CreateInstance(dependencyRegistrarType) as IDependencyRegistrar;
                }
                catch (Exception)
                {
                    throw new Exception("Error creating '" + dependencyRegistrarType.FullName +
                                        "', please be sure that it contains a empty constructor.");
                }
                dependencyRegistrarTypes.Add(instance);
            }

            // Reorder the dependencies by order of importance
            dependencyRegistrarTypes = dependencyRegistrarTypes.OrderBy(x => x.Importance).ToList();

            // Register them in order, where higher importance overwrites lover
            foreach (var dependencyRegistrarType in dependencyRegistrarTypes)
            {
                dependencyRegistrarType.Register(container);
            }
        }

        public static void RunStartupTasks(Container container)
        {
            var registrations = RegistrationsFieldInfo.GetValue(container) as IDictionary;
            if (registrations == null) return;
            foreach (var instance in registrations.Keys.Cast<Type>().Where(x => typeof(IStartupTask).IsAssignableFrom(x))
                .Select(container.GetInstance).OfType<IStartupTask>().OrderBy(x => x.Order))
            {
                instance.Execute();
            }
        }

        static EngineContext()
        {
            InitializeExcludedAndIncludedAssemblies();
        }

        #endregion
    }
}
