using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using GameHelper.Interfaces;

namespace GameHelper.Windows
{
    public partial class ChatHistoryWindow
    {
        public IChatHistoryReadonly ChatHistory => _cbGame.SelectedItem as IChatHistoryReadonly;

        public ChatHistoryWindow()
        {
            InitializeComponent();
        }

        public ChatHistoryWindow(IReadOnlyCollection<IChatHistoryReadonly> chatHistoryProviders): this()
        {
            _cbGame.ItemsSource = chatHistoryProviders;
            if (chatHistoryProviders.Count == 1)
                _cbGame.SelectedItem = chatHistoryProviders.First();
        }

        private void _cbGame_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _cbChannel.ItemsSource = ChatHistory.SearchChannel("");
        }
    }
}
