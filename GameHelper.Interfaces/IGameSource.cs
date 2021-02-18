namespace GameHelper.Interfaces
{
    public interface IGameSource
    {
        string Name { get; }

        void Connect();

        void Disconnect();

        ILowEventsSource IncomeLowEventsSource { get; }

        ILowEventsSource OutcomeLowEventsSource { get; }

        IEventsSource EventsSource { get; }
    }
}
