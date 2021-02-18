using System;
using System.Collections.Generic;
using Albion.Common;
using Albion.Network;
using GameHelper.Interfaces;

namespace GameHelper.Albion
{
    public class IncomeLowEventsSource: ILowEventsSource
    {
        public IncomeLowEventsSource(ReceiverBuilder builder)
        {
            foreach (EventCodes code in Enum.GetValues(typeof(EventCodes)))
                switch (code)
            {
                    case EventCodes.Move:
                        break;
                    case EventCodes.ChatMessage:
                    case EventCodes.ChatSay:
                    case EventCodes.ChatWhisper:
                    case EventCodes.SystemMessage:
                    case EventCodes.UtilityTextMessage:
                        var eventHandler = new CustomEventHandler(code);
                        eventHandler.OnEvent += EventHandler_OnEvent;
                        builder.AddEventHandler(eventHandler);
                        break;
                    default:
                        //var eventHandler = new CustomEventHandler(code);
                        //eventHandler.OnEvent += EventHandler_OnEvent;
                        //builder.AddEventHandler(eventHandler);
                        break;
            }
        }

        private void EventHandler_OnEvent(EventCodes code, Dictionary<byte, object> dict)
        {
            Event?.Invoke((ushort)code, dict);
        }

        public event Action<ushort, IReadOnlyDictionary<byte, object>> Event;
    }
}
