using NUnit.Framework;

namespace OpticalReader.Chat.Tests
{
    public class Chat_Tests
    {
        [TestCase("LoomPuL 0 GLOBAL", false)]
        [TestCase("need tank Armine", true)]
        public void NeedHighlight_Test(string line, bool expected)
        {
            var settings = new ChatSettings();

            var res = Chat.NeedHighlight(new Message("", "", line), settings);
            Assert.AreEqual(expected, res);
        }
    }
}
