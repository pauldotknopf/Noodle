using System;
using MongoDB.Driver;
using Noodle.Extensions.Data;
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

            _container.Register<IConnectionProvider>((context, p) => new SqlConnectionProvider("mongodb://localhost:{0}/?safe=true".F(PortNumber)));

            // this clears the connection pool
            try
            {
                _container.Resolve<IMongoService>().GetServer().Reconnect();
            }
            catch (MongoConnectionException)
            {
                // some tests may still have a connetion. since we restarted the server, those connections have errors.
                // no biggy. reconnect again.
                _container.Resolve<IMongoService>().GetServer().Reconnect();
            }
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
