using System;
using System.Diagnostics;

namespace OpticalReader.Chat
{
    [DebuggerDisplay("[{Channel}] {Author}; {Text}")]
    public class Message
    {
        public string Author { get; }

        public string Channel { get; }

        public string Text { get; }

        public DateTime DateTime { get; set; }

        public Message(string author, string channel, string text)
        {
            Author = author;
            Channel = channel;
            Text = text;
        }

        public bool AreEquals(Message msg)
        {
            if (msg.Channel != Channel)
                return false;

            if (msg.Author != Author)
                return false;

            if (msg.Text != Text)
                return false;

            return msg.Text.StartsWith(Text, StringComparison.InvariantCultureIgnoreCase) || Text.StartsWith(msg.Text, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
