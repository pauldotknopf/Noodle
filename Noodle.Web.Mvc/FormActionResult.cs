using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Noodle.Web.Mvc
{
    public class FormActionResult<T> : ActionResult where T : class
    {
        private readonly T _form;
        private readonly ICollection<IValidator<T>> _validators;

        /// <summary>
        /// 
        /// </summary>
        public Func<T, FormHandlerResult<T>> Handler { get; set; }
        public Func<FormContext<T>, ActionResult> SuccessResult { get; set; }
        public Func<T, ActionResult> FailureResult { get; set; }
        public Func<T, ActionResult> UnauthorizedResult { get; set; }
        public List<Func<bool>> IsAuthorized { get; set; }

        public FormActionResult(T form)
        {
            this._form = form;
            _validators = new List<IValidator<T>>();
            IsAuthorized = new List<Func<bool>>();
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var viewData = context.Controller.ViewData;

            if (IsAuthorized != null)
            {
                if (IsAuthorized.Any(x => !x()))
                {
                    var unauthorizedResult = UnauthorizedResult(_form);

                    if (unauthorizedResult == null)
                        throw new InvalidOperationException("The context has determined to be unauthorized, but 'UnauthorizedResult' hasn't been overriden to determine where the request shuld be sent to");

                    unauthorizedResult.ExecuteResult(context);

                    return;
                }
            }

            if (!IsValid(new ModelStateWrapper(viewData.ModelState)))
            {
                // failed
                FailureResult(_form).ExecuteResult(context);
            }
            else
            {
                // execute handler
                var result = ExecuteHandler(_form, Handler);

                // if errors were added to the model state, executed the failure result.
                if (!viewData.ModelState.IsValid)
                {
                    // failure
                    FailureResult(_form).ExecuteResult(context);
                }
                else
                {
                    // success
                    SuccessResult(result).ExecuteResult(context);
                }
            }
        }

        /// <summary>
        /// Adds a validator
        /// </summary>
        /// <param name="validator"></param>
        public void AddValidator(IValidator<T> validator)
        {
            this._validators.Add(validator);
        }

        /// <summary>
        /// Validates the mode with the model state
        /// </summary>
        /// <param name="modelState"></param>
        /// <returns></returns>
        protected bool IsValid(IValidationDictionary modelState)
        {
            if (modelState.IsValid)
            {
                foreach (var validator in _validators)
                {
                    validator.Validate(_form, modelState);
                }

                return modelState.IsValid;
            }

            return false;
        }

        /// <summary>
        /// Handles a form with a given handler and supports a return result (for later usage by success handlers).
        /// </summary>
        /// <param name="form"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        protected static FormContext<T> ExecuteHandler(T form, Func<T, FormHandlerResult<T>> handler)
        {
            var result = handler(form);
            return new FormContext<T>(result.Form, result.Result);
        }
    }

    public class FormContext<T>
    {
        public FormContext(T form, object result)
        {
            Form = form;
            Result = result;
        }

        public T Form { get; private set; }
        public object Result { get; private set; }
    }

    public static class HandleExtensions
    {
        public static FormActionResult<T> HandleWith<T>(this FormActionResult<T> result, Func<T> handler) where T : class
        {
            result.Handler = (form) => new FormHandlerResult<T>
            {
                Form = handler()
            };
            return result;
        }

        public static FormActionResult<T> HandleWith<T>(this FormActionResult<T> result, Action<T> handler) where T : class
        {
            result.Handler = form =>
            {
                handler(form);
                return new FormHandlerResult<T>
                {
                    Form = form
                };
            };
            return result;
        }

        public static FormActionResult<T> HandleWith<T>(this FormActionResult<T> result, Func<T, object> handler) where T : class
        {
            result.Handler = form =>
            {
                var handlerResult = handler(form);
                return new FormHandlerResult<T>
                {
                    Form = form,
                    Result = handlerResult
                };
                ;
            };
            return result;
        }

        public static FormActionResult<T> OnSuccess<T>(this FormActionResult<T> result, Func<FormContext<T>, ActionResult> successResult) where T : class
        {
            result.SuccessResult = successResult;
            return result;
        }

        public static FormActionResult<T> OnFailure<T>(this FormActionResult<T> result, Func<T, ActionResult> failureResult) where T : class
        {
            result.FailureResult = failureResult;
            return result;
        }

        public static FormActionResult<T> ValidateWith<T>(this FormActionResult<T> result, IValidator<T> validator) where T : class
        {
            result.AddValidator(validator);
            return result;
        }

        public static FormActionResult<T> ValidateWith<T>(this FormActionResult<T> result, Func<T, bool> validator, string message = null) where T : class
        {
            result.AddValidator(new DelegateValidator<T>(validator, message));
            return result;
        }

        public static FormActionResult<T> ValidateWith<T>(this FormActionResult<T> result, Func<T, bool> validator, Func<T, string> message) where T : class
        {
            result.AddValidator(new DelegateValidator<T>(validator, message));
            return result;
        }
    }

    public class FormHandlerResult<T>
    {
        public T Form { get; set; }
        public object Result { get; set; }
    }
}
