//#define LOGGING
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Noodle.Engine;
#if SIMPLEINJECTOR
using Cont = SimpleInjector.Container;
#else
using Cont = Noodle.TinyIoCContainer;
#endif

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
#if LOGGING
        private static Logger<EngineContext> _logger = new Logger<EngineContext>(); 
#endif
#if SIMPLEINJECTOR
        private static readonly FieldInfo RegistrationsFieldInfo = typeof(Cont).GetField("registrations",
                                                                     BindingFlags.Public | BindingFlags.NonPublic |
                                                                     BindingFlags.Instance);
#endif

        #region Current

        /// <summary>
        /// Return the singleton kernel
        /// </summary>
        public static Cont Current
        {
            get
            {
                Configure(false);
                return Singleton<Cont>.Instance;
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
                    ?? (Singleton<ITypeFinder>.Instance = new AppDomainTypeFinder(AssemblyFinder));
            }
            set
            {
                if(value == null)
                    throw new ArgumentNullException("value");
                Singleton<ITypeFinder>.Instance = value;
            }
        }

        public static IAssemblyFinder AssemblyFinder
        {
            get
            {
                return Singleton<IAssemblyFinder>.Instance
                       ?? (Singleton<IAssemblyFinder>.Instance = new AssemblyFinder());
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                if(Singleton<IAssemblyFinder>.Instance != null)
                    throw new Exception("Assembly finder already set");
                Singleton<IAssemblyFinder>.Instance = value;
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
            if (Singleton<Cont>.Instance == null || force)
            {
                lock (ContainerCreationLockObject)
                {
                    // someone may have waited for the lock, but it has been built for them, check one more time.
                    if (Singleton<Cont>.Instance == null || force)
                    {
                        #if LOGGING
                        _logger.Info("Creating the container");
                        #endif

                        var container = new Cont();

                        #if LOGGING
                        _logger.Info("Registering the core services");
                        #endif

                        CoreDependencyRegistrar.Register(container);

                        #if LOGGING
                        _logger.Info("Registering all the dependency registrars");
                        #endif

                        // register everything!
                        RegisterDependencyRegistrar(TypeFinder, container);

                        #if LOGGING
                        _logger.Info("Setting the static accessor");
                        #endif

                        // set the kernel to the static accessor
                        Singleton<Cont>.Instance = container;

                        // run all startup tasks
                        var onStartupTasksRunning = OnStartupTasksRunning;
                        if (onStartupTasksRunning != null)
                            onStartupTasksRunning(container);
                        RunStartupTasks(container);
                        var onStartupTasksRan = OnStartupTasksRan;
                        if (onStartupTasksRan != null)
                            onStartupTasksRan(container);
                    }
                }
            }
        }

        #endregion

        #region Shortcuts

        public static T Resolve<T>() where T : class
        {
#if SIMPLEINJECTOR
            return Current.GetInstance<T>();
#else
             return Current.Resolve<T>();
#endif

        }

        public static IEnumerable<T> ResolveAll<T>() where T : class
        {
#if SIMPLEINJECTOR
            return Current.GetAllInstances<T>();
#else
            return Current.ResolveAll<T>();
#endif

        }

        #endregion

        #region Methods

        private static void RegisterDependencyRegistrar(ITypeFinder typeFinder, Cont container)
        {
            var dependencyRegistrarTypes = new List<IDependencyRegistrar>();
            foreach (var dependencyRegistrarType in typeFinder.Find<IDependencyRegistrar>())
            {
                IDependencyRegistrar instance;
                try
                {
                    #if LOGGING
                    _logger.Info("Creating the dependency registrar {0}".F(dependencyRegistrarType.FullName));
                    #endif
                    instance = Activator.CreateInstance(dependencyRegistrarType) as IDependencyRegistrar;
                }
                catch (Exception ex)
                {
                    #if LOGGING
                    _logger.Error("Error creating dependency registrar {0} ({1})".F(dependencyRegistrarType.FullName, ex.Message));
                    #endif
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
                #if LOGGING
                _logger.Info("Registering dependency registrar {0}".F(dependencyRegistrarType.GetType().FullName));
                #endif
                dependencyRegistrarType.Register(container);
            }
        }

        public static void RunStartupTasks(Cont container)
        {
            #if LOGGING
            _logger.Info("Running startup tasks");
            #endif

#if SIMPLEINJECTOR
            var registrations = RegistrationsFieldInfo.GetValue(container) as IDictionary;
            if(registrations == null) throw new Exception("Couldn't get registrations from simpleinjector");
            var startupServiceTypes =
                registrations.Keys.Cast<Type>().Where(x => typeof (IStartupTask).IsAssignableFrom(x)).ToList(); 
#else
            var startupServiceTypes = container.GetServicesOf<IStartupTask>();
#endif
            var startupServices = new List<IStartupTask>();

            foreach(var startUpServiceType in startupServiceTypes)
            {
                #if LOGGING
                _logger.Info("Creating an instance of the startup task {0}".F(startUpServiceType.FullName));
                #endif
                #if SIMPLEINJECTOR
                startupServices.Add(container.GetInstance(startUpServiceType) as IStartupTask);
                #else
                startupServices.Add(container.Resolve(startUpServiceType) as IStartupTask);
                #endif

                #if LOGGING
                _logger.Info("Created an instance of the startup task {0}".F(startUpServiceType.FullName));
                #endif
            }

            startupServices = startupServices.OrderBy(x => x.Order).ToList();

            foreach (var startupService in startupServices)
            {
                #if LOGGING
                _logger.Info("Executing an instance of the startup task {0}".F(startupService.GetType().FullName));
                #endif
                startupService.Execute();
                #if LOGGING
                _logger.Info("Executing an instance of the startup task {0}".F(startupService.GetType().FullName));
                #endif
            }

            #if LOGGING
            _logger.Info("Ran startup tasks");
            #endif
        }

        #endregion

        #region Events

        public delegate void ContainerDelegate(Cont container);

        /// <summary>
        /// This is raised before any startup tasks have been ran
        /// </summary>
        public static event ContainerDelegate OnStartupTasksRunning;

        /// <summary>
        /// This is raised directly after all startup tasks have ran
        /// </summary>
        public static event ContainerDelegate OnStartupTasksRan;

        #endregion
    }
}
