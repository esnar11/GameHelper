using System;
using System.Collections.Generic;

namespace GameHelper.Interfaces.LowLevel
{
    public interface IBinarySearcher
    {
        IReadOnlyCollection<BinarySearchMatch> Search<T>(IReadOnlyCollection<byte[]> items, T value);
    }

    public class BinarySearchMatch
    {
        public byte[] Data { get; }

        public uint Position { get; }

        public BinarySearchMatch(byte[] data, uint position)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
            Position = position;
        }
    }
}
