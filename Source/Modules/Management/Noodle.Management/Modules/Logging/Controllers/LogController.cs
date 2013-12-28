using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Noodle.Management.Logging.Controllers
{
    public class LogController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            return Handle<LogListModel>()
                .RequiredPermissions(_permissionService, StandardPermissionProvider.ManageSystemLog)
                .HandleWith(() =>
                {
                    var model = new LogListModel();
                    model.AvailableLogLevels = LogLevel.Debug.ToSelectList(_localizationService, false).ToList();
                    model.AvailableLogLevels.Insert(0, new SelectListItem()
                    {
                        Text = _localizationService.GetResource(() => Resources.Resources.Common.All),
                        Value = "0"
                    });
                    return model;
                });
        }
    }
}