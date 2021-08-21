namespace GameHelper.Interfaces.LowLevel
{
    public class Datagram
    {
        public Datagram(byte[] data, DataDirection direction)
        {
            Data = data;
            Direction = direction;
        }

        public DataDirection Direction { get; }

        public byte[] Data { get; }
    }

    public enum DataDirection
    {
        ServerToClient,
        ClientToServer
    }
}
