using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Noodle.Management.Logging.ViewModels
{
    public class LogListViewModel
    {
        public LogListViewModel()
        {
            AvailableLogLevels = new List<SelectListItem>();
        }

        public DateTime? CreatedOnFrom { get; set; }

        public DateTime? CreatedOnTo { get; set; }

        public string Message { get; set; }

        public int LogLevelId { get; set; }

        public IList<SelectListItem> AvailableLogLevels { get; set; }
    }
}