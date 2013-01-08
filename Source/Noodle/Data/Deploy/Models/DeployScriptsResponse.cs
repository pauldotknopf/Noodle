using System;
using System.Collections.Generic;

namespace Noodle.Data.Deploy.Models
{
    /// <summary>
    /// This is the resonse of a generation of some delpoy scripts
    /// </summary>
    public class DeployScriptsResponse : ActionResponse
    {
        public DeployScriptsResponse()
        {
            Scripts = new Dictionary<string, DeployScript>();
            successMessage = "The deploy scripts have been generated.";
        }

        public Dictionary<string, DeployScript> Scripts { get; protected set; }

        /// <summary>
        /// This houses temporary delpoy scripts that will be held in memory for downloads
        /// </summary>
        public class DeployScript
        {
            /// <summary>
            /// The schema group this was generated for
            /// </summary>
            public string ForSchema { get; set; }

            /// <summary>
            /// The schema this one represents (dependencies, etc)
            /// </summary>
            public string Schema { get; set; }

            /// <summary>
            /// The script
            /// </summary>
            public string Script { get; set; }

            /// <summary>
            /// Unique key used for downloading
            /// </summary>
            public Guid Key { get; set; }
        }
    }
}
