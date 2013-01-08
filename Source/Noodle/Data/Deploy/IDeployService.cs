using Noodle.Data.Deploy.Models;

namespace Noodle.Data.Deploy
{
    /// <summary>
    /// Handles common functionality for deploying embedded schemas
    /// </summary>
    public interface IDeployService
    {
        /// <summary>
        /// Generate a deployment report to the given connection string
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="schemaName"></param>
        /// <returns></returns>
        DeployReportResponse DeployReport(ConnectionString connectionString, string schemaName);

        /// <summary>
        /// Deploy an embedded schema to the target connection string
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="schemaName"></param>
        /// <returns></returns>
        DeployResponse Deploy(ConnectionString connectionString, string schemaName);

        /// <summary>
        /// Generate deploy scripts for the target connection string
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="schemaName"></param>
        /// <returns></returns>
        DeployScriptsResponse DeployScripts(ConnectionString connectionString, string schemaName);
    }
}
