using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GameHelper.Interfaces.LowLevel
{
    public interface IBinarySearcher
    {
        IReadOnlyCollection<BinarySearchMatch> Search<T>(IReadOnlyCollection<Datagram> items, T value);
    }

    [DebuggerDisplay("{Position}")]
    public class BinarySearchMatch
    {
        public Datagram Data { get; }

        public int Position { get; }

        public int Length { get; }

        public BinarySearchMatch(Datagram data, int position, int length)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
            Position = position;
            Length = length;
        }
    }
}
