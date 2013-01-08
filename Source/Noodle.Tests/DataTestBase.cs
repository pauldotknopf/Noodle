using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Noodle.Data;

namespace Noodle.Tests
{
    [TestClass]
    public abstract class DataTestBase : TestBase
    {
        public string DataFilePath;
        public string LogFilePath;

        public virtual string GetServerInstance()
        {
            return @"(local)\SQLEXPRESS";
        }

        public virtual string GetConnectionString()
        {
            return string.Format("Data Source={0};Integrated Security=True;", GetServerInstance());
        }

        public virtual string GetDatabaseName()
        {
            return this.GetType().Name;
        }

        public abstract List<AbstraceEmbeddedSchemaProvider> GetSchemaProviders();

        private string GetExecutingDirectory()
        {
            var directory = new DirectoryInfo("C:\\_DataTestDatabases");
            
            if (!directory.Exists)
                directory.Create();

            return directory.FullName;
        }

        public void InitializePaths()
        {
            var directoryPath = this.GetExecutingDirectory();

            this.DataFilePath = Path.Combine(directoryPath, GetDatabaseName() + ".mdf");
            this.LogFilePath = Path.Combine(directoryPath, GetDatabaseName() + "_log.ldf");
        }

        public virtual IConnectionProvider GetConnectionProvider()
        {
            return new SqlConnectionProvider(GetConnectionString() + string.Format(";Initial Catalog={0};", GetDatabaseName()));
        }

        public override TinyIoC.TinyIoCContainer GetTestKernel(params Engine.IDependencyRegistrar[] dependencyRegistrars)
        {
            var kernel = base.GetTestKernel(dependencyRegistrars);
            kernel.Register((context, namedParams) => GetConnectionProvider()).AsSingleton();
            return kernel;
        }
    }

    public static class DataTestBaseHelper
    {
        private static readonly object LockObject = new object();

        public static void DropCreateDatabase<T>() where T : DataTestBase, new()
        {
            var testBase = new T();
            Trace.WriteLine("DataTest: Creating {0}".F(typeof(T).Name));
            Trace.WriteLine("DataTest: Locking {0}".F(typeof(T).Name));
            Monitor.Enter(LockObject);

            try
            {
                testBase.InitializePaths();
                DeleteDatabaseIfExists(testBase);
                CreateDatabase(testBase);

                var schemaProviders = testBase.GetSchemaProviders();
                Trace.WriteLine("DataTest: using schema providers ({0}) for database {1}.".F(
                    string.Join(", ", schemaProviders.Select(x => x.GetType().Name).ToArray()),
                    testBase.GetDatabaseName()));

                var schemas = schemaProviders
                        .SelectMany(x => new EmbeddedSchemaPlanner().GetPlansFor(x))
                        .Distinct(new EmbeddedSchemaPlanner())
                        .ToList();

                Trace.WriteLine("DataTest: plan is to use execute providers: {0}".F(
                    string.Join(", ", schemas.Select(x => x.GetType().Name).ToArray())));

                foreach (var schemaProvider in schemas)
                {
                    bool hasError = false;
                    if (schemaProvider is SqlPackageEmbeddedSchemaProvider)
                    {
                        (schemaProvider as SqlPackageEmbeddedSchemaProvider).OnError =
                            (error) =>
                            {
                                hasError = true;
                                Trace.WriteLine("sqlpackage error    : " + error);
                            };
                        (schemaProvider as SqlPackageEmbeddedSchemaProvider).OnOutput =
                            (output) => Trace.WriteLine("sqlpackage output  : " + output);
                        (schemaProvider as SqlPackageEmbeddedSchemaProvider).OnWarning =
                            (warning) => Trace.WriteLine("sqlpackage warning : " + warning);
                    }

                    Trace.WriteLine("DataTest: executing plan {0}".F(schemaProvider.GetType().Name));

                    AbstraceEmbeddedSchemaProvider provider = schemaProvider;
                    RetryUtility.RetryAction(() =>
                    {
                        provider.PublishTo(testBase.GetConnectionString(), testBase.GetDatabaseName());
                        if (hasError) throw new Exception();
                        hasError = false;
                    }, 3, 3,
                    (exception) => OutputDebugInfo(testBase));

                    Trace.WriteLine("DataTest: executed plan {0}".F(schemaProvider.GetType().Name));
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("DataTest: Error creating database. {0}".F(ex.Message));
                Trace.WriteLine("DataTest: Unlocking {0}".F(typeof(T).Name));
                Monitor.Exit(LockObject);

                DropDatabase<T>();

                throw;
            }
            Trace.WriteLine("DataTest: Created {0}".F(typeof(T).Name));
            Trace.WriteLine("DataTest: Unlocking {0}".F(typeof(T).Name));
            Monitor.Exit(LockObject);
        }

        private static void OutputDebugInfo<T>(T testBase) where T : DataTestBase
        {
            Trace.Write("DataTest: Get all databases...");
            ExecuteSqlText(testBase, "EXEC sp_databases");
            Trace.Write("DataTest: Get all sessions...");
            ExecuteSqlText(testBase, "select spid, status, loginame, hostname, blocked, db_name(dbid), cmd from master..sysprocesses");
            Trace.Write("DataTest: Get all users for the database...");
            ExecuteSqlText(testBase, "sp_helpuser");
        }

        private static void ExecuteSqlText(DataTestBase dataTestBase, string commandText)
        {
            using (var sqlConnection = new SqlConnection(dataTestBase.GetConnectionString()))
            {
                using (var command = new SqlCommand(commandText, sqlConnection))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    sqlConnection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var values = new List<string>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            values.Add("\"{0}\":\"{1}\"".F(reader.GetName(i), reader.GetValue(i)));
                        }
                        Trace.WriteLine("DataTest: SqlOutput: " + string.Join(", ", values.ToArray()));
                    }
                }
            }
        }

        public static void DropDatabase<T>() where T : DataTestBase, new()
        {
            Trace.WriteLine("DataTest: Deleting {0}".F(typeof(T).Name));
            Trace.WriteLine("DataTest: Locking {0}".F(typeof(T).Name));
            Monitor.Enter(LockObject);

            var testBase = new T();
            DeleteDatabaseIfExists(testBase);

            Trace.WriteLine("DataTest: Unlocking {0}".F(typeof(T).Name));
            Monitor.Exit(LockObject);
            Trace.WriteLine("DataTest: Deleted {0}".F(typeof(T).Name));
        }

        private static void CreateDatabase(DataTestBase testBase)
        {
            Trace.WriteLine("DataTest: Creating database {0}".F(testBase.GetDatabaseName()));

            var server = new Server(testBase.GetServerInstance());
            var database = new Database(server, testBase.GetDatabaseName());

            var fileGroup = new FileGroup(database, "PRIMARY");
            database.FileGroups.Add(fileGroup);

            var dataFile = new DataFile(fileGroup, testBase.GetDatabaseName(), testBase.DataFilePath);
            dataFile.Growth = 10;
            dataFile.GrowthType = FileGrowthType.Percent;
            fileGroup.Files.Add(dataFile);

            var logFile = new LogFile(database, testBase.GetDatabaseName() + "_log", testBase.LogFilePath);
            logFile.Growth = 10;
            logFile.GrowthType = FileGrowthType.Percent;
            database.LogFiles.Add(logFile);

            database.Create();

            database.Refresh();

            database.SetOffline();
            database.SetOnline();

            database.Refresh();

            while (database.Status != DatabaseStatus.Normal)
            {
                database.Refresh();
                Thread.Sleep(1000);
            }

            Thread.Sleep(5000);

            Trace.WriteLine("DataTest: Created database {0}".F(testBase.GetDatabaseName()));

            System.Security.Principal.WindowsIdentity user = System.Security.Principal.WindowsIdentity.GetCurrent();

            //Trace.WriteLine("DataTest: Granting login rights for current user '' (integrated security for unit tests).".F(user.Name));
            //database.ExecuteNonQuery("sp_grantdbaccess @loginame= '{0}', @name_in_db ='{0}'".F(user.Name, database.Name));
            //database.ExecuteNonQuery("CREATE LOGIN [{0}] FROM WINDOWS;".F(user.Name));
            //database.ExecuteNonQuery("EXEC sp_addrolemember N'db_owner', N'{1}'".F(database.Name, user.Name));
        }

        private static void DeleteDatabaseIfExists(DataTestBase testBase)
        {
            testBase.InitializePaths();
            var server = new Server(testBase.GetServerInstance());
            server.Refresh();
    
            if (server.Databases.Contains(testBase.GetDatabaseName()))
            {
                Trace.WriteLine("DataTest: Database {0} exists. Deleteing it.".F(testBase.GetDatabaseName()));
                // Something might be caching an open connection, so tell everyone
                // to screw off by forcing their connections shut. If we don't do this,
                // the DetachDatabase() call could faile.
                var sql = string.Format(
                    CultureInfo.InvariantCulture,
                    "ALTER DATABASE {0} SET SINGLE_USER WITH ROLLBACK IMMEDIATE",
                    testBase.GetDatabaseName());
                server.Databases[testBase.GetDatabaseName()].ExecuteNonQuery(sql);

                server.DetachDatabase(testBase.GetDatabaseName(), true);

                File.Delete(testBase.DataFilePath);
                File.Delete(testBase.LogFilePath);
            }
        }
    }
}
