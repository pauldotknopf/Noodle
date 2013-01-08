using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Noodle.Configuration;

namespace Noodle.Data
{
    public class ConnectionProvider : IConnectionProvider
    {
        private readonly NoodleCoreConfiguration _configuration;
        private readonly System.Configuration.ConnectionStringSettingsCollection _connectionStrings;

        public ConnectionProvider(NoodleCoreConfiguration configuration, System.Configuration.ConnectionStringSettingsCollection connectionStrings)
        {
            _configuration = configuration;
            _connectionStrings = connectionStrings;
        }

        public IDbConnection GetDbConnection()
        {
            if(string.IsNullOrEmpty(_configuration.ConnectionStrings.DefaultConnectionStringName))
            {
                throw new NoodleException("The default connection string name must be provided.");
            }

            var connectionString = _connectionStrings[_configuration.ConnectionStrings.DefaultConnectionStringName];

            if(connectionString == null)
            {
                throw new NoodleException("The default connection string name was \"{0}\" but it wasn't found within the connection strings section.", 
                    _configuration.ConnectionStrings.DefaultConnectionStringName);
            }

            return BuildDbConnection(connectionString);
        }

        public System.Data.IDbConnection GetDbConnection(string name, bool throwErrorIfMissing = false)
        {
            Contract.IsNotNullOrWhitespace(name, "name");

            var pointers = _configuration.ConnectionStrings.AllElements.Where(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)).ToList();

            if(pointers.Count > 1)
            {
                throw new NoodleException("There was more than one connection string pointer with the name \"{0}\".", name);
            }
            if (pointers.Count == 0)
            {
                if (throwErrorIfMissing)
                {
                    throw new NoodleException("The connection string pointer entry \"{0}\" doesn't exist.", name);
                }

                return GetDbConnection();
            }

            var connectionStringName = pointers[0].ConnectionStringName;
            var connectionString = _connectionStrings[connectionStringName];

            if (connectionString == null)
            {
                if (throwErrorIfMissing)
                {
                    throw new NoodleException("The connection string \"{0}\" doesn't exist. It was referenced by pointer \"{1}\".", connectionStringName, name);
                }
                return GetDbConnection();
            }

            return BuildDbConnection(connectionString);
        }

        /// <summary>
        /// This is only false for unit testing in some situations
        /// </summary>
        public bool CanOpenClose
        {
            get { return true; }
        }

        /// <summary>
        /// Always returns sql connection. Maybe someone wants to override and change that?
        /// </summary>
        /// <returns></returns>
        protected virtual IDbConnection BuildDbConnection(System.Configuration.ConnectionStringSettings connectionString)
        {
            return new SqlConnection(connectionString.ConnectionString);
        }
    }
}
