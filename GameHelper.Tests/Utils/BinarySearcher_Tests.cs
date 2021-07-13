using System.Linq;
using GameHelper.Utils;
using NUnit.Framework;

namespace GameHelper.Tests.Utils
{
    public class BinarySearcher_Tests
    {
        private static readonly byte[] Data1 = { 0, 0, 0, 1, 90, 208, 115, 7, 121, 22, 144, 156, 6, 0, 1, 0, 0, 0, 0, 60, 0, 0, 48, 139, 243, 4, 1, 0, 5, 0, 98, 13, 1, 115, 0, 9, 118, 109, 97, 118, 101, 114, 105, 99, 107, 2, 115, 0, 16, 76, 70, 71, 32, 68, 80, 83, 32, 49, 54, 48, 48, 32, 32, 43, 54, 3, 98, 0, 252, 107, 0, 64 };
        private static readonly byte[] Data2 = { 0, 0, 0, 1, 90, 208, 90, 63, 121, 22, 144, 156, 6, 0, 1, 0, 0, 0, 0, 106, 0, 0, 48, 132, 243, 4, 1, 0, 5, 0, 98, 13, 1, 115, 0, 9, 103, 111, 114, 110, 105, 105, 107, 111, 116, 2, 115, 0, 62, 208, 191, 208, 190, 208, 185, 208, 180, 209, 131, 32, 208, 189, 208, 176, 32, 208, 186, 208, 176, 209, 135, 32, 208, 178, 32, 208, 179, 209, 128, 209, 131, 208, 191, 208, 191, 208, 176, 208, 178, 209, 139, 208, 181, 32, 208, 180, 208, 176, 208, 189, 208, 182, 208, 184, 32, 209, 133, 208, 184, 208, 187, 3, 98, 0, 252, 107, 0, 64 };

        [Test]
        public void SearchByte_Test()
        {
            var results = BinarySearcher.Search<byte>(new byte[0], 200);
            Assert.AreEqual(0, results.Count);
            
            results = BinarySearcher.Search<byte>(new byte[] { 0, 1, 2, 200, 255, 200, 3, 1 }, 200);
            Assert.AreEqual(2, results.Count);
            Assert.AreEqual(3, results.First().Position);
            Assert.AreEqual(5, results.Last().Position);
        }

        [Test]
        public void SearchString_Test()
        {
            var results = BinarySearcher.Search(new byte[0], "Test");
            Assert.AreEqual(0, results.Count);
            
            results = BinarySearcher.Search(Data1, "LFG");
            Assert.AreEqual(3, results.Single().Length);
            
            results = BinarySearcher.Search(Data1, "DPS");
            Assert.AreEqual(3, results.Single().Length);
            
            results = BinarySearcher.Search(Data2, "групп");
            Assert.AreEqual(5, results.Single().Length);
        }
    }
}
