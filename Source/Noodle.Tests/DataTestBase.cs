using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Noodle.Data;
using Noodle.MongoDB;

namespace Noodle.Tests
{
    public abstract class DataTestBase : TestBase
    {
        private static readonly object ServerScopeLockObject = new object();

        public virtual int PortNumber
        {
            get { return 8989; }
        }

        public override Ninject.IKernel GetTestKernel(params Engine.IDependencyRegistrar[] dependencyRegistrars)
        {
            var kernel = base.GetTestKernel(dependencyRegistrars);
            kernel.Rebind<IConnectionProvider>().ToMethod(context => new SqlConnectionProvider("mongodb://localhost:{0}/?safe=true".F(PortNumber)));
            return kernel;
        }

        public virtual IDisposable ServerScope()
        {
            return new MongoServerScope(PortNumber);
        }
    }
}
