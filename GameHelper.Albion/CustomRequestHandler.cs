using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Albion.Common;
using Albion.Network;

namespace GameHelper.Albion
{
    public class CustomRequestHandler : RequestPacketHandler<CustomOperation>
    {
        private readonly OperationCodes _operationCode;

        public event Action<OperationCodes, Dictionary<byte, object>> OnEvent;

        public CustomRequestHandler(OperationCodes operationCode) : base((short)operationCode)
        {
            _operationCode = operationCode;
        }

        protected override async Task OnActionAsync(CustomOperation value)
        {
            switch (_operationCode)
            {
                //case OperationCodes.QueryGuildPlayerStats:
                //case OperationCodes.CastCancel: // координаты и поворот
                //case OperationCodes.ChannelingCancel: // атака
                //case OperationCodes.TerminateToggleSpell: // атака
                //    break;
                default:
                    if (value.Parameters.Any(p =>
                    {
                        if (!Utils.TryToInt(p.Value, out var i))
                            return false;
                        return i == 2096;
                    }))
                    {
                        Debug.WriteLine($"-------------- Operation {_operationCode} -----------------");
                        foreach (var pair in value.Parameters)
                            Debug.WriteLine(pair.Key + ": " + Utils.ToString(pair.Value));
                    }
                    break;
            }

            OnEvent?.Invoke(_operationCode, value.Parameters);
        }
    }

    public class CustomOperation : BaseOperation
    {
        public Dictionary<byte, object> Parameters { get; }

        public CustomOperation(Dictionary<byte, object> parameters) : base(parameters)
        {
            Parameters = parameters;
        }
    }
}
