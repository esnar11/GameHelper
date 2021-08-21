using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameHelper.Interfaces.LowLevel;

namespace GameHelper.Engine.Impl
{
    public class UdpDataStorage: IUdpDataStorage
    {
        private readonly ICollection<Datagram> _data = new List<Datagram>();

        public void Add(Datagram datagram)
        {
            lock (_data)
                _data.Add(datagram);
        }

        public uint Count
        {
            get
            {
                lock (_data)
                    return (uint)_data.Count;
            }
        }

        public IReadOnlyCollection<Datagram> Items
        {
            get
            {
                lock (_data)
                    return _data.ToArray();
            }
        }

        public void Clear()
        {
            lock (_data)
                _data.Clear();
        }

        public void Save(Stream stream)
        {
            using var writer = new BinaryWriter(stream);
            lock (_data)
                foreach (var data in _data)
                {
                    writer.Write((byte)data.Direction);
                    writer.Write(data.Data.Length);
                    writer.Write(data.Data);
                }
        }

        public void Load(Stream stream)
        {
            lock (_data)
            {
                _data.Clear();
                using var reader = new BinaryReader(stream);
                while (stream.Position < stream.Length - 1)
                {
                    var direction = (DataDirection)reader.ReadByte();
                    var count = reader.ReadInt32();
                    var data = reader.ReadBytes(count);
                    _data.Add(new Datagram(data, direction));
                }
            }
        }
    }
}
