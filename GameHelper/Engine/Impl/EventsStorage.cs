using System.Collections.Generic;

namespace GameHelper.Engine.Impl
{
    public class EventsStorage: IEventsStorage
    {
        private readonly IDictionary<ushort, IReadOnlyDictionary<byte, object>> _events = new Dictionary<ushort, IReadOnlyDictionary<byte, object>>();

        public void Add(ushort code, IReadOnlyDictionary<byte, object> parameters)
        {
            lock(_events)
                _events.Add(code, parameters);
        }

        public uint Count
        {
            get
            {
                lock (_events)
                    return (uint) _events.Count;
            }
        }

        public void Clear()
        {
            lock (_events)
                _events.Clear();
        }
    }
}
