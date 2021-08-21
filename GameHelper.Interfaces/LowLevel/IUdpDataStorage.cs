using System.Collections.Generic;
using System.IO;

namespace GameHelper.Interfaces.LowLevel
{
    public interface IUdpDataStorage
    {
        void Add(Datagram datagram);

        uint Count { get; }

        IReadOnlyCollection<Datagram> Items { get; }

        void Clear();
        
        void Save(Stream stream);

        void Load(Stream stream);
    }
}
