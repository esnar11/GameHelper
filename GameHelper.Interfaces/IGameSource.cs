using System;

namespace GameHelper.Interfaces
{
    public interface IGameSource
    {
        string Name { get; }

        public Character Avatar { get; }

        void Connect();

        void Disconnect();

        event Action Connected;

        event Action Disconnected;

        ILowEventsSource IncomeLowEventsSource { get; }

        ILowEventsSource OutcomeLowEventsSource { get; }

        IEventsSource EventsSource { get; }

        IRepository<BuffInfo> BuffRepository { get; }
    }
}
