using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("OpticalReader.Chat.Tests")]

namespace OpticalReader.Chat
{
    public interface IChatParser
    {
        event Action<Message> NewMessage;
    }

    public class ChatParser : IChatParser
    {
        private readonly string _captureAreaName;

        private readonly Queue<Message> _queue = new Queue<Message>();

        public ChatParser(ICaptureEngine captureEngine, string captureAreaName)
        {
            _captureAreaName = captureAreaName;
            captureEngine.TextCaptured += CaptureEngine_TextCaptured;
        }

        private void CaptureEngine_TextCaptured(CaptureArea area, IReadOnlyCollection<string> lines)
        {
            if (area.Name != _captureAreaName)
                return;

            var messages = ParseMessages(lines);

            foreach (var msg in messages)
            {
                if (_queue.Any(m => m.AreEquals(msg)))
                    continue;

                msg.DateTime = DateTime.Now;
                _queue.Enqueue(msg);
                while (_queue.Count > 20)
                    _queue.Dequeue();
                NewMessage?.Invoke(msg);
            }
        }

        private static IReadOnlyCollection<Message> ParseMessages(IReadOnlyCollection<string> lines)
        {
            var list = new List<Message>();

            var buffer = new List<string>();
            foreach (var line in lines)
            {
                if (IsSign(line))
                {
                    if (buffer.Any())
                        list.Add(new Message(ParseAuthor(line), ParseChannelName(line), string.Join(" ", buffer)));
                    buffer.Clear();
                }
                else
                    buffer.Add(line);
            }

            return list;
        }

        internal static bool IsSign(string line)
        {
            var i = line.LastIndexOf(" ");
            if (i < 0)
                return false;
            var lastWord = line.Substring(i + 1);
            switch (lastWord)
            {
                case "GLOBAL":
                case "GROUP":
                case "AREA":
                case "COMPANY":
                case "RECRUITMENT":
                case "FACTION":
                case "SYSTEM":
                    return true;
                default:
                    return false;
            }
        }

        internal static string ParseAuthor(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return null;

            var i = line.LastIndexOf(" ");
            if (i < 0)
                return null;

            i = line.Substring(0, i).LastIndexOf(" ");
            if (i < 0)
                return null;

            return line.Substring(0, i);
        }

        private static string ParseChannelName(string line)
        {
            var i = line.LastIndexOf(" ");
            switch (line.Substring(i + 1))
            {
                case "GLOBAL":
                    return Chat.ChannelName_Global;
                case "GROUP":
                    return Chat.ChannelName_Group;
                case "AREA":
                    return Chat.ChannelName_Area;
                case "COMPANY":
                    return Chat.ChannelName_Guild;
                case "RECRUITMENT":
                    return Chat.ChannelName_Recruitment;
                case "FACTION":
                    return Chat.ChannelName_Faction;
                case "SYSTEM":
                    return Chat.ChannelName_System;
                default:
                    throw new NotImplementedException();
            }
        }

        public event Action<Message> NewMessage;
    }
}
