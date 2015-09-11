using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using Noodle.Extensions.Data;

namespace Noodle.MongoDB
{
    /// <summary>
    /// Handle mongo sever/database interaction
    /// </summary>
    public class MongoService : IMongoService
    {
        private readonly IConnectionProvider _connectionProvider;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="connectionProvider"></param>
        public MongoService(IConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        /// <summary>
        /// Get the client, optionaly for a given name
        /// </summary>
        /// <returns></returns>
        public virtual MongoClient GetClient()
        {
            return new MongoClient(_connectionProvider.GetConnectionString());
        }

        /// <summary>
        /// Get sever, optionally a the given name
        /// </summary>
        /// <returns></returns>
        public virtual MongoServer GetServer()
        {
            return GetClient().GetServer();
        }

        /// <summary>
        /// Get a database for th given server. Optionally specify a database name, or use the default one.
        /// </summary>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public virtual MongoDatabase GetDatabase( string databaseName = "Default")
        {
            return GetServer().GetDatabase(databaseName);
        }
    }
}
