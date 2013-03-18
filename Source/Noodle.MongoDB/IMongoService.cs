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
        /// <returns></returns>
        MongoClient GetClient();

        /// <summary>
        /// Get sever, optionally a the given name
        /// </summary>
        /// <returns></returns>
        MongoServer GetServer();

        /// <summary>
        /// Get a database for th given server. Optionally specify a database name, or use the default one.
        /// </summary>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        MongoDatabase GetDatabase(string databaseName = "Default");
    }
}
