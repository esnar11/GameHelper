using System;
using System.Linq;
using GameHelper.Interfaces.LowLevel;
using PacketDotNet;
using SharpPcap;

namespace GameHelper.Utils
{
    public class UDP: IUDP
    {
        private readonly ICaptureDevice _device;

        public UDP(ICaptureDevice device, ushort port)
        {
            _device = device ?? throw new ArgumentNullException(nameof(device));
            Port = port;
        }

        public ushort Port { get; }

        public event Action<byte[]> OnData;
        
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
            var packet = Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data).Extract<UdpPacket>();
            if (packet != null)
                if (packet.SourcePort == Port || packet.DestinationPort == Port)
                    OnData?.Invoke(packet.PayloadData);
        }
    }
}
