using System;
using System.Data;

namespace Noodle.Data
{
    /// <summary>
    /// Common helper for querying connections
    /// </summary>
    public interface IDatabaseService
    {
        /// <summary>
        /// query a connection and return a result
        /// </summary>
        /// <typeparam name="TReturnType"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        TReturnType QueryConnection<TReturnType>(Func<IDbCommand, TReturnType> query);

        /// <summary>
        /// query a connection without a return value
        /// </summary>
        /// <param name="query"></param>
        void QueryConnection(Action<IDbCommand> query);

        /// <summary>
        /// The database type we are currently trying to query
        /// </summary>
        DatabaseTypeEnum DatabaseType { get; }
    }
}
