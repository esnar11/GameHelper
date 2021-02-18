using System;
using System.Linq;

namespace GameHelper.UserControls
{
    public class ChannelSettings
    {
        public string Channel { get; set; }

        public bool RussianOnly { get; set; }

        /// <summary>
        /// Использовать ЧС, стоп-слова и т. п.
        /// </summary>
        public bool UseBlocking { get; set; }
    }

    public class ChatsSettings
    {
        public string Game { get; set; }

        public string[] StopWords { get; set; } = new string[0];

        public string[] HighlightWords { get; set; } = new string[0];

        /// <summary>
        /// ЧС
        /// </summary>
        public string[] BlackList { get; set; } = new string[0];

        public ChannelSettings[] Channels { get; set; } = new ChannelSettings[0];

        public void Add(ChannelSettings channelSettings)
        {
            if (channelSettings == null) throw new ArgumentNullException(nameof(channelSettings));

            var list = Channels.ToList();
            list.Add(channelSettings);
            Channels = list.ToArray();
        }

        public void AddToBlackList(string sender)
        {
            if (BlackList.Any(s => s.Equals(sender, StringComparison.InvariantCultureIgnoreCase)))
                return;

            var list = BlackList.ToList();
            list.Add(sender);
            BlackList = list.OrderBy(s => s).ToArray();
        }
    }
}
