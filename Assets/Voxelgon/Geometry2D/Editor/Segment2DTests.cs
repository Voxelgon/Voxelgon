using NUnit.Framework;
using UnityEngine;

namespace Voxelgon.Geometry2D.Tests {
    [TestFixture]
    public class Segment2DTests {
        [Test]
        public void SegmentRayCast() {
            var v1 = new Vector2(5, 7);
            var v2 = new Vector2(9, 3);

            var ray1 = new Ray2D(new Vector2(3, 2), new Vector2(3, 2)); //should hit
            var ray2 = new Ray2D(new Vector2(9, 6), new Vector2(-1, -1)); //should hit
            var ray3 = new Ray2D(new Vector2(3, 4), new Vector2(1, 3)); //should miss
            var ray4 = new Ray2D(new Vector2(9, 6), new Vector2(1, 1)); //should miss

            Assert.IsTrue(Segment2D.RaycastSegment(ray1, v1, v2));
            Assert.IsTrue(Segment2D.RaycastSegment(ray1, v2, v1)); //opposite segment orientation

            Assert.IsTrue(Segment2D.RaycastSegment(ray2, v1, v2));

            Assert.IsFalse(Segment2D.RaycastSegment(ray3, v1, v2));

            Assert.IsFalse(Segment2D.RaycastSegment(ray4, v1, v2));
        }

        [Test]
        public void SegmentIntersection() {
            var segment1 = new Segment2D(new Vector2(5, 7), new Vector2(9, 3)); //target
            var segment2 = new Segment2D(new Vector2(4, 4), new Vector2(10, 10)); //intersects at (6,6)
            var segment3 = new Segment2D(new Vector2(3, 4), new Vector2(4, 5)); //does not intersect, stops short of target
            var segment4 = new Segment2D(new Vector2(3, 2), new Vector2(5, 2)); //does not intersect, does not hit target
            var segment5 = new Segment2D(Vector2.left, Vector2.right);
            var segment6 = new Segment2D(Vector2.down, Vector2.up);

            Vector2 intersection;
            Assert.IsTrue(segment1.Intersect(segment2, out intersection));
            Assert.AreEqual(new Vector2(6,6), intersection);

            Assert.IsTrue(segment2.Intersect(segment1, out intersection));
            Assert.AreEqual(new Vector2(6,6), intersection);

            Assert.IsFalse(segment1.Intersect(segment3, out intersection));
            Assert.IsFalse(segment1.Intersect(segment4, out intersection));

            Assert.IsTrue(segment5.Intersect(segment6, out intersection));
            Assert.AreEqual(Vector2.zero, intersection);
        }

        [Test]
        public void SegmentLength() {
            var segment = new Segment2D(new Vector2(5, 7), new Vector2(9, 3));

            Assert.AreEqual(5.65685, segment.Length, 0.001);
            Assert.AreEqual(32, segment.SqrLength);
        }

        [Test]
        public void SegmentSlope() {
            var segment1 = new Segment2D(new Vector2(9, 3), new Vector2(5, 7));
            var segment2 = new Segment2D(new Vector2(5, 7), new Vector2(8, 12));
            var segment3 = new Segment2D(new Vector2(5, 7), new Vector2(5, 10));
            var segment4 = new Segment2D(new Vector2(5, 7), new Vector2(5, -4));

            Assert.AreEqual(-1, segment1.Slope);
            Assert.AreEqual((5f/3f), segment2.Slope);
            Assert.AreEqual(float.PositiveInfinity, segment3.Slope);
            Assert.AreEqual(float.NegativeInfinity, segment4.Slope);
        }
    }
}