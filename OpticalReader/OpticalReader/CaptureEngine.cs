using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Windows.Globalization;
using Windows.Media.Ocr;

namespace OpticalReader
{
    public interface ICaptureEngine
    {
        event Action<CaptureArea, IReadOnlyCollection<string>> TextCaptured;
    }

    public interface ICaptureEngineExt: ICaptureEngine
    {
        void Start();

        event Action<CaptureArea, BitmapImage> ImageCaptured;
    }

    public class CaptureEngine : ICaptureEngineExt
    {
        private readonly IReadOnlyCollection<CaptureArea> _captureAreas;
        private readonly DispatcherTimer _timer;
        private readonly IDictionary<CaptureArea, DateTime> _lastTimes = new Dictionary<CaptureArea, DateTime>();
        private readonly Language _language = new Language("en-US");
        private readonly OcrEngine _ocrEngine;

        public CaptureEngine(IReadOnlyCollection<CaptureArea> captureAreas)
        {
            _captureAreas = captureAreas ?? throw new ArgumentNullException(nameof(captureAreas));

            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += Timer_Tick;

            foreach (var captureArea in _captureAreas.Where(cs => cs.IsEnabled))
                _lastTimes.Add(captureArea, DateTime.MinValue);
            _ocrEngine = OcrEngine.TryCreateFromLanguage(_language);
        }

        private async void Timer_Tick(object sender, EventArgs e)
        {
            using (var screen = GetScreen(Screen.PrimaryScreen))
                foreach (var captureArea in _captureAreas.Where(cs => cs.IsEnabled))
                {
                    if (DateTime.Now - _lastTimes[captureArea] < captureArea.Interval)
                        continue;
                    _lastTimes[captureArea] = DateTime.Now;

                    using (var bmp = new Bitmap(captureArea.Size.Width, captureArea.Size.Height))
                    {
                        using (var gr = Graphics.FromImage(bmp))
                            gr.DrawImageUnscaled(screen, -captureArea.Point.X, -captureArea.Point.Y, captureArea.Size.Width, captureArea.Size.Height);

                        //bmp.Save(@"C:\_\10\06\2.jpg", ImageFormat.Jpeg);

                        using (var memory = new MemoryStream())
                        {
                            bmp.Save(memory, ImageFormat.Bmp);
                            memory.Position = 0;
                            var bmpDecoder = await Windows.Graphics.Imaging.BitmapDecoder.CreateAsync(memory.AsRandomAccessStream());
                            var softwareBmp = await bmpDecoder.GetSoftwareBitmapAsync();

                            var ocrResult = await _ocrEngine.RecognizeAsync(softwareBmp);
                            TextCaptured?.Invoke(captureArea, ocrResult.Lines.Select(ln => ln.Text).ToArray());

                            if (ImageCaptured != null)
                            {
                                memory.Position = 0;
                                var bitmapimage = new BitmapImage();
                                bitmapimage.BeginInit();
                                bitmapimage.StreamSource = memory;
                                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                                bitmapimage.EndInit();
                                ImageCaptured.Invoke(captureArea, bitmapimage);
                            }
                        }
                    }
                }
        }

        private static Bitmap GetScreen(Screen screen)
        {
            var bitmap = new Bitmap(screen.Bounds.Width, screen.Bounds.Height);
            using (var graphics = Graphics.FromImage(bitmap))
                graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
            return bitmap;
        }

        public void Start()
        {
            _timer.Start();
        }

        public event Action<CaptureArea, IReadOnlyCollection<string>> TextCaptured;
        
        public event Action<CaptureArea, BitmapImage> ImageCaptured;
    }
}
