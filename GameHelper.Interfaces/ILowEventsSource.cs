using System;
using System.Collections.Generic;

namespace GameHelper.Interfaces
{
    public interface ILowEventsSource
    {
        event Action<ushort, IReadOnlyDictionary<byte, object>> Event;
    }
}
