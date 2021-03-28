using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Albion.Common;
using Albion.Network;

namespace GameHelper.Albion
{
    internal class CustomEventHandler : EventPacketHandler<CustomEvent>
    {
        private readonly EventCodes _eventCode;

        private static int _avatarId;

        public event Action<EventCodes, Dictionary<byte, object>> OnEvent;

        public CustomEventHandler(EventCodes eventCode) : base((short)eventCode)
        {
            _eventCode = eventCode;
        }

        protected override async Task OnActionAsync(CustomEvent value)
        {
            switch (_eventCode)
            {
                //case EventCodes.CastHit:
                //case EventCodes.CastHits:
                //case EventCodes.QuestGiverDebugInfo:
                //case EventCodes.CastSpell:
                //case EventCodes.HealthUpdate:
                //case EventCodes.EnergyUpdate:
                //case EventCodes.ActionOnBuildingCancel:
                //case EventCodes.ActiveSpellEffectsUpdate:
                //case EventCodes.Attack:
                //    break;

                case EventCodes.Leave:
                    if (Utils.ToInt(value.Parameters[0]) == _avatarId)
                        _avatarId = default;
                    break;

                //case EventCodes.UpdateMatchDetails:
                //    Debug.WriteLine($"-------------- {_eventCode} -----------------");
                //    foreach (var pair in value.Parameters)
                //        Debug.WriteLine(pair.Key + ": " + ToString(pair.Value));
                //    break;

                case EventCodes.TreasureChestUsingFinished:
                    //Debug.WriteLine($"-------------- {_eventCode} -----------------");
                    //Debug.WriteLine(Utils.ToString(value.Parameters[0]));
                    _avatarId = Utils.ToInt(value.Parameters[0]);
                    break;

                //case EventCodes.PlayerBuildingInfo:
                //case EventCodes.PartyChangedOrder:
                //case EventCodes.PartyDisbanded:
                //case EventCodes.PartyJoined:
                //case EventCodes.PartyLootItems:
                //case EventCodes.PartyPlayerJoined:
                //case EventCodes.PartyPlayerLeft:
                //case EventCodes.PartyPlayerUpdated:
                //    Debug.WriteLine($"-------------- {_eventCode} -----------------");
                //    foreach (var pair in value.Parameters)
                //        Debug.WriteLine(pair.Key + ": " + ToString(pair.Value));
                //    break;
                default:
                    //if (_avatarId != default)
                        if (value.Parameters.Any(p =>
                            {
                                if (!Utils.TryToInt(p.Value, out var i))
                                    return false;
                                return i == 2096;
                            }))
                        {
                            Debug.WriteLine($"-------------- {_eventCode} -----------------");
                            foreach (var pair in value.Parameters)
                                Debug.WriteLine(pair.Key + ": " + Utils.ToString(pair.Value));
                        }
                    break;
            }

            OnEvent?.Invoke(_eventCode, value.Parameters);
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
