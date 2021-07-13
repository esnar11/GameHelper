using System;

namespace GameHelper.Interfaces.LowLevel
{
    public interface IUDP
    {
        ushort Port { get; }

        event Action<byte[]> OnData;

        void Connect();
        void Disconnect();

        event Action Connected;
        event Action Disconnected;
    }
}
