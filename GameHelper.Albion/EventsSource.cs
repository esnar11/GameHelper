using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Albion.Common;
using GameHelper.Interfaces;

[assembly:InternalsVisibleTo("GameHelper.Tests")]

namespace GameHelper.Albion
{
    internal class EventsSource: IEventsSource
    {
        private const int Channel_Group = 63234;

        private static readonly IReadOnlyDictionary<int, string> Channels = new Dictionary<int, string>
        {
            { 11, "Торг" },
            { 12, "Рекруты" },
            { 13, "LFG" },
            { 14, "Справка" },
            { 6, "Мир" },
            { 122, "Местные" },
            { Channel_Group, "Группа" },
            { 1383, "Ги" },
        };

        private ChatMessage _prevMessage;

        public EventsSource(ILowEventsSource lowEventsSource)
        {
            lowEventsSource.Event += LowEventsSource_Event;
        }

        private void LowEventsSource_Event(ushort code, IReadOnlyDictionary<byte, object> parameters)
        {
            try
            {
                switch ((EventCodes)code)
                {
                    case EventCodes.ChatMessage:
                    case EventCodes.ChatSay:
                    case EventCodes.ChatWhisper:
                    case EventCodes.SystemMessage:
                    case EventCodes.UtilityTextMessage:
                        if (ChatMessage == null)
                            break;
                        if (parameters.TryGetValue(0, out var channel))
                            if (parameters.TryGetValue(1, out var p1))
                                if (parameters.TryGetValue(2, out var p2))
                                {
                                    var channelNumber = ToInt(channel);
                                    if (channelNumber > 16000)
                                        channelNumber = Channel_Group;
                                    if (Channels.ContainsKey(channelNumber))
                                    {
                                        if (!(p1 is string))
                                            break;
                                        var sender = (string)p1;

                                        if (!(p2 is string))
                                            break;
                                        var message = (string)p2;

                                        var chatMessage = new ChatMessage
                                        {
                                            Time = DateTime.Now,
                                            Channel = Channels[channelNumber],
                                            Sender = sender,
                                            Message = Clean(message)
                                        };
                                        if (_prevMessage != null && _prevMessage.AreEquals(chatMessage))
                                            break;
                                        ChatMessage?.Invoke(chatMessage);
                                        _prevMessage = chatMessage;
                                        break;
                                    }
                                }
                        Debug.WriteLine("------------------------------");
                        foreach (var pair in parameters)
                            Debug.WriteLine($"{pair.Key}; {CustomEventHandler.ToString(pair.Value)}");

                        break;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }
        }

        internal static string Clean(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            while (value.Contains("  "))
                value = value.Replace("  ", " ");

            while (value.Contains('￿'))
            {
                var i1 = value.IndexOf('￿');
                var i2 = value.IndexOf('￿', i1 + 1);
                value = value.Substring(0, i1) + value.Substring(i2 + 1, value.Length - i2 - 1);
            }

            return value.Trim();
        }

        public event Action<ChatMessage> ChatMessage;

        internal static int ToInt(object obj)
        {
            if (obj is int i)
                return i;

            if (obj is short s)
                return s;

            if (obj is byte b)
                return b;

            throw new NotImplementedException();
        }
    }
}
