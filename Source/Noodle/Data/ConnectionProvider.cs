using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Noodle.Configuration;

namespace Noodle.Data
{
    /// <summary>
    /// Provide connections to anyone who cares
    /// </summary>
    public class ConnectionProvider : IConnectionProvider
    {
        private readonly NoodleCoreConfiguration _configuration;
        private readonly System.Configuration.ConnectionStringSettingsCollection _connectionStrings;

        public ConnectionProvider(NoodleCoreConfiguration configuration, ConnectionStringSettingsCollection connectionStrings)
        {
            _configuration = configuration;
            _connectionStrings = connectionStrings;
        }

        /// <summary>
        /// This method will retrieve the default data connection
        /// </summary>
        /// <returns></returns>
        public virtual IDbConnection GetDbConnection()
        {
            return BuildDbConnection(GetConnectionStringSetting());
        }

        /// <summary>
        /// This method will retrieve the default data connection
        /// </summary>
        /// <returns></returns>
        public virtual string GetConnectionString()
        {
            return GetConnectionStringSetting().ConnectionString;
        }

        /// <summary>
        /// This method will try and retrieve a connection string matching the given name.
        /// It will return the default connection string if none is found (unless throwErrorIfMissing).
        /// </summary>
        /// <param name="name"></param>
        /// <param name="throwErrorIfMissing">If true and the name isn't found, an error is thrown.</param>
        /// <returns></returns>
        public virtual IDbConnection GetDbConnection(string name, bool throwErrorIfMissing = false)
        {
            return BuildDbConnection(GetConnectionStringSetting(name, throwErrorIfMissing));
        }

        /// <summary>
        /// This method will try and retrieve a connection string matching the given name.
        /// It will return the default connection string if none is found (unless throwErrorIfMissing).
        /// </summary>
        /// <param name="name"></param>
        /// <param name="throwErrorIfMissing">If true and the name isn't found, an error is thrown.</param>
        /// <returns></returns>
        public virtual string GetConnectionString(string name, bool throwErrorIfMissing = false)
        {
            return GetConnectionStringSetting(name, throwErrorIfMissing).ConnectionString;
        }

        #region Helpers

        public virtual ConnectionStringSettings GetConnectionStringSetting()
        {
            if (string.IsNullOrEmpty(_configuration.ConnectionStrings.DefaultConnectionStringName))
            {
                throw new NoodleException("The default connection string name must be provided.");
            }

            var connectionString = _connectionStrings[_configuration.ConnectionStrings.DefaultConnectionStringName];

            if (connectionString == null)
            {
                throw new NoodleException("The default connection string name was \"{0}\" but it wasn't found within the connection strings section.",
                    _configuration.ConnectionStrings.DefaultConnectionStringName);
            }

            return connectionString;
        }

        public virtual ConnectionStringSettings GetConnectionStringSetting(string name, bool throwErrorIfMissing)
        {
            var pointers = _configuration.ConnectionStrings.AllElements.Where(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)).ToList();

            if (pointers.Count > 1)
            {
                throw new NoodleException("There was more than one connection string pointer with the name \"{0}\".", name);
            }
            if (pointers.Count == 0)
            {
                if (throwErrorIfMissing)
                {
                    throw new NoodleException("The connection string pointer entry \"{0}\" doesn't exist.", name);
                }

                return GetConnectionStringSetting();
            }

            var connectionStringName = pointers[0].ConnectionStringName;
            var connectionString = _connectionStrings[connectionStringName];

            if (connectionString == null)
            {
                if (throwErrorIfMissing)
                {
                    throw new NoodleException("The connection string \"{0}\" doesn't exist. It was referenced by pointer \"{1}\".", connectionStringName, name);
                }
                return GetConnectionStringSetting();
            }

            return connectionString;
        }

        /// <summary>
        /// Always returns sql connection. Maybe someone wants to override and change that?
        /// </summary>
        /// <returns></returns>
        protected virtual IDbConnection BuildDbConnection(ConnectionStringSettings connectionString)
        {
            return new SqlConnection(connectionString.ConnectionString);
        }

        #endregion
    }
}
