using System;
using GameHelper.Interfaces;
using GameHelper.Utils;
using NUnit.Framework;

namespace GameHelper.Tests
{
    public class ChatMessageExtensions_Tests
    {
        [TestCase("", true)]
        [TestCase("  123 ?-` 456  ", true)]
        [TestCase("  123 аБв ABCD  ", true)]
        [TestCase("  https://discord.gg/eA8mBdd3  ", true)]
        [TestCase("123 AbcD", false)]
        [TestCase("绿城1245冰法求组蓝洞", false)]
        [TestCase("LFG: HEaler 1443 IP + skip set", false)]
        public void CanBeRussian_Test(string msg, bool res)
        {
            var chatMessage = new ChatMessage { Message = msg };
            Assert.AreEqual(res, chatMessage.CanBeRussian());
        }

        [TestCase("", "", false)]
        [TestCase("Hello", "", false)]
        [TestCase("Hello", "guild", false)]
        [TestCase("Hello guild", "guild", true)]
        [TestCase("Hello приму в guild", "guild;приму", true)]

        [TestCase("приму в ги Суперги", "приму в ги", true)]
        [TestCase("приму в ги Суперги", "приму в пати", false)]

        public void HasStopWord_Test(string msg, string stopWords, bool res)
        {
            var chatMessage = new ChatMessage { Message = msg };
            Assert.AreEqual(res, chatMessage.HasStopWord(stopWords.Split(';', StringSplitOptions.RemoveEmptyEntries)));
        }

        [TestCase("LFG TANK 1750", "танк;tank", true)]
        public void HasHighlightWord_Test(string msg, string stopWords, bool res)
        {
            var chatMessage = new ChatMessage { Message = msg };
            Assert.AreEqual(res, chatMessage.HasHighlightWord(stopWords.Split(';', StringSplitOptions.RemoveEmptyEntries)));
        }
    }
}
