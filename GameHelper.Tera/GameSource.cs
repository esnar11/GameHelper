using System;
using System.Collections.Generic;
using System.Linq;
using GameHelper.Interfaces;
using GameHelper.Interfaces.LowLevel;
using GameHelper.Utils;
using SharpPcap;

namespace GameHelper.Tera
{
    public class GameSource: IGameSource
    {
        private ICaptureDevice _device;
        private readonly ICollection<IPortListener> _listeners = new List<IPortListener>();
        private readonly EventsSource _eventsSource = new EventsSource();

        public string Name => "NW";

        public Character Avatar { get; }

        public void Connect(string deviceName)
        {
            var detector = new PortDetector();
            var channels = detector.GetChannels("NW");

            _device = CaptureDeviceList.Instance.Single(d => d.Name == deviceName);
            _device.Open(DeviceMode.Promiscuous, (int)TimeSpan.FromSeconds(1).TotalMilliseconds);

            foreach (var channel in channels)
            {
                IPortListener listener = new PortListener(_device, channel.Port, channel.Protocol);
                _listeners.Add(listener);
                listener.DataCaptured += OnDataCaptured;
                listener.Connect();
            }

            _device.StartCapture();

            Connected?.Invoke();
        }

        public void Disconnect()
        {
            _device.StopCapture();

            foreach (var udp in _listeners)
            {
                udp.DataCaptured -= OnDataCaptured;
                udp.Disconnect();
            }

            _device.Close();
            Disconnected?.Invoke();
        }

        public event Action Connected;
        public event Action Disconnected;

        private void OnDataCaptured(Datagram datagram)
        {
        }

        public ILowEventsSource IncomeLowEventsSource { get; }
        
        public ILowEventsSource OutcomeLowEventsSource { get; }
        
        public IEventsSource EventsSource => _eventsSource;
        
        public IRepository<BuffInfo> BuffRepository { get; }

        public IReadOnlyCollection<IPortListener> UDPs => _listeners.ToArray();
    }
}
