using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;

namespace ChatMonitor
{
    public class CapturableWindowDetector
    {
        private static readonly string[] _ignoreProcesses =
        {
            "applicationframehost",
            "shellexperiencehost",
            "systemsettings",
            "winstore.app",
            "searchui"
        };

        public IReadOnlyCollection<CapturableWindow> FindWindows()
        {
            var wih = new WindowInteropHelper(Application.Current.MainWindow);
            var list = new List<CapturableWindow>();
            NativeMethods.EnumWindows((hWnd, lParam) =>
            {
                // ignore invisible windows
                if (!NativeMethods.IsWindowVisible(hWnd))
                    return true;

                // ignore untitled windows
                var title = new StringBuilder(1024);
                NativeMethods.GetWindowText(hWnd, title, title.Capacity);
                if (string.IsNullOrWhiteSpace(title.ToString()))
                    return true;

                // ignore me
                if (wih.Handle == hWnd)
                    return true;

                NativeMethods.GetWindowThreadProcessId(hWnd, out var processId);

                // ignore by process name
                var process = Process.GetProcessById((int)processId);
                if (_ignoreProcesses.Any(p => p == process.ProcessName.ToLower()))
                    return true;

                list.Add(new CapturableWindow
                {
                    Handle = hWnd,
                    Name = $"{title} ({process.ProcessName}.exe)"
                });

                return true;
            }, IntPtr.Zero);

            return list;
        }
    }

    public struct CapturableWindow
    {
        public string Name { get; set; }

        public IntPtr Handle { get; set; }
    }
}
