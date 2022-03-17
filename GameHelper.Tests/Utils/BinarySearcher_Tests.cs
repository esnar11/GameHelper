using System;
using System.Linq;
using System.Text;
using GameHelper.Interfaces.LowLevel;
using GameHelper.Utils;
using NUnit.Framework;

namespace GameHelper.Tests.Utils
{
    public class BinarySearcher_Tests
    {
        private static readonly byte[] Data1 = { 0, 0, 0, 1, 90, 208, 115, 7, 121, 22, 144, 156, 6, 0, 1, 0, 0, 0, 0, 60, 0, 0, 48, 139, 243, 4, 1, 0, 5, 0, 98, 13, 1, 115, 0, 9, 118, 109, 97, 118, 101, 114, 105, 99, 107, 2, 115, 0, 16, 76, 70, 71, 32, 68, 80, 83, 32, 49, 54, 48, 48, 32, 32, 43, 54, 3, 98, 0, 252, 107, 0, 64 };
        private static readonly Datagram EmptyDatagram = new Datagram(Array.Empty<byte>(), DataDirection.ClientToServer);

        [Test]
        public void SearchByte_Test()
        {
            var results = BinarySearcher.Search<byte>(EmptyDatagram, 200);
            Assert.AreEqual(0, results.Count);
            
            results = BinarySearcher.Search<byte>(new Datagram(new byte[] { 0, 1, 2, 200, 255, 200, 3, 1 }, DataDirection.ClientToServer), 200);
            Assert.AreEqual(2, results.Count);
            Assert.AreEqual(3, results.First().Position);
            Assert.AreEqual(5, results.Last().Position);
        }

        [Test]
        public void SearchUShort_Test()
        {
            var results = BinarySearcher.Search<ushort>(EmptyDatagram, 1000);
            Assert.AreEqual(0, results.Count);
            
            results = BinarySearcher.Search<ushort>(new Datagram(new byte[] { 0, 1, 2, 3, 232, 3, 2, 1, 0 }, DataDirection.ClientToServer), 1000);
            Assert.AreEqual(3, results.Single().Position);
            Assert.AreEqual(2, results.Single().Length);
        }

        [Test]
        public void SearchString_Test()
        {
            var data = Encoding.UTF8.GetBytes("В лесу родилась ёлочка, в лесу она росла");
            var results = BinarySearcher.Search(new Datagram(data, DataDirection.ClientToServer), "лесу");
            Assert.AreEqual(2, results.Count);
            Assert.AreEqual(2, results.First().Position);
            Assert.AreEqual(4, results.First().Length);
            Assert.AreEqual(26, results.Skip(1).First().Position);
            Assert.AreEqual(4, results.Skip(1).First().Length);

            results = BinarySearcher.Search(EmptyDatagram, "Test");
            Assert.AreEqual(0, results.Count);
            
            results = BinarySearcher.Search(new Datagram(Data1, DataDirection.ClientToServer), "LFG");
            Assert.AreEqual(3, results.Single().Length);
            
            results = BinarySearcher.Search(new Datagram(Data1, DataDirection.ClientToServer), "DPS");
            Assert.AreEqual(3, results.Single().Length);
        }
    }
}
