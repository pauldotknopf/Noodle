using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using Noodle.Configuration;
using Noodle.Engine;
using Noodle.TinyIoC;

namespace Noodle
{
    public class EngineContext
    {
        private static readonly object ContainerCreationLockObject = new object();

        #region Current
        /// <summary>
        /// Return the singleton kernel element
        /// </summary>
        public static TinyIoC.TinyIoCContainer Current
        {
            get
            {
                Configure(false);
                return Singleton<TinyIoC.TinyIoCContainer>.Instance;
            }
        }
        #endregion

        #region Configure
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
                        var kernel = new TinyIoCContainer();

                        CoreDependencyRegistrar.Register(kernel);
                        var configuration = kernel.Resolve<ConfigurationManagerWrapper>();
                        var typeFinder = kernel.Resolve<ITypeFinder>();

                        // register everything!
                        RegisterAttributedServices(kernel);
                        RegisterDependencyRegistrar(typeFinder, kernel, configuration);

                        // set the kernel to the static accessor
                        Singleton<TinyIoC.TinyIoCContainer>.Instance = kernel;
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
            return Current.Resolve<T>();
        }

        public static IEnumerable<T> ResolveAll<T>() where T : class
        {
            return Current.ResolveAll<T>();
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

        #region Private helpers

        private static void RegisterDependencyRegistrar(ITypeFinder typeFinder, TinyIoC.TinyIoCContainer kernel, ConfigurationManagerWrapper configuration)
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

        public static void RunStartupTasks(TinyIoC.TinyIoCContainer kernel)
        {
            var services = kernel.GetServiceWithImplementationsOf<IStartupTask>();
            var startUpTasks = services.Select(kernel.Resolve).Cast<IStartupTask>().ToList();

            foreach (var startupTask in startUpTasks.OrderBy(x => x.Order))
            {
                startupTask.Execute();
            }
        }

        private static void RegisterAttributedServices(TinyIoC.TinyIoCContainer kernel)
        {
            var serviceRegistrar = kernel.Resolve<ServiceRegistrator>();
            serviceRegistrar.RegisterServices(kernel);
        }

        #endregion
    }
}
