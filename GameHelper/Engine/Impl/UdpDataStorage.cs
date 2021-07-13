using System.Collections.Generic;
using System.IO;
using GameHelper.Interfaces.LowLevel;

namespace GameHelper.Engine.Impl
{
    public class UdpDataStorage: IUdpDataStorage
    {
        private readonly ICollection<byte[]> _data = new List<byte[]>();

        public void Add(byte[] data)
        {
            lock (_data)
                _data.Add(data);
        }

        public uint Count
        {
            get
            {
                lock (_data)
                    return (uint)_data.Count;
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
                    writer.Write(data.Length);
                    writer.Write(data);
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
                    var count = reader.ReadInt32();
                    var data = reader.ReadBytes(count);
                    _data.Add(data);
                }
            }
        }
    }
}
