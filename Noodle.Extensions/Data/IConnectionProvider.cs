using System.Data;

namespace Noodle.Extensions.Data
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
