using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Albion.Common;
using Albion.Network;

namespace GameHelper.Albion
{
    internal class CustomEventHandler : EventPacketHandler<CustomEvent>
    {
        private readonly EventCodes _eventCode;

        public event Action<EventCodes, Dictionary<byte, object>> OnEvent;

        public CustomEventHandler(EventCodes eventCode) : base((short)eventCode)
        {
            _eventCode = eventCode;
        }

        protected override async Task OnActionAsync(CustomEvent value)
        {
            OnEvent?.Invoke(_eventCode, value.Parameters);
        }

        internal static string ToString(object value)
        {
            if (value.GetType() == typeof(byte[]))
                return string.Join(", ", (byte[])value);

            if (value.GetType() == typeof(int[]))
                return string.Join(", ", (int[])value);

            if (value.GetType() == typeof(float[]))
                return string.Join(", ", (float[])value);

            if (value.GetType() == typeof(short[]))
                return string.Join(", ", (short[])value);

            return value.ToString();
        }
    }

    internal class CustomEvent : BaseEvent
    {
        public Dictionary<byte, object> Parameters { get; private set; }

        public CustomEvent(Dictionary<byte, object> parameters) : base(parameters)
        {
            Parameters = parameters;
        }
    }
}
