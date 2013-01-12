using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;

namespace Noodle.MongoDB
{
    /// <summary>
    /// Handle mongo sever/database interaction
    /// </summary>
    public interface IMongoService
    {
        /// <summary>
        /// Get the client, optionaly for a given name
        /// </summary>
        /// <param name="serverName"></param>
        /// <returns></returns>
        MongoClient GetClient(string serverName = null);

        /// <summary>
        /// Get sever, optionally a the given name
        /// </summary>
        /// <param name="serverName"></param>
        /// <returns></returns>
        MongoServer GetServer(string serverName = null);

        /// <summary>
        /// Get a database for th given server. Optionally specify a database name, or use the default one.
        /// </summary>
        /// <param name="severName"></param>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        MongoDatabase GetDatabase(string severName = null, string databaseName = "Default");
    }
}
