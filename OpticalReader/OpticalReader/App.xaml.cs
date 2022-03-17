using System;
using System.Windows;

namespace OpticalReader
{
    public partial class App
    {
        public static void ShowError(Exception error)
        {
            var e = error.GetBaseException();
            var message = e.Message + Environment.NewLine + Environment.NewLine + e.StackTrace;
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
