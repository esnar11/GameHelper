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
        private readonly IRepository<BuffInfo> _buffRepository;
        private readonly Character _avatar;
        private const int Channel_Group = 63234;
        private const string AvatarName = "Infernalis";
        private readonly IDictionary<string, int> _characterIds = new Dictionary<string, int>();
        private int _avatarId;

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

        public EventsSource(ILowEventsSource lowEventsSource, IRepository<BuffInfo> buffRepository, Character avatar)
        {
            _buffRepository = buffRepository ?? throw new ArgumentNullException(nameof(buffRepository));
            _avatar = avatar ?? throw new ArgumentNullException(nameof(avatar));
            lowEventsSource.Event += LowEventsSource_Event;
        }

        private void LowEventsSource_Event(ushort code, IReadOnlyDictionary<byte, object> parameters)
        {
            try
            {
                switch ((EventCodes)code)
                {
                    case EventCodes.UpdateMatchDetails:
                        break;

                    case EventCodes.TreasureChestUsingFinished:
                        if ((string)parameters[2] == AvatarName)
                            _avatarId = Utils.ToInt(parameters[0]);
                        break;

                    case EventCodes.Leave:
                        if (Utils.ToInt(parameters[0]) == _avatarId)
                            _avatarId = default;
                        break;

                    case EventCodes.ActiveSpellEffectsUpdate:
                        if (BuffAdded != null)
                        {
                            if (Utils.ToInt(parameters[0]) != _avatarId)
                                break;

                            if (!(parameters[3] is long[]))
                                break;

                            var longs = (long[])parameters[3];
                            var serverTime = DateTime.FromBinary(longs[^1]);
                            var clientTime = DateTime.UtcNow;
                            var delta = clientTime - serverTime;

                            if (parameters.ContainsKey(7))
                            {
                                double dur = 0;
                                if (parameters[7] is short)
                                    dur = (short)parameters[7];
                                if (parameters[7] is short[])
                                    dur = ((short[])parameters[7])[0];
                                if (parameters[7] is int[])
                                    dur = ((int[])parameters[7])[0];
                                var duration = TimeSpan.FromMilliseconds(dur);

                                if (duration < TimeSpan.FromMinutes(1))
                                {
                                    var buffTypeId = ((short[])parameters[1])[^1];
                                    var buffInfo = _buffRepository.GetById(buffTypeId);
                                    if (!buffInfo.IsInvisible)
                                    {
                                        var buff = new Buff
                                        {
                                            Name = buffInfo.Name,
                                            BeginTime = serverTime,
                                            EndTime = clientTime + duration + delta
                                        };
                                        BuffAdded?.Invoke(buff);
                                    }
                                }
                            }
                        }
                        break;
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
                                    var channelNumber = Utils.ToInt(channel);
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
                                    }
                                }
                        break;

                    case EventCodes.HealthUpdate:
                        if (HealthChange != null)
                        {
                            var healthChange = new HealthChange { TargetId = Utils.ToInt(parameters[0]) };
                            if (parameters.TryGetValue(6, out var sourceId))
                                healthChange.SourceId = Utils.ToInt(sourceId);
                            if (parameters.TryGetValue(2, out var changeHP))
                                healthChange.Value = (float)changeHP;
                            if (parameters.TryGetValue(7, out var skillId))
                                healthChange.SkillId = Utils.ToInt(skillId);
                            HealthChange?.Invoke(healthChange);
                        }

                        if (_avatarId == Utils.ToInt(parameters[0]))
                        {
                            var cur = Utils.ToInt(parameters[3]);
                            if (_avatar.HP.Max < cur)
                                _avatar.HP.Max = cur;
                            _avatar.HP.Value = cur;
                        }

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
        
        public event Action<HealthChange> HealthChange;
        
        public event Action<Buff> BuffAdded;
    }

    internal static class Utils
    {
        public static int ToInt(object obj)
        {
            if (obj is int i)
                return i;

            if (obj is short s)
                return s;

            if (obj is byte b)
                return b;

            if (obj is float f)
                return (int)f;

            throw new NotImplementedException();
        }

        public static bool TryToInt(object obj, out int value)
        {
            if (obj is int i)
            {
                value = i;
                return true;
            }

            if (obj is short s)
            {
                value = s;
                return true;
            }

            if (obj is byte b)
            {
                value = b;
                return true;
            }

            value = default;
            return false;
        }

        public static string ToString(object value)
        {
            if (value.GetType() == typeof(byte[]))
                return string.Join(", ", (byte[])value);

            if (value.GetType() == typeof(short[]))
                return string.Join(", ", (short[])value);

            if (value.GetType() == typeof(int[]))
                return string.Join(", ", (int[])value);

            if (value.GetType() == typeof(long[]))
                return string.Join(", ", (long[])value);

            if (value.GetType() == typeof(float[]))
                return string.Join(", ", (float[])value);

            return value.ToString();
        }
    }
}
