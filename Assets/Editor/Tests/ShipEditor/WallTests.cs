using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Voxelgon.ShipEditor;

namespace Voxelgon.ShipEditor.Tests {
    public class WallTests {

        [Test]
        public void WallCannotHaveDuplicateVertices() {
            //Arrange
            var editor = new ShipEditor();
            var wall = new Wall(editor);
            var nodes = new List<Vector3>();
            nodes.Add(new Vector3(5, 8, 3));

            //Act
            wall.UpdateVertices(nodes, ShipEditor.BuildMode.Polygon);

            //Assert
            Assert.That(wall.ValidVertex(new Vector3(5, 8, 3)), Is.False);
        }

    }
}