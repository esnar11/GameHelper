using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Albion.Network;
using GameHelper.Interfaces;
using PacketDotNet;
using SharpPcap;

namespace GameHelper.Albion
{
    public class GameSource: IGameSource
    {
        private IPhotonReceiver _receiver;
        private readonly ICollection<ICaptureDevice> _devices = new List<ICaptureDevice>();

        private static readonly ushort[] _serverTcpPorts = { 64766, 64767 };
        private static readonly ushort[] _serverUdpPorts = { 4535, 5055, 5056 };

        public string Name => "Albion";
        
        public void Connect()
        {
            var builder = ReceiverBuilder.Create();

            IncomeLowEventsSource = new IncomeLowEventsSource(builder);
            OutcomeLowEventsSource = new OutcomeLowEventsSource(builder);

            _receiver = builder.Build();

            foreach (var device in CaptureDeviceList.Instance)
                if (!device.Description.Contains("Check Point"))
                    Task.Run(() =>
                    {
                        device.OnPacketArrival += PacketHandler;
                        device.Open(DeviceMode.Promiscuous, (int)TimeSpan.FromSeconds(1).TotalMilliseconds);
                        device.StartCapture();
                        _devices.Add(device);
                    });

            EventsSource = new EventsSource(IncomeLowEventsSource);
        }

        public void Disconnect()
        {
            foreach (var device in _devices)
            {
                device.OnPacketArrival -= PacketHandler;
                device.StopCapture();
                device.Close();
            }
        }

        public ILowEventsSource IncomeLowEventsSource { get; private set; }

        public ILowEventsSource OutcomeLowEventsSource { get; private set; }

        public IEventsSource EventsSource { get; private set; }

        private void PacketHandler(object sender, CaptureEventArgs e)
        {
            try
            {
                var packet = Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data).Extract<UdpPacket>();
                if (packet != null)
                    if (_serverUdpPorts.Contains(packet.SourcePort) || _serverUdpPorts.Contains(packet.DestinationPort))
                        _receiver.ReceivePacket(packet.PayloadData);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                throw;
            }
        }
    }
}
