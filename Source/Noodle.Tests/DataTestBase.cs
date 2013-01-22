using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Noodle.Data;
using Noodle.MongoDB;

namespace Noodle.Tests
{
    /// <summary>
    /// Base class for monodb testing
    /// </summary>
    public abstract class DataTestBase : TestBase
    {
        private IDisposable _serverScope;

        public override void SetUp()
        {
            // start the server for each class
            _serverScope = ServerScope();

            base.SetUp();

            // and set the kernel up to point to it
            _kernel.Rebind<IConnectionProvider>().ToMethod(context => new SqlConnectionProvider("mongodb://localhost:{0}/?safe=true".F(PortNumber)));
            //_kernel.Rebind<IConnectionProvider>().ToMethod(context => new SqlConnectionProvider("mongodb://192.168.5.161:27017/safe=true"));
            _kernel.Resolve<IMongoService>().GetServer().Reconnect(); // this clears the connection pool
        }

        public override void TearDown()
        {
            base.TearDown();

            // and then close the server after each class is finished
            _serverScope.Dispose();
        }

        /// <summary>
        /// The port number for the server to be started on
        /// </summary>
        public virtual int PortNumber
        {
            get { return 8989; }
        }

        /// <summary>
        /// Build the server (which must be disposed of to stop the server)
        /// </summary>
        /// <returns></returns>
        public virtual IDisposable ServerScope()
        {
            return new MongoServerScope(PortNumber);
        }
    }
}
