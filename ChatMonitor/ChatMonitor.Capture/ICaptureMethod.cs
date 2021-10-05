using System;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace ChatMonitor.Capture
{
    public interface ICaptureMethod : IDisposable
    {
        bool IsCapturing { get; }

        void StartCapture(IntPtr hWnd, SharpDX.Direct3D11.Device device, Factory factory);

        Texture2D TryGetNextFrameAsTexture2D(SharpDX.Direct3D11.Device device);

        void StopCapture();
    }
}
