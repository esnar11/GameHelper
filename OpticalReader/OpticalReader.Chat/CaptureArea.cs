using System;
using System.Drawing;

namespace OpticalReader
{
    public class CaptureArea
    {
        public bool IsEnabled { get; set; } = true;

        public string Name { get; set; }

        public TimeSpan Interval { get; set; }

        public Size Size { get; set; }

        public Point Point { get; set; }
    }
}
