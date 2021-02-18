using System;
using System.Windows.Threading;

namespace GameHelper
{
    public static class DispatcherObjectExtensions
    {
        public static void Do(this DispatcherObject dispatcherObject, Action action)
        {
            if (dispatcherObject.Dispatcher.CheckAccess())
                action();
            else
                dispatcherObject.Dispatcher.Invoke(action);
        }
    }
}
