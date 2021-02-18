using System;
using System.Diagnostics;

namespace GameHelper.Interfaces
{
    public interface IEventsSource
    {
        event Action<ChatMessage> ChatMessage;
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
}
