using System.Collections.Generic;

namespace GameHelper.Engine
{
    public interface IEventsStorage
    {
        void Add(ushort code, IReadOnlyDictionary<byte, object> parameters);

        uint Count { get; }

        void Clear();
    }
}
