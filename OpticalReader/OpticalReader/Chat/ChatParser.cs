using System;
using System.Collections.Generic;
using System.Linq;
using OpticalReader.Chat.Model;

namespace OpticalReader.Chat
{
    public interface IChatParser
    {
        event Action<Message> NewMessage;
    }

    public class ChatParser : IChatParser
    {
        private const string GlobalChannel = "O GLOBAL";
        private const string GroupChannel = "O GROUP";
        private const string AreaChannel = "O AREA";
        private const string CompanyChannel = "O COMPANY";

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
                if (_queue.Any() && _queue.All(m => m.AreEquals(msg)))
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

        private static bool IsSign(string line)
        {
            return line.EndsWith(GlobalChannel) || line.EndsWith(GroupChannel) || line.EndsWith(AreaChannel) || line.EndsWith(CompanyChannel);
        }

        private static string ParseAuthor(string line)
        {
            var i = line.LastIndexOf(" ");
            i = line.Substring(0, i).LastIndexOf(" ");
            return line.Substring(0, i);
        }

        private static string ParseChannelName(string line)
        {
            var i = line.LastIndexOf(" ");
            return line.Substring(i + 1);
        }

        public event Action<Message> NewMessage;
    }
}
