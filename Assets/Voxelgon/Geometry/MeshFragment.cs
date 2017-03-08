using UnityEngine;
using System.Collections.Generic;


namespace Voxelgon.Geometry {

    public class MeshFragment {

        // FIELDS

        private readonly Vector3[] _vertices;
        private readonly Color32[] _colors32;
        private readonly int[] _tris;


        // CONSTRUCTORS

        public MeshFragment(Vector3[] vertices, Color32[] colors32, int[] tris) : this(vertices, colors32, tris, true) { }

        public MeshFragment(List<Vector3> vertices, List<Color32> colors32, List<int> tris) {
            if (vertices.Count != colors32.Count) throw new InvalidMeshException("array sizes do not match");
            _vertices = vertices.ToArray();
            _colors32 = colors32.ToArray();
            _tris = tris.ToArray();
        }

        private MeshFragment(Vector3[] vertices, Color32[] colors32, int[] tris, bool clone) {
            if (vertices.Length != colors32.Length) throw new InvalidMeshException("array sizes do not match");

            if (clone) {
                _vertices = (Vector3[])vertices.Clone();
                _colors32 = (Color32[])colors32.Clone();
                _tris = (int[])tris.Clone();
            } else {
                _vertices = vertices;
                _colors32 = colors32;
                _tris = tris;
            }
        }


        // PROPERTIES

        public int VertexCount {
            get { return _vertices.Length; }
        }


        // METHODS
        public static MeshFragment Cube(float sideLength, Color32 color32, Vector3 center) {
            const int CUBESIZE = 24;
            float r = sideLength / 2;
            Vector3[] vertices = {
                new Vector3(r, -r, r),  new Vector3(-r, -r, r), new Vector3(r, r, r),   new Vector3(-r, r, r),
                new Vector3(r, r, -r),  new Vector3(-r, r, -r), new Vector3(r, -r, -r), new Vector3(-r, -r, -r),
                new Vector3(r, r, r),   new Vector3(-r, r, r),  new Vector3(r, r, -r),  new Vector3(-r, r, -r),
                new Vector3(r, -r, -r), new Vector3(r, -r, r),  new Vector3(-r, -r, r), new Vector3(-r, -r, -r),
                new Vector3(-r, -r, r), new Vector3(-r, r, r),  new Vector3(-r, r, -r), new Vector3(-r, -r, -r),
                new Vector3(r, -r, -r), new Vector3(r, r, -r),  new Vector3(r, r, r),   new Vector3(r, -r, r)
            };

            int[] tris = {
                0,2,3, 0,3,1,
                8,4,5, 8,5,9,
                10,6,7, 10,7,11,
                12,13,14, 12,14,15,
                16,17,18, 16,18,19,
                20,21,22, 20,22,23
            };

            Color32[] colors32 = new Color32[CUBESIZE];
            for (var i = 0; i < CUBESIZE; i++) colors32[i] = color32;

            return new MeshFragment(vertices, colors32, tris, false);
        }

        public static MeshFragment Cube(float sideLength, Color32 color32) {
            return Cube(sideLength, color32, Vector3.zero);
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