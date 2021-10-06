using System;
using System.Collections.Generic;
using OpticalReader.Chat;

namespace OpticalReader
{
    public class Settings
    {
        private readonly IReadOnlyCollection<CaptureArea> _captureAreas = new []
        {
            new CaptureArea
            {
                Name = "NW_Chat",
                Interval = TimeSpan.FromSeconds(2),
                Point = new System.Drawing.Point(20, 750),
                Size = new System.Drawing.Size(500, 300)
            }
        };

        public IReadOnlyCollection<CaptureArea> CaptureAreas => _captureAreas;

        public ChatSettings ChatSettings { get; } = new ChatSettings();
    }
}
