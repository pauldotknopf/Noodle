using System.Collections.Generic;

namespace Noodle.Data.Deploy.Models
{
    public class DeployResponse : ActionResponse
    {
        public DeployResponse()
        {
            Output = new Dictionary<string, List<string>>();
            successMessage = "The schema has been deployed.";
        }

        public Dictionary<string, List<string>> Output { get; set; }
    }
}