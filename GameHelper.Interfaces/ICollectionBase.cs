using System;
using System.Collections.Generic;

namespace GameHelper.Interfaces
{
    public interface ICollectionBase<T>
    {
        event Action<T> Added;

        event Action<T> Removed;

        int Count { get; }

        IReadOnlyCollection<T> Objects { get; }

        void Add(T obj);

        void Remove(T obj);

        void Clear();
    }
}
