using System;
using System.Collections.Generic;
using GameHelper.Interfaces.LowLevel;

namespace GameHelper.Interfaces
{
    public interface IGameSource
    {
        string Name { get; }

        public Character Avatar { get; }

        void Connect(string deviceName);

        void Disconnect();

        event Action Connected;

        event Action Disconnected;

        ILowEventsSource IncomeLowEventsSource { get; }

        ILowEventsSource OutcomeLowEventsSource { get; }

        IEventsSource EventsSource { get; }

        IRepository<BuffInfo> BuffRepository { get; }

        IReadOnlyCollection<IPortListener> UDPs { get; }
    }
}
