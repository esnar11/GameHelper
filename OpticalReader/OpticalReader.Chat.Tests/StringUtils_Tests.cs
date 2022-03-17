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

        [TestCase(
            "Ru CL Blood Corsairs LFM for PVP PvE active and Funn•.)-",
            "Ru CL Blood Corsairs LFM for PVP PvE active and Funn•.)-",
            1)]
        [TestCase(
            "Ru CL Blood Corsairs LFM for PVP PvE active and Funn•.)-",
            "Ru CL Blood Corsairs LFM for PVP PvE active and Fun F)",
            0.9)]
        [TestCase(
            "Ru CL Blood Corsairs LFM for PVP PvE active and Funn•.)-",
            "Ru CL Blood Corsairs LFM for PVP PvE active a",
            0.9)]
        public void EqualsRatio_Test(string s1, string s2, double expected)
        {
            var ratio = s1.EqualsRatio(s2);
            Assert.AreEqual(expected, ratio, 0.1);
        }
    }
}
