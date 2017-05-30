using System.Linq;
using NUnit.Framework;
using UnityEngine;
using Voxelgon.Geometry;
using Voxelgon.Geometry2D;

namespace Voxelgon.Geometry2D.Tests {
    [TestFixture]
    public class TriangulationTests {
        [Test]
        public void TriangulateTest() {
            var verts = new[] {
                new Vector2(-1.0f, -5.1f),
                new Vector2(-0.4f, -5.8f),
                new Vector2(0.8f, -6.6f),
                new Vector2(2.3f, -6.7f),
                new Vector2(4.4f, -6.6f),
                new Vector2(5.7f, -5.8f),
                new Vector2(6.7f, -4.9f),
                new Vector2(7.7f, -3.7f),
                new Vector2(8.8f, -2.5f),
                new Vector2(9.7f, -1.9f),
                new Vector2(10.9f, -2.3f),
                new Vector2(11.0f, -4.3f),
                new Vector2(9.7f, -5.9f),
                new Vector2(8.9f, -6.9f),
                new Vector2(6.7f, -6.9f),
                new Vector2(5.4f, -7.6f),
                new Vector2(4.1f, -7.8f),
                new Vector2(2.9f, -8.9f),
                new Vector2(1.9f, -9.7f),
                new Vector2(0.6f, -10.6f),
                new Vector2(-1.0f, -11.5f),
                new Vector2(-3.1f, -11.5f),
                new Vector2(-5.4f, -11.4f),
                new Vector2(-6.2f, -10.3f),
                new Vector2(-5.3f, -9.5f),
                new Vector2(-3.9f, -9.3f),
                new Vector2(-2.1f, -8.6f),
                new Vector2(-1.4f, -7.7f),
                new Vector2(-1.1f, -6.9f),
                new Vector2(-1.5f, -5.9f),
                new Vector2(-3.1f, -6.1f),
                new Vector2(-4.6f, -6.3f),
                new Vector2(-6.2f, -6.4f),
                new Vector2(-7.3f, -6.5f),
                new Vector2(-8.0f, -6.1f),
                new Vector2(-8.3f, -5.6f),
                new Vector2(-8.3f, -5.0f),
                new Vector2(-7.7f, -4.6f),
                new Vector2(-6.4f, -4.5f),
                new Vector2(-5.0f, -4.2f),
                new Vector2(-3.5f, -3.4f),
                new Vector2(-2.9f, -2.7f),
                new Vector2(-3.0f, -1.8f),
                new Vector2(-3.5f, -1.3f),
                new Vector2(-4.9f, -1.0f),
                new Vector2(-6.2f, -1.0f),
                new Vector2(-7.3f, -1.1f),
                new Vector2(-8.2f, -1.2f),
                new Vector2(-8.9f, -1.4f),
                new Vector2(-9.8f, -0.9f),
                new Vector2(-9.7f, 0.0f),
                new Vector2(-9.5f, 1.1f),
                new Vector2(-8.2f, 0.2f),
                new Vector2(-7.5f, 1.4f),
                new Vector2(-5.7f, 0.3f),
                new Vector2(-6.1f, 1.2f),
                new Vector2(-3.2f, 1.8f),
                new Vector2(-3.3f, 2.7f),
                new Vector2(-2.4f, 2.9f),
                new Vector2(-1.9f, 4.0f),
                new Vector2(-0.4f, 2.5f),
                new Vector2(0.5f, 2.9f),
                new Vector2(1.1f, 1.7f),
                new Vector2(1.9f, 1.9f),
                new Vector2(1.8f, 0.2f),
                new Vector2(3.0f, 0.1f),
                new Vector2(2.1f, -1.2f),
                new Vector2(3.4f, -1.6f),
                new Vector2(1.9f, -2.3f),
                new Vector2(3.1f, -3.3f),
                new Vector2(1.7f, -3.5f),
                new Vector2(1.7f, -4.6f),
                new Vector2(0.7f, -4.0f),
                new Vector2(-0.3f, -4.0f)
            };

            var tris = Triangulation.Triangulate(verts).ToList();

            float polyArea = GeoUtil2D.Shoelace(verts) / 2;
            float triArea = 0;

            for (var i = 0; i < tris.Count; i += 3) {
                triArea += GeoUtil2D.TriangleArea(verts[tris[i]], verts[tris[i + 1]], verts[tris[i + 2]]);
                if (GeoUtil2D.WindingOrder(verts[tris[i]], verts[tris[i + 1]], verts[tris[i + 2]]) == -1) {
                    Assert.Fail("Triangulation gave counter-clockwise triangle at index "
                                + tris[i] + ": "
                                + verts[tris[i]] + ", "
                                + verts[tris[i + 1]] + ", "
                                + verts[tris[i + 2]]);
                }
            }

            Assert.AreEqual(triArea, polyArea);
        }
    }
}