using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using Ninject;
using Noodle.Configuration;
using Noodle.Engine;

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

        #region Current

        /// <summary>
        /// Return the singleton kernel
        /// </summary>
        public static IKernel Current
        {
            get
            {
                Configure(false);
                return Singleton<IKernel>.Instance;
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
            if (Singleton<IKernel>.Instance == null || force)
            {
                lock (ContainerCreationLockObject)
                {
                    // someone may have waited for the lock, but it has been built for them, check one more time.
                    if (Singleton<IKernel>.Instance == null || force)
                    {
                        var kernel = new StandardKernel();

                        CoreDependencyRegistrar.Register(kernel);
                        var configuration = kernel.Get<ConfigurationManagerWrapper>();
                        var typeFinder = kernel.Get<ITypeFinder>();

                        // register everything!
                        RegisterAttributedServices(kernel);
                        RegisterDependencyRegistrar(typeFinder, kernel, configuration);

                        // set the kernel to the static accessor
                        Singleton<IKernel>.Instance = kernel;
;
                        // run all startup tasks
                        RunStartupTasks(kernel);
                    }
                }
            }
        }

        #endregion

        #region Shortcuts

        public static T Resolve<T>() where T : class
        {
            return Current.Get<T>();
        }

        public static IEnumerable<T> ResolveAll<T>() where T : class
        {
            return Current.GetAll<T>();
        }

        public static object ResolveUnregistered(Type type)
        {
            throw new NotImplementedException();
        }

        public static T ResolveUnregistered<T>()
        {
            return (T)ResolveUnregistered(typeof(T));
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

        #region Methods

        private static void RegisterDependencyRegistrar(ITypeFinder typeFinder, IKernel kernel, ConfigurationManagerWrapper configuration)
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
                //Debug.WriteLine("Ioc registering: " + dependencyRegistrarType.GetType().FullName);
                dependencyRegistrarType.Register(kernel, typeFinder, configuration);
            }
        }

        public static void RunStartupTasks(IKernel kernel)
        {
            var all = kernel.GetBindings(typeof(AutoStartBindingResolver.AutoStartBindingService));
            var startupServices = all.Select(x => kernel.Get(x.Service)).Cast<IStartupTask>().OrderBy(x => x.Order);
            foreach (var service in startupServices)
            {
                service.Execute();
            }
        }

        private static void RegisterAttributedServices(IKernel kernel)
        {
            var serviceRegistrar = kernel.Get<ServiceRegistrator>();
            serviceRegistrar.RegisterServices(kernel);
        }

        static EngineContext()
        {
            InitializeExcludedAndIncludedAssemblies();
        }

        #endregion
    }
}
