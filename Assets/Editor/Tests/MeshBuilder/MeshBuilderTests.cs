using System.Collections.Generic;
using Voxelgon.MeshBuilder;
using UnityEngine;
using NUnit.Framework;

namespace Voxelgon.MeshBuilder.Tests {
    public class MeshBuilderTests {
        [Test]
        public void WindingOrder() {
            //clockwise triangle
            var point1 = new Vector3(1, 0, 3);
            var point2 = new Vector3(7, 1, 0);
            var point3 = new Vector3(0, 1, 3);

            //colinear points
            var point4 = new Vector3(1, 0, 2);
            var point5 = new Vector3(1, 0, 4);
            var point6 = new Vector3(1, 0, 3);

            //Assert
            Assert.That(
                MeshBuilder.WindingOrder(
                    point1,
                    point2,
                    point3,
                    Vector3.up),
                Is.EqualTo(1)
            );

            Assert.That(
                MeshBuilder.WindingOrder(
                    point3,
                    point2,
                    point1,
                    Vector3.up),
                Is.EqualTo(-1)
            );

            Assert.That(
                MeshBuilder.WindingOrder(
                    point2,
                    point1,
                    point3,
                    Vector3.down),
                Is.EqualTo(1)
            );

            Assert.That(
                MeshBuilder.WindingOrder(
                    point3,
                    point2,
                    point1,
                    Vector3.down),
                Is.EqualTo(1)
            );

            Assert.That(
                MeshBuilder.WindingOrder(
                    point4,
                    point5,
                    point6,
                    Vector3.down),
                Is.EqualTo(0)
            );

            Assert.That(
                MeshBuilder.WindingOrder(
                    point6,
                    point5,
                    point4,
                    Vector3.down),
                Is.EqualTo(0)
            );
        }

        [Test]
        public void TriangleContains() {
            var builder = new MeshBuilder("test", false);
            var points = new List<Vector3>();
            points.Add(new Vector3(4, 0, 4));
            points.Add(new Vector3(2, 0, 0));
            points.Add(new Vector3(0, 0, 2));
            points.Add(new Vector3(2, 0, 2));
            points.Add(new Vector3(2, 0, 3));
            points.Add(new Vector3(2, 0, 4));
            var normal = new Vector3(0, 1, 0);

            builder.AddVertices(points, normal);
            
            Assert.That(
                builder.TriangleWindingOrder(0, 1, 2, normal),
                Is.EqualTo(1)
            );

            Assert.That(
                builder.TriangleContains(0, 1, 2, 3, normal),
                Is.True
            );

            Assert.That(
                builder.TriangleContains(0, 1, 2, 4, normal),
                Is.True
            );

            Assert.That(
                builder.TriangleContains(0, 1, 2, 5, normal),
                Is.False
            );
        }
    }
}