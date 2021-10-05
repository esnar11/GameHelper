using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GameHelper.Interfaces;
using GameHelper.Utils;

namespace GameHelper.UserControls
{
    public partial class ChatControl
    {
        private readonly IEventsSource _eventsSource;
        private readonly ChatsSettings _chatsSettings;
        private readonly ICollection<ChatMessage> _messages = new ObservableCollection<ChatMessage>();

        public IReadOnlyCollection<ChatMessage> SelectedMessages
        {
            get
            {
                return _itemsControl.SelectedItems.OfType<ChatMessage>().ToArray();
            }
        }

        public ChatMessage SelectedMessage
        {
            get
            {
                var msgs = SelectedMessages;
                return msgs.Count == 1 ? msgs.First() : null;
            }
        }

        public ChannelSettings Settings
        {
            get
            {
                return _chatsSettings.Channels.FirstOrDefault(s => s.Channel == string.Join(";", Channels));
            }
        }

        public string[] Channels { get; set; } = new string[0];

        public ChatControl()
        {
            InitializeComponent();
        }

        public ChatControl(IEventsSource eventsSource, ChatsSettings chatsSettings) : this()
        {
            _eventsSource = eventsSource ?? throw new ArgumentNullException(nameof(eventsSource));
            _chatsSettings = chatsSettings ?? throw new ArgumentNullException(nameof(chatsSettings));

            _itemsControl.ItemsSource = _messages;

            Loaded += delegate
            {
                _eventsSource.ChatMessage += OnChatMessage;
            };
            Unloaded += (sender, e) => _eventsSource.ChatMessage -= OnChatMessage;
            GotFocus += ChatControl_GotFocus;

            TuneControls();
        }

        private void TuneControls()
        {
            _miCopy.IsEnabled = SelectedMessages.Count > 0;
            _miCopyToPM.IsEnabled = SelectedMessage != null;
        }

        private void ChatControl_GotFocus(object sender, RoutedEventArgs e)
        {
            var lastMessage = _messages.OrderBy(m => m.Time).LastOrDefault();
            if (lastMessage != null)
                _itemsControl.ScrollIntoView(lastMessage);
        }

        public void OnChatMessage(ChatMessage chatMessage)
        {
            this.Do(() =>
            {
                if (Channels.Contains(chatMessage.Channel))
                {
                    if (Settings.UseBlocking)
                    {
                        if (chatMessage.HasStopWord(_chatsSettings.StopWords))
                            return;

                        if (_chatsSettings.BlackList.Any(s => s.Equals(chatMessage.Sender, StringComparison.InvariantCultureIgnoreCase)))
                            return;
                    }

                    if (Settings.RussianOnly)
                        if (!chatMessage.CanBeRussian())
                            return;

                    // костыль. непонятно, почему двоятся сообщения (в источнике всё норм)
                    var last = _messages.OrderBy(m => m.Time).LastOrDefault();
                    if (last != null && last.AreEquals(chatMessage))
                        return;

                    _messages.Add(chatMessage);

                    _itemsControl.ScrollIntoView(chatMessage);
                }
            });
        }

        public void Clear()
        {
            _messages.Clear();
        }

        private void OnToBlackListClick(object sender, RoutedEventArgs e)
        {
            foreach (var chatMessage in SelectedMessages)
            {
                _chatsSettings.AddToBlackList(chatMessage.Sender);
                var hideMessages = _messages
                    .Where(m => m.Sender.Equals(chatMessage.Sender, StringComparison.InvariantCultureIgnoreCase))
                    .ToArray();
                foreach (var msg in hideMessages)
                    _messages.Remove(msg);
            }
        }

        private void OnCopySenderClick(object sender, RoutedEventArgs e)
        {
            var list = new List<string>();
            foreach (var chatMessage in SelectedMessages)
                if (!list.Contains(chatMessage.Sender))
                    list.Add(chatMessage.Sender);
            Clipboard.SetText(string.Join(Environment.NewLine, list.OrderBy(s => s)));
        }

        private void OnCopyMessageClick(object sender, RoutedEventArgs e)
        {
            var list = new List<string>();
            foreach (var chatMessage in SelectedMessages)
                if (!list.Contains(chatMessage.Message))
                    list.Add(chatMessage.Message);
            Clipboard.SetText(string.Join(Environment.NewLine, list));
        }

        private async void OnTranslateClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var list = new List<string>();
                foreach (var chatMessage in SelectedMessages)
                {
                    var translated = await App.TranslateService.Translate(chatMessage.Message);
                    if (!list.Contains(translated))
                        list.Add(translated);
                }

                MessageBox.Show(string.Join(Environment.NewLine, list));
            }
            catch (Exception exception)
            {
                App.ShowError(exception);
            }
        }

        private void OnChangeKeyboardLayoutClick(object sender, RoutedEventArgs e)
        {
            var list = new List<string>();
            foreach (var chatMessage in SelectedMessages)
            {
                var translated = App.ChangeKeyboardLayoutService.ToAnotherKeyboardLayout(chatMessage.Message);
                if (!list.Contains(translated))
                    list.Add(translated);
            }
            MessageBox.Show(string.Join(Environment.NewLine, list));
        }

        private void OnCopyToPMClick(object sender, RoutedEventArgs e)
        {
            if (SelectedMessage == null)
                return;
            Clipboard.SetText($"/w {SelectedMessage.Sender} ");
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TuneControls();
        }
    }
}
