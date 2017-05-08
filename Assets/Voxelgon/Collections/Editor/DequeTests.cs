using NUnit.Framework;

namespace Voxelgon.Collections.Tests {
    [TestFixture]
    public class DequeTests {
        private static readonly int[] testData = {
            1, 3, 4, 2, 12, 54, 12, 33, 29, 12, 34, 84, -10, 40
        };

        [Test]
        public void DequePush() {
            var deque = new Deque<int>();

            deque.PushFront(4);
            deque.PushBack(5);
            deque.PushFront(6);

            Assert.AreEqual(3, deque.Count);

            Assert.AreEqual(6, deque.PopFront());
            Assert.AreEqual(4, deque.PopFront());
            Assert.AreEqual(5, deque.PopFront());

            Assert.AreEqual(0, deque.Count);
        }

        [Test]
        public void DequePushFrontCollection() {
            var deque = new Deque<int>();

            deque.PushFront(testData);

            Assert.AreEqual(14, deque.Count);
            Assert.AreEqual(1, deque.PeekFront());
            Assert.AreEqual(40, deque.PeekBack());
        }

        [Test]
        public void DequePushBackCollection() {
            var deque = new Deque<int>();

            deque.PushBack(testData);

            Assert.AreEqual(14, deque.Count);
            Assert.AreEqual(1, deque.PeekFront());
            Assert.AreEqual(40, deque.PeekBack());
        }

        [Test]
        public void DequeExpands() {
            var deque1 = new Deque<int>(3);

            deque1.PushFront(4);
            deque1.PushFront(14);
            deque1.PushBack(43);
            deque1.PushBack(421);
            deque1.PushBack(-43);

            Assert.AreEqual(5, deque1.Count);
            Assert.GreaterOrEqual(deque1.Capacity, 5);

            var deque2 = new Deque<int>(3);
            deque2.PushBack(testData);
            Assert.AreEqual(14, deque2.Count);
            Assert.GreaterOrEqual(deque2.Capacity, 14);
        }

        [Test]
        public void DequeRemove() {
            var deque = new Deque<int>();

            deque.PushBack(testData);

            Assert.IsTrue(deque.Remove(12));
            Assert.AreEqual(2, deque[3]);
            Assert.AreEqual(54, deque[4]);
            Assert.AreEqual(13, deque.Count);

            Assert.IsFalse(deque.Remove(38));
            Assert.AreEqual(13, deque.Count);

            deque.RemoveAt(6);
            Assert.AreEqual(12, deque[5]);
            Assert.AreEqual(29, deque[6]);
            Assert.AreEqual(12, deque.Count);
        }

        [Test]
        public void DequeInsert() {
            var deque = new Deque<int>();

            deque.PushBack(testData);

            deque.Insert(5, 50);
            Assert.AreEqual(12, deque[4]);
            Assert.AreEqual(50, deque[5]);
            Assert.AreEqual(54, deque[6]);
            Assert.AreEqual(15, deque.Count);
        }

    }
}