using System;

namespace OpticalReader.Winds
{
    public partial class SettingsWindow
    {
        private readonly Settings _settings;
        private readonly ICaptureEngineExt _captureEngine;

        public SettingsWindow(Settings settings, ICaptureEngineExt captureEngine)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _captureEngine = captureEngine ?? throw new ArgumentNullException(nameof(captureEngine));
            InitializeComponent();
            _tuneCaptureArea.CaptureEngine = captureEngine;
            _tuneCaptureArea.Settings = settings;
        }
    }
}
