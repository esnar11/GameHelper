using GameHelper.Utils;
using NUnit.Framework;

namespace GameHelper.Tests.Utils
{
    public class NameCreator_Tests
    {
        [Test]
        public void CreateWord_Test()
        {
            var result = NameCreator.CreateWord("РЫСЬ", new[]
            {
                new NameCreator.Letter { Ru = 'Р', En = "P" },
                new NameCreator.Letter { Ru = 'Ы', En = "bI" },
                new NameCreator.Letter { Ru = 'С', En = "C" },
                new NameCreator.Letter { Ru = 'Ь', En = "b" }
            });
            Assert.AreEqual("PbICb", result);
        }
    }
}
