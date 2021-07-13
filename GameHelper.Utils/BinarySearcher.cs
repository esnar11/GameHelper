using System;
using System.Collections.Generic;
using GameHelper.Interfaces.LowLevel;

namespace GameHelper.Utils
{
    public class BinarySearcher: IBinarySearcher
    {
        public IReadOnlyCollection<BinarySearchMatch> Search<T>(IReadOnlyCollection<byte[]> items, T value)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            throw new NotImplementedException();
        }
    }
}
