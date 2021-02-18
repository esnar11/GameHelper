using System;
using System.Collections.Generic;
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
