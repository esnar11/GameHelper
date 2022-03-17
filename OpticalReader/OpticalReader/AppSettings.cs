using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using OpticalReader.Chat;

namespace OpticalReader
{
    public class AppSettings
    {
        public IReadOnlyCollection<CaptureArea> CaptureAreas { get; set; } = new []
        {
            new CaptureArea
            {
                Name = "NW_Chat",
                Interval = TimeSpan.FromSeconds(2),
                Point = new System.Drawing.Point(20, 750),
                Size = new System.Drawing.Size(500, 300)
            }
        };

        public ChatSettings ChatSettings { get; set; } = new ChatSettings();

        public static AppSettings Load()
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.AppSettings))
                return new AppSettings();

            return JsonConvert.DeserializeObject<AppSettings>(Properties.Settings.Default.AppSettings);
        }

        public void Save()
        {
            Properties.Settings.Default.AppSettings = JsonConvert.SerializeObject(this, Formatting.Indented);
            Properties.Settings.Default.Save();
        }
    }
}
