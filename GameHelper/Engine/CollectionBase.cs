using System;
using System.Collections.Generic;
using System.Linq;
using GameHelper.Interfaces;

namespace GameHelper.Engine
{
    public class CollectionBase<T> : ICollectionBase<T>
    {
        protected readonly ICollection<T> _objects = new List<T>();

        public event Action<T> Added;
        public event Action<T> Removed;

        public int Count
        {
            get
            {
                lock (_objects)
                    return _objects.Count;
            }
        }

        public IReadOnlyCollection<T> Objects
        {
            get
            {
                lock (_objects)
                    return _objects.ToArray();
            }
        }

        public void Add(T obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            lock (_objects)
                _objects.Add(obj);

            Added?.Invoke(obj);
        }

        public void Remove(T obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            lock (_objects)
                _objects.Remove(obj);

            Removed?.Invoke(obj);
        }

        public void Clear()
        {
            T[] objects;
            lock (_objects)
            {
                objects = _objects.ToArray();
                _objects.Clear();
            }

            foreach (var obj in objects)
                Removed?.Invoke(obj);
        }
    }
}
