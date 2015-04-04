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
    }
}
