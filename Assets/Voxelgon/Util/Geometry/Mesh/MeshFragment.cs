using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Voxelgon.Collections;

namespace Voxelgon.Util.Geometry {

    public class MeshFragment {

        // FIELDS

        private List<Vector3> _vertices;
        private List<Color32> _colors32;
        private List<int> _tris;
        protected Bounds _bounds;


        // CONSTRUCTORS

        public MeshFragment(IEnumerable<Vector3> vertices, IEnumerable<Color32> colors32, IEnumerable<int> tris) {
            _vertices = new List<Vector3>(vertices);
            _colors32 = new List<Color32>(colors32);
            _tris = new List<int>(tris);
            if (_vertices.Count != _colors32.Count) throw new InvalidMeshException("array sizes do not match");

            _bounds = GeometryVG.CalcBounds(_vertices);
        }

        protected MeshFragment() { }

        // PROPERTIES

        public virtual IEnumerable<Vector3> Vertices {
            get { return new ReadOnlyEnumerable<Vector3>(_vertices); }
        }

        public virtual IEnumerable<Color32> Colors32 {
            get { return new ReadOnlyEnumerable<Color32>(_colors32); }
        }

        public virtual IEnumerable<int> Tris {
            get { return new ReadOnlyEnumerable<int>(_tris); }
        }

        public virtual int VertexCount {
            get { return _vertices.Count; }
        }

        public virtual Bounds Bounds {
            get { return GeometryVG.CalcBounds(_vertices); }
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

            return new MeshFragment(vertices, colors32, tris);
        }

        public static MeshFragment Cube(float sideLength, Color32 color32) {
            return Cube(sideLength, color32, Vector3.zero);
        }

        public static MeshFragment UVSphere(float radius, int resolution, Color32 color32, Vector3 center, Vector3 axis) {
            axis.Normalize();
            var builder = new MeshBuilder();
            var profile = new Polygon(center, radius, resolution, axis);
            var stepcount = resolution / 4;
            var profcount = (stepcount * 2) + 1;
            var points = new Vector3[profcount];
            var tangents = new Vector3[profcount];
            var scales = new float[profcount];

            points[stepcount] = center;
            scales[stepcount] = 1.0f;
            for (var i = 1; i <= stepcount; i++) {
                float angle = ((float)i / stepcount) * Mathf.PI / 2;
                var cos = Mathf.Cos(angle);
                var sin = Mathf.Sin(angle);

                scales[stepcount + i] = cos;
                scales[stepcount - i] = cos;
                tangents[stepcount + i] = axis;
                tangents[stepcount - i] = axis;
                points[stepcount + i] = center + (sin * axis * radius);
                points[stepcount - i] = center - (sin * axis * radius);
            }

            var path = new Path(points, tangents, scales);

            builder.Sweep(profile, path, color32, false, true, true);
            return builder.ToFragment();
        }

        public static MeshFragment TruncatedUVSphere(float radius, int resolution, Color32 color32, Vector3 center, Vector3 axis, float percent) {
            axis.Normalize();
            var builder = new MeshBuilder();
            var profile = new Polygon(center, radius, resolution, axis);
            var stepcount = resolution / 4;
            var profcount = (stepcount * 2) + 1;
            var points = new Vector3[profcount];
            var tangents = new Vector3[profcount];
            var scales = new float[profcount];

            points[stepcount] = center;
            scales[stepcount] = 1.0f;
            for (var i = 1; i <= stepcount; i++) {
                var sin = ((float)i / stepcount) * percent;
                var cos = Mathf.Sqrt(1 - (sin * sin));

                scales[stepcount + i] = cos;
                scales[stepcount - i] = cos;
                tangents[stepcount + i] = axis;
                tangents[stepcount - i] = axis;
                points[stepcount + i] = center + (sin * axis * radius);
                points[stepcount - i] = center - (sin * axis * radius);
            }

            var path = new Path(points, tangents, scales);

            builder.Sweep(profile, path, color32, false, true, true);
            return builder.ToFragment();

        }

        public void CopyVertices(List<Vector3> dest) {
            dest.AddRange(Vertices);
        }

        public void CopyColors32(List<Color32> dest) {
            dest.AddRange(Colors32);
        }

        public void CopyTris(List<int> dest, int offset) {
            dest.AddRange(Tris.Select(i => i + offset));
        }

        public Mesh ToMesh() {
            Mesh mesh = new Mesh();
            mesh.vertices = Vertices.ToArray();
            mesh.colors32 = Colors32.ToArray();
            mesh.triangles = Tris.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }
    }
}