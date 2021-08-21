namespace GameHelper.Interfaces.LowLevel
{
    public class Channel
    {
        public Channel(ushort port, ChannelProtocol protocol)
        {
            Port = port;
            Protocol = protocol;
        }

        public ushort Port { get; }

        public ChannelProtocol Protocol { get; }
    }

    public enum ChannelProtocol
    {
        UDP,
        TCP
    }
}
