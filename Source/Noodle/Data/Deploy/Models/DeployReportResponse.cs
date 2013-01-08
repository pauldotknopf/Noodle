using System.Collections.Generic;
using Noodle.Data.SqlPackage;

namespace Noodle.Data.Deploy.Models
{
    public class DeployReportResponse : ActionResponse
    {
        public DeployReportResponse()
        {
            successMessage = "The deploy report has been created.";
            DeploymentReports = new Dictionary<string, DeploymentReport>();
        }

        public Dictionary<string, DeploymentReport> DeploymentReports { get; set; }
    }
}