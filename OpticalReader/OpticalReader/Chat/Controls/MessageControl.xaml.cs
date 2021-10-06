using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace OpticalReader.Chat.Controls
{
    public partial class MessageControl
    {
        public MessageControl()
        {
            InitializeComponent();
        }
    }

    public class MessageForegroundConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MessageWrapper wrapper)
                switch (wrapper.Message.Channel)
                {
                    case Chat.ChannelName_Global:
                        return Brushes.LightYellow;
                    case Chat.ChannelName_Guild:
                        return Brushes.DarkGreen;
                    case Chat.ChannelName_Area:
                        return Brushes.Chocolate;
                    case Chat.ChannelName_Group:
                        return Brushes.DodgerBlue;
                    case Chat.ChannelName_Recruitment:
                        return Brushes.LightYellow;
                    case Chat.ChannelName_Faction:
                        return Brushes.LightGreen;
                    case Chat.ChannelName_System:
                        return Brushes.DarkRed;
                    default:
                        throw new NotImplementedException();
                }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MessageBackgroundConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MessageWrapper wrapper)
                return wrapper.NeedHighlight ? Brushes.Yellow : Brushes.Transparent;

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
