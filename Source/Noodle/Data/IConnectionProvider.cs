using System.Data;

namespace Noodle.Data
{
    /// <summary>
    /// A provider of connections (go figuire).
    /// </summary>
    public interface IConnectionProvider
    {
        /// <summary>
        /// This method will retrieve the default data connection
        /// </summary>
        /// <returns></returns>
        IDbConnection GetDbConnection();

        /// <summary>
        /// This method will retrieve the default data connection
        /// </summary>
        /// <returns></returns>
        string GetConnectionString();

        /// <summary>
        /// This method will try and retrieve a connection string matching the given name.
        /// It will return the default connection string if none is found (unless throwErrorIfMissing).
        /// </summary>
        /// <param name="name"></param>
        /// <param name="throwErrorIfMissing">If true and the name isn't found, an error is thrown.</param>
        /// <returns></returns>
        IDbConnection GetDbConnection(string name, bool throwErrorIfMissing = false);

        /// <summary>
        /// This method will try and retrieve a connection string matching the given name.
        /// It will return the default connection string if none is found (unless throwErrorIfMissing).
        /// </summary>
        /// <param name="name"></param>
        /// <param name="throwErrorIfMissing">If true and the name isn't found, an error is thrown.</param>
        /// <returns></returns>
        string GetConnectionString(string name, bool throwErrorIfMissing = false);
    }
}
