using System.Windows;

namespace OpticalReader.Chat.Controls
{
    public partial class ChatControl
    {
        private IChat _chat;

        public IChat Chat
        {
            get => _chat;
            set
            {
                if (_chat == value)
                    return;

                _chat = value;
                _chat.MessagesChanged += OnMessagesChanged;
            }
        }

        private void OnMessagesChanged(System.Collections.Generic.IReadOnlyCollection<MessageWrapper> wrappers)
        {
            _lb.ItemsSource = wrappers;
        }

        public ChatControl()
        {
            InitializeComponent();
        }

        private void OnClearClick(object sender, RoutedEventArgs e)
        {
            _chat.Clear();
        }
    }
}
