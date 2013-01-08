using System;

namespace Noodle.Web
{
    public interface IErrorNotifier
    {
        void Notify(Exception ex);
        event EventHandler<ErrorEventArgs> ErrorOccured;
    }
}
