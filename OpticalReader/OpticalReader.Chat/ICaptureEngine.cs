using System;
using System.Collections.Generic;
using System.Text;

namespace OpticalReader.Chat
{
    public interface ICaptureEngine
    {
        event Action<CaptureArea, IReadOnlyCollection<string>> TextCaptured;
    }
}
