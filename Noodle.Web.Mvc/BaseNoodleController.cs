using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Noodle.Serialization;

namespace Noodle.Web.Mvc
{
    /// <summary>
    /// Base controller with common functionality
    /// </summary>
    /// <remarks></remarks>
    public abstract class BaseNoodleController : Controller
    {
        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception != null)
                LogException(filterContext.Exception);
            base.OnException(filterContext);
        }

        protected virtual T Model<T>() where T : class, new()
        {
            return new T();
        }

        protected virtual EnrichedViewResult<T> EnrichedView<T>(T model)
        {
            return EnrichedView(null, model);
        }

        protected virtual EnrichedViewResult<T> EnrichedView<T>(string viewName, T model)
        {
            if (model != null)
            {
                ViewData.Model = model;
            }
            return new EnrichedViewResult<T>(viewName, ViewData);
        }

        protected virtual ActionResult AccessDeniedView()
        {
            return null;
        }

        protected virtual ActionResult ErrorView(string error = null)
        {
            return null;
        }

        protected virtual FormActionResult<T> Handle<T>(T form) where T : class
        {
            return new FormActionResult<T>(form)
            {
                UnauthorizedResult = cmd => AccessDeniedView(),
                FailureResult = cmd => cmd != null
                                           ? EnrichedView(cmd)
                                           : View(),
                SuccessResult = cmd => cmd.Form != null
                                           ? EnrichedView(cmd.Form)
                                           : View(),
                Handler = cmd => new FormHandlerResult<T> { Form = cmd }
            };
        }

        protected virtual FormActionResult<T> Handle<T>() where T : class
        {
            return new FormActionResult<T>(null)
            {
                UnauthorizedResult = cmd => AccessDeniedView(),
                FailureResult = cmd => EnrichedView(cmd),
                SuccessResult = cmd => EnrichedView(cmd.Form),
                Handler = cmd => new FormHandlerResult<T> { Form = cmd }
            };
        }

        protected virtual void LogException(Exception ex)
        {
            // TODO: Log exception
        }
    }
}
