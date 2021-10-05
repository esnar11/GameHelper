using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using GameHelper.Interfaces;
using GameHelper.Utils;

namespace GameHelper.UserControls
{
    public partial class ChatMessageControl
    {
        public ChatMessageControl()
        {
            InitializeComponent();
        }
    }

    public class ChatMessageConverter : IValueConverter
    {
        private static readonly SolidColorBrush HighlightBrush = new SolidColorBrush(Color.FromArgb(64, 255, 192, 0));

        public static ChatsSettings ChatsSettings { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var chatMessage = (ChatMessage)value;

            if (targetType == typeof(Brush))
            {
                if (ChatsSettings != null)
                    if (chatMessage.HasHighlightWord(ChatsSettings.HighlightWords))
                        return HighlightBrush;
                return Brushes.Transparent;
            }
            
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
