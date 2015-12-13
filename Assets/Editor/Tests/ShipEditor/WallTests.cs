using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Voxelgon.ShipEditor;

namespace Voxelgon.Tests {
    public class WallTests {

        [Test]
        public void WallCannotHaveDuplicateVertices() {
            //Arrange
            var wall = new Wall();
            var nodes = new List<Vector3>();
            nodes.Add(new Vector3(5, 8, 3));

            //Act
            wall.UpdateVertices(nodes, ShipEditor.ShipEditor.BuildMode.Polygon);

            //Assert
            Assert.That(wall.ValidVertex(new Vector3(5, 8, 3)), Is.False);
        }

    }
}