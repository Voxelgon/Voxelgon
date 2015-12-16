using System.Collections.Generic;
using Voxelgon.MeshBuilder;
using UnityEngine;
using NUnit.Framework;

namespace Voxelgon.MeshBuilder.Tests {
    public class MeshBuilderTests {

        private List<Vector3> points = new List<Vector3>
        {
            new Vector3(4, 0, 4),
            new Vector3(2, 0, 0),
            new Vector3(0, 0, 2),
            new Vector3(2, 0, 2),
            new Vector3(2, 0, 3),
            new Vector3(2, 0, 4)   
        };

        private Vector3 normal = Vector3.up;
        private MeshBuilder builder = new MeshBuilder("test", false);

        //SETUP

        [SetUp]
        public void SetUp() {
            builder.AddVertices(points, normal);
        }

        //TESTS

        //WindingOrder

        [Test]
        public void WindingOrder_IsColinear0Deg() {
            Assert.That(
                MeshBuilder.WindingOrder(
                    points[0],
                    points[2],
                    points[4],
                    Vector3.up),
                Is.EqualTo(0)
            );
        }

        [Test]
        public void WindingOrder_IsColinear180Deg() {
            Assert.That(
                MeshBuilder.WindingOrder(
                    points[4],
                    points[2],
                    points[0],
                    Vector3.up),
                Is.EqualTo(0)
            );
        }

        [Test]
        public void WindingOrder_NormalDown_IsClockwise() {
            Assert.That(
                MeshBuilder.WindingOrder(
                    points[0],
                    points[1],
                    points[2],
                    Vector3.up),
                Is.EqualTo(1)
            );

            Assert.That(
                MeshBuilder.WindingOrder(
                    points[2],
                    points[1],
                    points[0],
                    Vector3.down),
                Is.EqualTo(1)
            );
        }

        [Test]
        public void WindingOrder_IsCounterClockwise() {
            Assert.That(
                MeshBuilder.WindingOrder(
                    points[2],
                    points[1],
                    points[0],
                    Vector3.up),
                Is.EqualTo(-1)
            );

            Assert.That(
                MeshBuilder.WindingOrder(
                    points[0],
                    points[1],
                    points[2],
                    Vector3.down),
                Is.EqualTo(-1)
            );
        }

        //TriangleContains

        [Test]
        public void TriangleContains_Inside() {
            Assert.That(
                builder.TriangleContains(0, 1, 2, 3, normal),
                Is.True
            );
        }

        [Test]
        public void TriangleContains_OnEdge() {
            Assert.That(
                builder.TriangleContains(0, 1, 2, 4, normal),
                Is.True
            );
        }

        [Test]
        public void TriangleContains_Outside() {
            Assert.That(
                builder.TriangleContains(0, 1, 2, 5, normal),
                Is.False
            );
        }
    }
}