using System;
using System.Collections.Generic;
using System.Windows;

namespace OpticalReader
{
    public partial class MainWindow
    {
        private readonly IReadOnlyCollection<CaptureArea> _captureAreas = new[]
        {
            new CaptureArea
            {
                Name = "NW_Chat",
                Interval = TimeSpan.FromSeconds(2),
                Point = new System.Drawing.Point(10, 200),
                Size = new System.Drawing.Size(500, 600)
            }
        };
        private readonly ICaptureEngine _captureEngine;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            _captureEngine = new CaptureEngine(_captureAreas);

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
    }
}
