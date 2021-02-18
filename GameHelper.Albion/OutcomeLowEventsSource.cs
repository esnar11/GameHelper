using System;
using System.Collections.Generic;
using Albion.Common;
using Albion.Network;
using GameHelper.Interfaces;

namespace GameHelper.Albion
{
    public class OutcomeLowEventsSource : ILowEventsSource
    {
        public OutcomeLowEventsSource(ReceiverBuilder builder)
        {
            foreach (OperationCodes code in Enum.GetValues(typeof(OperationCodes)))
                switch(code)
                {
                    case OperationCodes.Ping:
                    case OperationCodes.Unused:
                        //case OperationCodes.CastCancel:
                        //case OperationCodes.QueryGuildPlayerStats:
                        //case OperationCodes.ChannelingCancel:
                        //case OperationCodes.RatingDoRate:
                        break;
                    default:
                        var requestHandler = new CustomRequestHandler(code);
                        requestHandler.OnEvent += RequestHandler_OnEvent;
                        builder.AddRequestHandler(requestHandler);
                        break;
                }
        }

        private void RequestHandler_OnEvent(OperationCodes code, Dictionary<byte, object> dict)
        {
            Event?.Invoke((ushort)code, dict);
        }

        public event Action<ushort, IReadOnlyDictionary<byte, object>> Event;
    }
}
