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
        /// <summary>
        /// Called when an unhandled exception occurs in the action.
        /// </summary>
        /// <param name="filterContext">Information about the current request and action.</param>
        /// <remarks></remarks>
        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception != null)
                LogException(filterContext.Exception);
            base.OnException(filterContext);
        }

        /// <summary>
        /// Models this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <remarks></remarks>
        protected virtual T Model<T>() where T : class
        {
            return DependencyResolver.Current.GetService(typeof (T)) as T;
        }

        /// <summary>
        /// Enricheds the view.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        protected virtual EnrichedViewResult<T> EnrichedView<T>(T model)
        {
            return EnrichedView(null, model);
        }

        /// <summary>
        /// Enricheds the view.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="viewName">Name of the view.</param>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        protected virtual EnrichedViewResult<T> EnrichedView<T>(string viewName, T model)
        {
            if (model != null)
            {
                ViewData.Model = model;
            }
            return new EnrichedViewResult<T>(viewName, ViewData);
        }

        /// <summary>
        /// Access denied view
        /// </summary>
        /// <returns>Access denied view</returns>
        protected virtual ActionResult AccessDeniedView()
        {
            return null;
        }

        /// <summary>
        /// Access denied view
        /// </summary>
        /// <returns>Access denied view</returns>
        protected virtual ActionResult ErrorView(string error = null)
        {
            return null;
        }

        /// <summary>
        /// Handles the specified form.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="form">The form.</param>
        /// <returns></returns>
        /// <remarks></remarks>
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

        /// <summary>
        /// Logs the exception.
        /// </summary>
        /// <param name="ex">The exception.</param>
        /// <remarks></remarks>
        protected virtual void LogException(Exception ex)
        {
            // TODO: Log exception
        }
    }
}
