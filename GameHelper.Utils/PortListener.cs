using System;
using GameHelper.Interfaces.LowLevel;
using PacketDotNet;
using SharpPcap;

namespace GameHelper.Utils
{
    public class PortListener: IPortListener
    {
        private readonly ICaptureDevice _device;
        private readonly ChannelProtocol _protocol;

        public PortListener(ICaptureDevice device, ushort port, ChannelProtocol protocol)
        {
            _device = device ?? throw new ArgumentNullException(nameof(device));
            _protocol = protocol;
            Port = port;
        }

        public ushort Port { get; }

        public event Action<Datagram> DataCaptured;
        
        public void Connect()
        {
            _device.OnPacketArrival += PacketHandler;

            Connected?.Invoke();
        }

        public void Disconnect()
        {
            _device.OnPacketArrival -= PacketHandler;
            _device.StopCapture();
            _device.Close();

            Disconnected?.Invoke();
        }

        public event Action Connected;
        public event Action Disconnected;

        private void PacketHandler(object sender, CaptureEventArgs e)
        {
            var packet = Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);
            if (packet == null)
                return;

            if (_protocol == ChannelProtocol.UDP)
            {
                var udpPacket = packet.Extract<UdpPacket>();
                if (udpPacket != null && udpPacket.PayloadData.Length > 0)
                    if (udpPacket.SourcePort == Port || udpPacket.DestinationPort == Port)
                    {
                        var direction = Port == udpPacket.SourcePort ? DataDirection.ClientToServer : DataDirection.ServerToClient;
                        DataCaptured?.Invoke(new Datagram(udpPacket.PayloadData, direction));
                    }
            }
            else
            {
                var tcpPacket = packet.Extract<TcpPacket>();
                if (tcpPacket != null && tcpPacket.PayloadData.Length > 0)
                    if (tcpPacket.SourcePort == Port || tcpPacket.DestinationPort == Port)
                    {
                        var direction = Port == tcpPacket.SourcePort ? DataDirection.ClientToServer : DataDirection.ServerToClient;
                        DataCaptured?.Invoke(new Datagram(tcpPacket.PayloadData, direction));
                    }
            }
        }

        public override string ToString()
        {
            return "PortListener port " + Port;
        }
    }
}
