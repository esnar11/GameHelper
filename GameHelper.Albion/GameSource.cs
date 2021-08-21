using System;
using System.Collections.Generic;
using System.Linq;
using Albion.Network;
using GameHelper.Interfaces;
using GameHelper.Interfaces.LowLevel;
using GameHelper.Utils;
using SharpPcap;

namespace GameHelper.Albion
{
    public class GameSource: IGameSource
    {
        private IPhotonReceiver _receiver;
        private ICaptureDevice _device;
        private readonly ICollection<ushort> _udpPorts = new List<ushort>();
        private readonly ICollection<IPortListener> _udps = new List<IPortListener>();

        public string Name => "Albion";
        
        public Character Avatar { get; } = new Character();

        public void Connect(string deviceName)
        {
            var detector = new PortDetector();
            var ports = detector.GetUdpPorts("Albion-Online");
            foreach (var port in ports)
                _udpPorts.Add(port);

            var builder = ReceiverBuilder.Create();

            IncomeLowEventsSource = new IncomeLowEventsSource(builder);
            OutcomeLowEventsSource = new OutcomeLowEventsSource(builder);

            _receiver = builder.Build();

            _device = CaptureDeviceList.Instance.Single(d => d.Name == deviceName);
            _device.Open(DeviceMode.Promiscuous, (int)TimeSpan.FromSeconds(1).TotalMilliseconds);

            foreach (var udpPort in _udpPorts)
            {
                IPortListener portListener = new PortListener(_device, udpPort, ChannelProtocol.UDP);
                _udps.Add(portListener);
                portListener.DataCaptured += Udp_OnData;
                portListener.Connect();
            }

            _device.StartCapture();

            EventsSource = new EventsSource(IncomeLowEventsSource, BuffRepository, Avatar);
            Connected?.Invoke();
        }

        public void Disconnect()
        {
            _device.StopCapture();

            foreach (var udp in _udps)
            {
                udp.DataCaptured -= Udp_OnData;
                udp.Disconnect();
            }

            _device.Close();
            Disconnected?.Invoke();
        }

        private void Udp_OnData(Datagram datagram)
        {
            _receiver.ReceivePacket(datagram.Data);
        }

        public event Action Connected;
        public event Action Disconnected;

        public ILowEventsSource IncomeLowEventsSource { get; private set; }

        public ILowEventsSource OutcomeLowEventsSource { get; private set; }

        public IEventsSource EventsSource { get; private set; }
        
        public IRepository<BuffInfo> BuffRepository => new BuffRepository();

        public IReadOnlyCollection<IPortListener> UDPs => _udps.ToArray();
    }
}
