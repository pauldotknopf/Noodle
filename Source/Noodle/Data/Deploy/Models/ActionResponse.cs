using System.Collections.Generic;
using System.Linq;

namespace Noodle.Data.Deploy.Models
{
    /// <summary>
    /// Common response properties for the delpoy service
    /// </summary>
    public class ActionResponse
    {
        public ActionResponse()
        {
            errors = new List<string>();
        }

        public List<string> errors { get; set; }

        public bool successful { get { return !errors.Any(); } }

        public string successMessage { get; set; }
    }
}