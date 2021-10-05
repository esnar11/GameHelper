using System;

namespace ChatMonitor.Capture
{
    public class GraphicsCapture : ICaptureMethod
    {
        private readonly IWindowPicker _windowPicker;
        private Windows.Graphics.Capture.Direct3D11CaptureFramePool _captureFramePool;
        private Windows.Graphics.Capture.GraphicsCaptureItem _captureItem;
        private Windows.Graphics.Capture.GraphicsCaptureSession _captureSession;

        public GraphicsCapture(IWindowPicker windowPicker)
        {
            _windowPicker = windowPicker ?? throw new ArgumentNullException(nameof(windowPicker));
            IsCapturing = false;
        }

        public bool IsCapturing { get; private set; }

        public void Dispose()
        {
            StopCapture();
        }

        public void StartCapture(IntPtr hWnd, SharpDX.Direct3D11.Device device, SharpDX.DXGI.Factory factory)
        {
            #region GraphicsCapturePicker version

            /*
            var capturePicker = new GraphicsCapturePicker();

            // ReSharper disable once PossibleInvalidCastException
            // ReSharper disable once SuspiciousTypeConversion.Global
            var initializer = (IInitializeWithWindow)(object)capturePicker;
            initializer.Initialize(hWnd);

            _captureItem = capturePicker.PickSingleItemAsync().AsTask().Result;
            */

            #endregion

            #region Window Handle version

            var captureHandle = _windowPicker.PickCaptureTarget(hWnd);
            if (captureHandle == IntPtr.Zero)
                return;

            _captureItem = CreateItemForWindow(captureHandle);

            #endregion

            if (_captureItem == null)
                return;

            _captureItem.Closed += CaptureItemOnClosed;

            var hr = ChatMonitor.Capture.Interop.NativeMethods.CreateDirect3D11DeviceFromDXGIDevice(device.NativePointer, out var pUnknown);
            if (hr != 0)
            {
                StopCapture();
                return;
            }

            var winrtDevice = (Windows.Graphics.DirectX.Direct3D11.IDirect3DDevice)System.Runtime.InteropServices.Marshal.GetObjectForIUnknown(pUnknown);
            System.Runtime.InteropServices.Marshal.Release(pUnknown);

            _captureFramePool = Windows.Graphics.Capture.Direct3D11CaptureFramePool.Create(winrtDevice, Windows.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized, 2, _captureItem.Size);
            _captureSession = _captureFramePool.CreateCaptureSession(_captureItem);
            _captureSession.StartCapture();
            IsCapturing = true;
        }

        public SharpDX.Direct3D11.Texture2D TryGetNextFrameAsTexture2D(SharpDX.Direct3D11.Device device)
        {
            using var frame = _captureFramePool?.TryGetNextFrame();
            if (frame == null)
                return null;

            // ReSharper disable once SuspiciousTypeConversion.Global
            var surfaceDxgiInterfaceAccess = (ChatMonitor.Capture.Interop.IDirect3DDxgiInterfaceAccess) frame.Surface;
            var pResource = surfaceDxgiInterfaceAccess.GetInterface(new Guid("dc8e63f3-d12b-4952-b47b-5e45026a862d"));

            using var surfaceTexture = new SharpDX.Direct3D11.Texture2D(pResource); // shared resource
            var texture2dDescription = new SharpDX.Direct3D11.Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = SharpDX.Direct3D11.BindFlags.ShaderResource | SharpDX.Direct3D11.BindFlags.RenderTarget,
                CpuAccessFlags = SharpDX.Direct3D11.CpuAccessFlags.None,
                Format = SharpDX.DXGI.Format.B8G8R8A8_UNorm,
                Height = surfaceTexture.Description.Height,
                MipLevels = 1,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                Usage = SharpDX.Direct3D11.ResourceUsage.Default,
                Width = surfaceTexture.Description.Width
            };
            var texture2d = new SharpDX.Direct3D11.Texture2D(device, texture2dDescription);
            device.ImmediateContext.CopyResource(surfaceTexture, texture2d);

            return texture2d;
        }

        public void StopCapture() // ...or release resources
        {
            _captureSession?.Dispose();
            _captureFramePool?.Dispose();
            _captureSession = null;
            _captureFramePool = null;
            _captureItem = null;
            IsCapturing = false;
        }

        // ReSharper disable once SuspiciousTypeConversion.Global
        private static Windows.Graphics.Capture.GraphicsCaptureItem CreateItemForWindow(IntPtr hWnd)
        {
            var factory = System.Runtime.InteropServices.WindowsRuntime.WindowsRuntimeMarshal.GetActivationFactory(typeof(Windows.Graphics.Capture.GraphicsCaptureItem));
            var interop = (ChatMonitor.Capture.Interop.IGraphicsCaptureItemInterop) factory;
            var pointer = interop.CreateForWindow(hWnd, typeof(Windows.Graphics.Capture.GraphicsCaptureItem).GetInterface("IGraphicsCaptureItem").GUID);
            var capture = System.Runtime.InteropServices.Marshal.GetObjectForIUnknown(pointer) as Windows.Graphics.Capture.GraphicsCaptureItem;
            System.Runtime.InteropServices.Marshal.Release(pointer);

            return capture;
        }

        private void CaptureItemOnClosed(Windows.Graphics.Capture.GraphicsCaptureItem sender, object args)
        {
            StopCapture();
        }
    }
}