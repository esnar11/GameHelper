using System.Collections.Generic;
using System.Windows;
using OpticalReader.Winds;

namespace OpticalReader
{
    public partial class MainWindow
    {
        private readonly ICaptureEngineExt _captureEngine;
        private readonly Settings _settings = new Settings();

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            _captureEngine = new CaptureEngine(_settings.CaptureAreas);
            _captureEngine.TextCaptured += TextCaptured;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _captureEngine.Start();
        }

        private void TextCaptured(CaptureArea area, IReadOnlyCollection<string> lines)
        {
            lines.Equals(null);
        }

        private void OnSettingsClick(object sender, RoutedEventArgs e)
        {
            var window = new SettingsWindow(_settings, _captureEngine);
            window.Owner = this;
            window.ShowDialog();
        }
    }
}
