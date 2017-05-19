using NUnit.Framework;
using Voxelgon.Geometry;
using UnityEngine;

namespace Voxelgon.Tests.Geometry {
    [TestFixture]
    public class GeometryVGTests {
        [Test]
        public void TriangleContains2D() {
            var v0 = new Vector2(0, 0);
            var v1 = new Vector2(4.3f, 3.2f);
            var v2 = new Vector2(5.2f, -3.9f);

            var inside = new Vector2(2.8f, -0.6f);

            Assert.IsTrue(GeoUtil.TriangleContains(v0, v1, v2, inside));
            Assert.IsTrue(GeoUtil.TriangleContains(v1, v2, v0, inside));
            Assert.IsTrue(GeoUtil.TriangleContains(v2, v0, v1, inside));

            Assert.IsTrue(GeoUtil.TriangleContains(v1, v0, v2, inside));
            Assert.IsTrue(GeoUtil.TriangleContains(v2, v1, v0, inside));
            Assert.IsTrue(GeoUtil.TriangleContains(v0, v2, v1, inside));

            var outside = new Vector2(-2.8f, -0.6f);

            Assert.IsFalse(GeoUtil.TriangleContains(v0, v1, v2, outside));
            Assert.IsFalse(GeoUtil.TriangleContains(v1, v2, v0, outside));
            Assert.IsFalse(GeoUtil.TriangleContains(v2, v0, v1, outside));

            Assert.IsFalse(GeoUtil.TriangleContains(v1, v0, v2, outside));
            Assert.IsFalse(GeoUtil.TriangleContains(v2, v1, v0, outside));
            Assert.IsFalse(GeoUtil.TriangleContains(v0, v2, v1, outside));
        }

        [Test]
        public void TriangleArea2D() {
            var v0 = new Vector2(0, 0);
            var v1 = new Vector2(4.3f, 3.2f);
            var v2 = new Vector2(5.2f, -3.9f);

            Assert.AreEqual(16.705f, GeoUtil.TriangleArea(v0, v1, v2));
            Assert.AreEqual(16.705f, GeoUtil.TriangleArea(v1, v2, v0));
            Assert.AreEqual(16.705f, GeoUtil.TriangleArea(v2, v0, v1));
            Assert.AreEqual(-16.705f, GeoUtil.TriangleArea(v0, v2, v1));
        }

        [Test]
        public void Shoelace() {
            var verts = new[] {
                new Vector2(3, 4),
                new Vector2(5, 11),
                new Vector2(12, 8),
                new Vector2(9, 5),
                new Vector2(5, 6)
            };

            Assert.AreEqual(60, GeoUtil.Shoelace(verts));
        }
    }
}