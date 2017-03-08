using UnityEngine;
using System.Collections.Generic;


namespace Voxelgon.Geometry {

    public class MeshFragment {

        private readonly Vector3[] _vertices;
        private readonly Color32[] _colors32;
        private readonly int[] _tris;

        public MeshFragment(Vector3[] vertices, Color32[] colors32, int[] tris) {
            if (vertices.Length != colors32.Length) throw new InvalidMeshException("array sizes do not match");
            _vertices = (Vector3[])vertices.Clone();
            _colors32 = (Color32[])colors32.Clone();
            _tris = (int[])tris.Clone();
        }

        public MeshFragment(List<Vector3> vertices, List<Color32> colors32, List<int> tris) {
            if (vertices.Count != colors32.Count) throw new InvalidMeshException("array sizes do not match");
            _vertices = vertices.ToArray();
            _colors32 = colors32.ToArray();
            _tris = tris.ToArray();
        }

        public int VertexCount {
            get { return _vertices.Length; }
        }

        public void CopyVertices(List<Vector3> dest) {
            dest.AddRange(_vertices);
        }

        public void CopyColors32(List<Color32> dest) {
            dest.AddRange(_colors32);
        }

        public void CopyTris(List<int> dest, int offset) {
            for (var i = 0; i < _tris.Length; i++) {
                dest.Add(_tris[i] + offset);
            }
        }

        public Mesh ToMesh() {
            Mesh mesh = new Mesh();
            mesh.vertices = _vertices;
            mesh.colors32 = _colors32;
            mesh.triangles = _tris;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }
    }
}