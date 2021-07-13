using System.Collections.Generic;
using System.IO;

namespace GameHelper.Interfaces.LowLevel
{
    public interface IUdpDataStorage
    {
        void Add(byte[] data);

        uint Count { get; }

        IReadOnlyCollection<byte[]> Items { get; }

        void Clear();
        
        void Save(Stream stream);

        void Load(Stream stream);
    }
}
