using System;

namespace ChatMonitor.Capture
{
    public interface IWindowPicker
    {
        IntPtr PickCaptureTarget(IntPtr hWnd);
    }
}
