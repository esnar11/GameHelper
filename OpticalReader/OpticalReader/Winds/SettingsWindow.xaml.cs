using System;
using System.Windows;

namespace OpticalReader.Winds
{
    public partial class SettingsWindow
    {
        private readonly AppSettings _settings;

        public SettingsWindow(AppSettings settings, ICaptureEngineExt captureEngine)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            InitializeComponent();
            _tuneCaptureArea.CaptureEngine = captureEngine;
            _tuneCaptureArea.Settings = settings;

            _tbIgnore.Text = string.Join(Environment.NewLine, settings.ChatSettings.IgnoreWords);
            _tbHighlight.Text = string.Join(Environment.NewLine, settings.ChatSettings.HighlightWords);
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            _settings.ChatSettings.IgnoreWords = _tbIgnore.Text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            _settings.ChatSettings.HighlightWords = _tbHighlight.Text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            _settings.Save();
            MessageBox.Show("Done");
        }
    }
}
