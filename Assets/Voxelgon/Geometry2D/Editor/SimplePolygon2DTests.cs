using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace Voxelgon.Geometry2D.Tests {
    [TestFixture]
    public class SimplePolygon2DTests {
        private static readonly List<Vector2> verts1 = new List<Vector2> {
            new Vector2(3, 4),
            new Vector2(5, 11),
            new Vector2(12, 8),
            new Vector2(9, 5),
            new Vector2(5, 6)
        };

        private static readonly List<Vector2> verts2 = new List<Vector2> {
            new Vector2(5, 6),
            new Vector2(9, 5),
            new Vector2(12, 8),
            new Vector2(5, 11),
            new Vector2(3, 4)
        };

        private static readonly List<Vector2> verts3 = new List<Vector2> {
            new Vector2(5, 6),
            new Vector2(10, 7),
            new Vector2(9, 3),
            new Vector2(4, 2)
        };

        [Test]
        public void Polygon2DCtor() {
            var poly = new SimplePolygon2D(verts1);

            Assert.AreEqual(5, poly.VertexCount);
            Assert.IsTrue(verts1.SequenceEqual(poly.Vertices));
        }

        [Test]
        public void Polygon2DArea() {
            var poly1 = new SimplePolygon2D(verts1);

            Assert.AreEqual(30, poly1.Area);


            var poly2 = new SimplePolygon2D(verts2);

            Assert.AreEqual(-30, poly2.Area);
        }

        [Test]
        public void Polygon2DContains() {
            var poly1 = new SimplePolygon2D(verts1);

            Assert.IsTrue(poly1.Contains(new Vector2(7, 7)));
            Assert.IsTrue(poly1.Contains(new Vector2(4, 5.5f)));
            Assert.IsFalse(poly1.Contains(new Vector2(6, 5)));
            Assert.IsFalse(poly1.Contains(new Vector2(1, 8)));
            Assert.IsFalse(poly1.Contains(new Vector2(12, 6)));

            var poly2 = new SimplePolygon2D(verts2);

            Assert.IsTrue(poly2.Contains(new Vector2(7, 7)));
            Assert.IsTrue(poly2.Contains(new Vector2(4, 5.5f)));
            Assert.IsFalse(poly2.Contains(new Vector2(6, 5)));
            Assert.IsFalse(poly2.Contains(new Vector2(1, 8)));
            Assert.IsFalse(poly2.Contains(new Vector2(12, 6)));
        }

        [Test]
        public void Polygon2DWinding() {
            var poly1 = new SimplePolygon2D(verts1);

            Assert.IsTrue(poly1.Clockwise);

            var poly2 = new SimplePolygon2D(verts2);

            Assert.IsFalse(poly2.Clockwise);
        }

        [Test]
        public void Polygon2DEquals() {
            var poly1 = new SimplePolygon2D(verts1);
            var poly2 = new SimplePolygon2D(verts2);
            var poly3 = new SimplePolygon2D(verts1);

            Assert.IsTrue(poly1.Equals(poly3));
            Assert.IsFalse(poly1.Equals(poly2));
        }

        [Test]
        public void Polygon2DIsConvex() {
            var poly1 = new SimplePolygon2D(verts1);
            var poly2 = new SimplePolygon2D(verts2);
            var poly3 = new SimplePolygon2D(verts3);

            Assert.IsFalse(poly1.IsConvex);
            Assert.IsFalse(poly2.IsConvex);
            Assert.IsTrue(poly3.IsConvex);
        }
    }
}