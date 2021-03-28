using System;
using System.Diagnostics;

namespace GameHelper.Interfaces
{
    public interface IEventsSource
    {
        event Action<ChatMessage> ChatMessage;

        event Action<HealthChange> HealthChange;

        event Action<Buff> BuffAdded;
    }

    [DebuggerDisplay("[{Channel}] {Sender}: {Message}")]
    public class ChatMessage
    {
        public DateTime Time { get; set; }

        public string Channel { get; set; }

        public string Sender { get; set; }

        public string Message { get; set; }

        public bool AreEquals(ChatMessage message)
        {
            if (message == null)
                return false;

            if (message.Channel != Channel)
                return false;
            if (message.Sender != Sender)
                return false;
            if (message.Message != Message)
                return false;

            return true;
        }
    }

    public class HealthChange
    {
        public DateTime Time { get; set; } = DateTime.Now;

        public int SourceId { get; set; }

        public int TargetId { get; set; }

        public int SkillId { get; set; }

        public float Value { get; set; }
    }

    public class Buff
    {
        public string Name { get; set; }
        
        public DateTime BeginTime { get; set; }
        
        public DateTime EndTime { get; set; }
    }
}
