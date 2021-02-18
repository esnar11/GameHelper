using System;
using System.Windows;
using System.Windows.Controls;
using GameHelper.Interfaces;

namespace GameHelper.UserControls
{
    public partial class ChatsControl
    {
        private readonly IEventsSource _eventsSource;
        private readonly ChatsSettings _chatsSettings;

        public ChatsControl()
        {
            InitializeComponent();
        }

        public ChatsControl(IEventsSource eventsSource, ChatsSettings chatsSettings) : this()
        {
            _eventsSource = eventsSource ?? throw new ArgumentNullException(nameof(eventsSource));
            _chatsSettings = chatsSettings ?? throw new ArgumentNullException(nameof(chatsSettings));

            Loaded += ChatsControl_Loaded;
            Unloaded += ChatsControl_Unloaded;

            ChatMessageConverter.ChatsSettings = chatsSettings;
        }

        private void ChatsControl_Unloaded(object sender, RoutedEventArgs e)
        {
            _eventsSource.ChatMessage -= OnChatMessage;
        }

        private void ChatsControl_Loaded(object sender, RoutedEventArgs e)
        {
            _eventsSource.ChatMessage += OnChatMessage;
        }

        private void OnChatMessage(ChatMessage message)
        {
            this.Do(() =>
            {
                foreach (TabItem tabItem in _tabControl.Items)
                    if (tabItem.Header == message.Channel)
                        return;

                var chatSettings = new ChannelSettings
                {
                    Channel = message.Channel,
                    UseBlocking = true,
                    RussianOnly = true
                };
                _chatsSettings.Add(chatSettings);
                var chatControl = new ChatControl(_eventsSource, _chatsSettings)
                {
                    Channels = new[] { message.Channel }
                };
                var newItem = new TabItem
                {
                    Header = message.Channel,
                    Content = chatControl,
                    ContextMenu = new ContextMenu()
                };

                CreateContextMenu(chatControl, newItem.ContextMenu);

                _tabControl.Items.Add(newItem);
                chatControl.OnChatMessage(message);
            });
        }

        private static void CreateContextMenu(ChatControl chatControl, ItemsControl contextMenu)
        {
            var miClear = new MenuItem { Header = "Clear", DataContext = chatControl };
            miClear.Click += OnClearClick;
            contextMenu.Items.Add(miClear);

            var miUseBlocking = new MenuItem { Header = "Use blocking", DataContext = chatControl, IsCheckable = true, IsChecked = chatControl.Settings.UseBlocking };
            miUseBlocking.Click += OnUseBlockingClick;
            contextMenu.Items.Add(miUseBlocking);

            var miRuOnly = new MenuItem { Header = "Russian only", DataContext = chatControl, IsCheckable = true, IsChecked = chatControl.Settings.RussianOnly };
            miRuOnly.Click += OnRuOnlyClick;
            contextMenu.Items.Add(miRuOnly);
        }

        private static void OnUseBlockingClick(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var chatControl = (ChatControl)menuItem.DataContext;
            chatControl.Settings.UseBlocking = menuItem.IsChecked;
        }

        private static void OnRuOnlyClick(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var chatControl = (ChatControl)menuItem.DataContext;
            chatControl.Settings.RussianOnly = menuItem.IsChecked;
        }

        private static void OnClearClick(object sender, RoutedEventArgs e)
        {
            var chatControl = (ChatControl)((FrameworkElement)sender).DataContext;
            chatControl.Clear();
        }
    }
}
