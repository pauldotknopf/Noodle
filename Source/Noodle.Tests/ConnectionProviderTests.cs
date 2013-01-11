using System.Collections.Generic;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ninject;
using Noodle.Configuration;
using Noodle.Data;

namespace Noodle.Tests
{
    [TestClass]
    public class ConnectionProviderTests : TestBase
    {
        private List<ConnectionStringElement> _connectionStringPointers = new List<ConnectionStringElement>();
        private ConnectionStringSettingsCollection _connectionStrings = new ConnectionStringSettingsCollection();

        [TestMethod]
        public void Connection_providers_throws_error_for_named_connections()
        {
            // setup
            _connectionStringPointers.Add(new ConnectionStringElement 
            { 
                Name = "testName", 
                ConnectionStringName = "connection1" 
            });
            _connectionStringPointers.Add(new ConnectionStringElement
            {
                Name = "testName2",
                ConnectionStringName = "connection2"
            });
            _connectionStrings.Add(new ConnectionStringSettings
            {
                ConnectionString = "server=somewhere",
                Name = "connection1"
            });
            var connectionProvider = GetTestKernel().Get<IConnectionProvider>();

            // assert
            ExceptionAssert.Throws<NoodleException>(() => connectionProvider.GetDbConnection("testName2", true));
            ExceptionAssert.Throws<NoodleException>(() => connectionProvider.GetDbConnection("testName3", true));
            connectionProvider.GetDbConnection("testName", true).ShouldNotBeNull();
        }

        [TestMethod]
        public void Can_resolve_default_connection()
        {
            // setup
            _connectionStringPointers.Add(new ConnectionStringElement
            {
                Name = "testName",
                ConnectionStringName = ""
            });
            _connectionStrings.Add(new ConnectionStringSettings
                                                         {
                                                             Name = "Noodle",
                                                             ConnectionString = "server=overhere"
                                                         });
            var connectionProvider = GetTestKernel().Get<IConnectionProvider>();

            // assert
            connectionProvider.GetDbConnection("testName").ConnectionString.ShouldEqual("server=overhere");
            connectionProvider.GetDbConnection().ConnectionString.ShouldEqual("server=overhere");
        }

        [TestMethod]
        public void Can_resolve_connections()
        {
            // setup
            _connectionStringPointers.Add(new ConnectionStringElement
            {
                Name = "testName",
                ConnectionStringName = "NoodleCustom"
            });
            _connectionStrings.Add(new ConnectionStringSettings
            {
                Name = "Noodle",
                ConnectionString = "server=overhere"
            });
            _connectionStrings.Add(new ConnectionStringSettings
            {
                Name = "NoodleCustom",
                ConnectionString = "server=overthere"
            });
            var connectionProvider = GetTestKernel().Get<IConnectionProvider>();

            // assert
            connectionProvider.GetDbConnection("testName").ConnectionString.ShouldEqual("server=overthere");
            connectionProvider.GetDbConnection().ConnectionString.ShouldEqual("server=overhere");
        }

        public override IKernel GetTestKernel(params Engine.IDependencyRegistrar[] dependencyRegistrars)
        {
            var kernel = base.GetTestKernel(dependencyRegistrars);

            var configuration = new Mock<NoodleCoreConfiguration>();
            configuration.Setup(x => x.ConnectionStrings).Returns(() =>
            {
                var connectionStrings = new ConnectionStringCollection();
                foreach(var pointer in _connectionStringPointers)
                {
                    connectionStrings.Add(pointer);
                }
                return connectionStrings;
            });
            kernel.Rebind<NoodleCoreConfiguration>().ToConstant(configuration.Object);
            kernel.Rebind<ConnectionStringSettingsCollection>().ToConstant(_connectionStrings);
            return kernel;
        }
    }
}
