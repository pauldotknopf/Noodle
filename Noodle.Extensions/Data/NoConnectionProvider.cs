using System;
using System.Data;

namespace Noodle.Extensions.Data
{
    /// <summary>
    /// Provide connections to anyone who cares
    /// </summary>
    public class NoConnectionProvider : IConnectionProvider
    {
        /// <summary>
        /// This method will retrieve the default data connection
        /// </summary>
        /// <returns></returns>
        public IDbConnection GetDbConnection()
        {
            throw new NotImplementedException("Please implement your own connection provider");
        }

        /// <summary>
        /// This method will retrieve the default data connection
        /// </summary>
        /// <returns></returns>
        public virtual string GetConnectionString()
        {
            throw new NotImplementedException("Please implement your own connection provider");
        }
    }
}
