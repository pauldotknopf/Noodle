//#define LOGGING
using System;
using System.Collections.Generic;
using System.Linq;
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
#if LOGGING
        private static Logger<EngineContext> _logger = new Logger<EngineContext>(); 
#endif

        #region Current

        /// <summary>
        /// Return the singleton kernel
        /// </summary>
        public static TinyIoCContainer Current
        {
            get
            {
                Configure(false);
                return Singleton<TinyIoCContainer>.Instance;
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
            set
            {
                if(value == null)
                    throw new ArgumentNullException("value");
                Singleton<ITypeFinder>.Instance = value;
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
            if (Singleton<TinyIoCContainer>.Instance == null || force)
            {
                lock (ContainerCreationLockObject)
                {
                    // someone may have waited for the lock, but it has been built for them, check one more time.
                    if (Singleton<TinyIoCContainer>.Instance == null || force)
                    {
                        #if LOGGING
                        _logger.Info("Creating the container");
                        #endif

                        var container = new TinyIoCContainer();

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
                        Singleton<TinyIoCContainer>.Instance = container;

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
            return Current.Resolve<T>();
        }

        public static IEnumerable<T> ResolveAll<T>() where T : class
        {
            return Current.ResolveAll<T>();
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

        private static void RegisterDependencyRegistrar(ITypeFinder typeFinder, TinyIoCContainer container)
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

        public static void RunStartupTasks(TinyIoCContainer container)
        {
            #if LOGGING
            _logger.Info("Running startup tasks");
            #endif

            var startupServiceTypes = container.GetServicesOf<IStartupTask>();
            var startupServices = new List<IStartupTask>();

            foreach(var startUpServiceType in startupServiceTypes)
            {
                #if LOGGING
                _logger.Info("Creating an instance of the startup task {0}".F(startUpServiceType.FullName));
                #endif
                startupServices.Add(container.Resolve(startUpServiceType) as IStartupTask);
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

        static EngineContext()
        {
            InitializeExcludedAndIncludedAssemblies();
        }

        #endregion

        #region Events

        public delegate void ContainerDelegate(TinyIoCContainer container);

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
