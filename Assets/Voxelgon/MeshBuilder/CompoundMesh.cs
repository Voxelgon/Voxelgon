using UnityEngine;
using System.Collections.Generic;

namespace Voxelgon.MeshBuilder {
    public class CompoundMesh {
        private List<Vector3> _vertices;
        private List<Vector3> _normals;
        private List<Vector3> _uvs;
        private List<Vector3> _tangents;
        private List<Color> _colors;
        private List<int> _triangles;
    }
}
