using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noodle.Web.Mvc
{
    public interface IValidator<T>
    {
        void Validate(T item, IValidationDictionary validationDictionary);
    }

    public class DelegateValidator<T> : IValidator<T>
    {
        private readonly Func<T, bool> _validator;
        private readonly Func<T, string> _messageDelegate;
        private readonly string _message;

        public DelegateValidator(Func<T, bool> validator, string message = null)
        {
            if (validator == null)
                throw new ArgumentNullException("validator");

            this._validator = validator;
            this._message = message;
        }

        public DelegateValidator(Func<T, bool> validator, Func<T, string> messageDelegate)
        {
            if (validator == null)
                throw new ArgumentNullException("validator");

            this._validator = validator;
            this._messageDelegate = messageDelegate;
            this._message = _message;
        }

        public void Validate(T item, IValidationDictionary validationDictionary)
        {
            if (!_validator(item))
            {
                if (_messageDelegate != null)
                {
                    validationDictionary.AddError("", _messageDelegate.Invoke(item));
                }
                else
                {
                    validationDictionary.AddError("", _message ?? "The item is not valid.");
                }
            }
        }
    }
}
