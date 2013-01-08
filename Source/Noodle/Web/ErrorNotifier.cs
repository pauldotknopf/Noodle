using System;

namespace Noodle.Web
{
    public class ErrorNotifier : IErrorNotifier
    {
        #region IErrorNotifier Members

        public void Notify(Exception ex)
        {
            if (ErrorOccured != null)
                ErrorOccured(this, new ErrorEventArgs{Error = ex});
        }

        public event EventHandler<ErrorEventArgs> ErrorOccured;

        #endregion
    }
}
