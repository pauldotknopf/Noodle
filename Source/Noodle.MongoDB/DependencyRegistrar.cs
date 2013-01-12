using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Noodle.Engine;

namespace Noodle.MongoDB
{
    /// <summary>
    /// Register mongodb service
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(Ninject.IKernel kernel, ITypeFinder typeFinder, Configuration.ConfigurationManagerWrapper configuration)
        {
            kernel.Bind<IMongoService>().To<MongoService>().InSingletonScope();
        }

        public int Importance
        {
            get { return 0; }
        }
    }
}
