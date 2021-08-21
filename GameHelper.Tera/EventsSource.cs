using System;
using GameHelper.Interfaces;

namespace GameHelper.Tera
{
    public class EventsSource: IEventsSource
    {
        public event Action<ChatMessage> ChatMessage;

        public event Action<HealthChange> HealthChange;

        public event Action<Buff> BuffAdded;
    }
}
