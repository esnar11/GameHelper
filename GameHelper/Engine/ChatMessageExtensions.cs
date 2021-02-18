using System;
using System.Collections.Generic;
using System.Linq;
using GameHelper.Interfaces;

namespace GameHelper.Engine
{
    public static class ChatMessageExtensions
    {
        public static bool CanBeRussian(this ChatMessage chatMessage)
        {
            if (chatMessage == null) throw new ArgumentNullException(nameof(chatMessage));

            if (string.IsNullOrWhiteSpace(chatMessage.Message))
                return true;

            if (IsLink(chatMessage.Message))
                return true;

            if (chatMessage.Message.Any(char.IsLetter))
                if (!chatMessage.Message.Any(IsRu))
                    return false;

            return true;
        }

        private static bool IsLink(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            if (Uri.TryCreate(value, UriKind.Absolute, out var uri))
            {
                if (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
                    return true;
            }

            return false;
        }

        private static bool IsEn(char ch)
        {
            return (ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z');
        }

        private static bool IsRu(char ch)
        {
            return (ch >= 'а' && ch <= 'я') || (ch >= 'А' && ch <= 'Я');
        }

        public static bool HasStopWord(this ChatMessage chatMessage, IReadOnlyCollection<string> stopWords)
        {
            foreach (var stopWord in stopWords)
                if (chatMessage.Message.Contains(stopWord, StringComparison.InvariantCultureIgnoreCase))
                    return true;

            return false;
        }

        public static bool HasHighlightWord(this ChatMessage chatMessage, IReadOnlyCollection<string> highlightWords)
        {
            foreach (var highlightWord in highlightWords)
                if (chatMessage.Message.Contains(highlightWord, StringComparison.InvariantCultureIgnoreCase))
                    return true;

            return false;
        }
    }
}
