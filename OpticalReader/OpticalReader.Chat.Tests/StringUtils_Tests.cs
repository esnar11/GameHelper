using NUnit.Framework;

namespace OpticalReader.Chat.Tests
{
    public class StringUtils_Tests
    {
        [TestCase("LoomPuL 0 GLOBAL", "Tank", false)]
        [TestCase("need tank Armine", "Tank", true)]
        public void Contains_IgnoreCase_Test(string s1, string s2, bool expected)
        {
            Assert.AreEqual(expected, s1.Contains_IgnoreCase(s2));
        }
    }
}
