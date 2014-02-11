using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using Noodle.Localization.Services;
using Noodle.Management.Library;
using Noodle.Management.Logging.ViewModels;
using Noodle.Security.Permissions;
using Noodle.Web.Mvc;

namespace Noodle.Management.Logging.Controllers
{
    public class LogController : BaseNoodleController
    {
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogController" /> class.
        /// </summary>
        /// <param name="permissionService">The permission service.</param>
        /// <param name="localizationService">The localization service.</param>
        public LogController(IPermissionService permissionService, ILocalizationService localizationService)
        {
            _permissionService = permissionService;
            _localizationService = localizationService;
        }

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            return Handle<LogListViewModel>()
                .RequiredPermissions(_permissionService, StandardPermissionProvider.ManageSystemLog)
                .HandleWith(() =>
                {
                    var model = new LogListViewModel();
                    model.AvailableLogLevels = Noodle.Logging.LogLevel.Debug.ToSelectList(_localizationService, false).ToList();
                    model.AvailableLogLevels.Insert(0, new SelectListItem()
                    {
                        Text = "All",
                        Value = "0"
                    });
                    return model;
                });
        }
    }
}