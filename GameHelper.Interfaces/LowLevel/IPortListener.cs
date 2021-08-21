using System;

namespace GameHelper.Interfaces.LowLevel
{
    public interface IPortListener
    {
        ushort Port { get; }

        event Action<Datagram> DataCaptured;

        void Connect();
        void Disconnect();

        event Action Connected;
        event Action Disconnected;
    }
}
