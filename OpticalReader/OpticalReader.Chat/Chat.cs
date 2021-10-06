using System;
using System.Collections.Generic;
using System.Linq;

namespace OpticalReader.Chat
{
    public interface IChat
    {
        event Action<IReadOnlyCollection<MessageWrapper>> MessagesChanged;
    }

    public class Chat : IChat
    {
        public const string ChannelName_Global = "Global";
        public const string ChannelName_Group = "Group";
        public const string ChannelName_Area = "Area";
        public const string ChannelName_Guild = "Guild";
        public const string ChannelName_Recruitment = "Reqruitment";
        public const string ChannelName_Faction = "Faction";
        public const string ChannelName_System = "System";

        private readonly ChatSettings _settings;
        private readonly ICollection<Message> _messages = new List<Message>();

        public Chat(IChatParser chatParser, ChatSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            chatParser.NewMessage += ChatParser_NewMessage;
        }

        private void ChatParser_NewMessage(Message message)
        {
            _messages.Add(message);
            RaiseChanges();
        }

        private void RaiseChanges()
        {
            if (MessagesChanged != null)
            {
                var wrappers = _messages
                    .OrderByDescending(m => m.DateTime)
                    .Where(Filter)
                    .Take(25)
                    .OrderBy(m => m.DateTime)
                    .Select(CreateWrapper)
                    .ToArray();
                MessagesChanged(wrappers);
            }
        }

        private bool Filter(Message m)
        {
            if (_settings.IgnoreWords.Any(w => m.Text.Contains(w)))
                return false;

            return true;
        }

        private MessageWrapper CreateWrapper(Message m)
        {
            var wrapper = new MessageWrapper(m);

            if (NeedHighlight(m, _settings))
                wrapper.NeedHighlight = true;

            return wrapper;
        }

        internal static bool NeedHighlight(Message m, ChatSettings chatSettings)
        {
            return chatSettings.HighlightWords.Any(w => m.Text.Contains_IgnoreCase(w));
        }

        public event Action<IReadOnlyCollection<MessageWrapper>> MessagesChanged;
    }

    public class MessageWrapper
    {
        public Message Message { get; }

        public bool NeedHighlight { get; set; }

        public MessageWrapper(Message message)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }
    }
}
