using NUnit.Framework;

namespace OpticalReader.Chat.Tests
{
    public class ChatParser_Tests
    {
        [TestCase(null, null)]
        [TestCase("", null)]
        [TestCase("O GLOBAL", null)]
        public void ParseAuthor_Test(string line, string expected)
        {
            var res = ChatParser.ParseAuthor(line);
            Assert.AreEqual(expected, res);
        }

        [TestCase("LoomPuL 0 GLOBAL", true)]
        public void IsSign_Test(string line, bool expected)
        {
            var res = ChatParser.IsSign(line);
            Assert.AreEqual(expected, res);
        }
    }
}
