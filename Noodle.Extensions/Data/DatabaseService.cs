using System;
using System.Data;

namespace Noodle.Extensions.Data
{
    /// <summary>
    /// Common helper for querying connections
    /// </summary>
    /// <remarks></remarks>
    public class DatabaseService : IDatabaseService
    {
        private readonly IConnectionProvider _connectionProvider;
        private DatabaseTypeEnum? _databaseType;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseService"/> class.
        /// </summary>
        /// <param name="connectionProvider">The connection provider.</param>
        /// <remarks></remarks>
        public DatabaseService(IConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        /// <summary>
        /// query a connection and return a result
        /// </summary>
        /// <typeparam name="TReturnType">The type of the return type.</typeparam>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public virtual TReturnType QueryConnection<TReturnType>(Func<IDbCommand, TReturnType> query)
        {
            TReturnType result;

            var connection = _connectionProvider.GetDbConnection();

            try
            {
                var command = connection.CreateCommand();
                connection.Open();
                result = query(command);
                connection.Close();
            }
            finally
            {
                if(connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }

            return result;
        }

        /// <summary>
        /// query a connection without a return value
        /// </summary>
        /// <param name="query">The query.</param>
        /// <remarks></remarks>
        public void QueryConnection(Action<IDbCommand> query)
        {
            QueryConnection(command =>
                                {
                                    query(command);
                                    return -1;
                                });
        }

        /// <summary>
        /// The database type we are currently trying to query
        /// </summary>
        /// <remarks></remarks>
        public DatabaseTypeEnum DatabaseType
        {
            get
            {
                if(_databaseType == null)
                {
                    switch(_connectionProvider.GetDbConnection().GetType().Name)
                    {
                        case "SqlConnection":
                            _databaseType = DatabaseTypeEnum.SqlServer;
                            break;
                        case "SQLiteConnection":
                            _databaseType = DatabaseTypeEnum.SqlLite;
                            break;
                        case "SqlCeConnection":
                            _databaseType = DatabaseTypeEnum.SqlCe;
                            break;
                        default:
                            _databaseType = DatabaseTypeEnum.Unkown;
                            break;
                    }
                }
                return _databaseType.Value;
            }
        }
    }
}
