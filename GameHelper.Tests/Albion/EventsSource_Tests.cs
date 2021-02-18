using GameHelper.Albion;
using NUnit.Framework;

namespace GameHelper.Tests.Albion
{
    public class EventsSource_Tests
    {
        [TestCase("", "")]
        [TestCase("  Привет  привет  ", "Привет привет")]
        public void Clean_Test(string s, string res)
        {
            Assert.AreEqual(res, EventsSource.Clean(s));

            Assert.AreEqual(
                "ab",
                EventsSource.Clean("a￿FACTION_FOREST￿b"));

            Assert.AreEqual(
                "Новый альянс ищет гильдии, готовые вступить и развиваться вместе! Живём Люме.",
                EventsSource.Clean("￿FACTION_FOREST￿￿OBJECTMARKER_GREEN￿Новый альянс ищет гильдии,  готовые вступить и развиваться вместе! Живём Люме.￿OBJECTMARKER_GREEN￿ ￿FACTION_FOREST￿"));
        }
    }
}
