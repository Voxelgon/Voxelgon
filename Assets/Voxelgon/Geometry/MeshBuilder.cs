using UnityEngine;
using System.Collections.Generic;


namespace Voxelgon.Geometry {

    public class MeshBuilder {

        //Private Fields

        private readonly string _name;

        private readonly bool _optimize;

        private readonly bool _calcNormals;

        private readonly List<Vector3> _vertices;

        private readonly List<Vector3> _normals;

        private readonly List<Color32> _colors32;

        private readonly List<int> _tris;

        private readonly List<Mesh> _completedMeshes;

        private readonly bool _printDebug = false;

        //Constructors

        public MeshBuilder(string name = "mesh", bool optimize = true, bool calcNormals = true) {
            _vertices = new List<Vector3>(512);
            _normals = new List<Vector3>(512);
            _colors32 = new List<Color32>(512);
            _tris = new List<int>(512);

            _completedMeshes = new List<Mesh>();

            _name = name;
            _optimize = optimize;
            _calcNormals = calcNormals;

#if PRINTDEBUG
            _printDebug = true;
#endif
        }

        //Properties

        public List<Mesh> AllMeshes {
            get {
                FinalizeLastMesh();
                return _completedMeshes;
            }
        }

        public Mesh FirstMesh {
            get {
                return AllMeshes[0];
            }
        }


        //Public Methods

        public void Clear() {
            ClearBuffer();
            _completedMeshes.Clear();
        }

        public void AddPolygon(Polygon p) {
            int size = p.VertexCount;
            int offset = CheckSize(size);

            for (var i = 0; i < size; i++) {
                _colors32.Add(p.Color);
                _normals.Add(p.Normal);
            }
            _vertices.AddRange(p.Vertices);

            foreach (int tri in p.TriangleIndices) {
                _tris.Add(offset + tri);
            }
        }

        public void AddTriangle(Vector3 point0, Vector3 point1, Vector3 point2, Color32 color) {
            var triOffset = CheckSize(3);

            _vertices.Add(point0);
            _vertices.Add(point1);
            _vertices.Add(point2);

            for (var t = 0; t < 3; t++) {
                _colors32.Add(color);
                _normals.Add(Geometry.TriangleNormal(point0, point1, point2));
                _tris.Add(triOffset + t);
            }
        }

        public int AddVertices(List<Vector3> vertices, List<Vector3> normals, List<Color32> colors) {
            int size = vertices.Count;

            if (normals.Count != size || colors.Count != size) {
                throw new InvalidPolygonException();
            }

            int offset = CheckSize(size);

            _vertices.AddRange(vertices);
            _normals.AddRange(normals);
            _colors32.AddRange(colors);

            return offset;
        }

        public int AddVertices(Vector3[] vertices, Vector3[] normals, Color32 color) {
            int size = vertices.Length;

            if (normals.Length != size) {
                throw new InvalidPolygonException();
            }

            int offset = CheckSize(size);

            _vertices.AddRange(vertices);
            _normals.AddRange(normals);
            for (var i = 0; i < size; i++) {
                _colors32.Add(color);
            }

            return offset;
        }

        public int AddVertices(Vector3[] vertices, Vector3 normal, Color32 color) {
            int size = vertices.Length;
            int offset = CheckSize(size);

            _vertices.AddRange(vertices);
            for (var i = 0; i < size; i++) {
                _normals.Add(normal);
                _colors32.Add(color);
            }

            return offset;
        }

        public void BridgePolygons(Polygon p1, Polygon p2, Color32 color, bool smooth = true) {
            if (p1.VertexCount > p2.VertexCount) {
                BridgePolygons(p2, p1, color, smooth);
                return;
            }

            var offset = p2.Center - p1.Center;

            if (p1.WindingOrder(offset) < 0) {
                p1 = p1.Reverse();
            }

            if (p2.WindingOrder(offset) < 0) {
                p2 = p2.Reverse();
            }

            var p2Indices = VertexPairs(p1, p2);
            if (smooth) {
                int p1Start = AddVertices(p1.Vertices, p1.VertexNormals, color);
                int p2Start = AddVertices(p2.Vertices, p2.VertexNormals, color);

                BridgeSmoothLoop(p1Start, p1.VertexCount, p2Start, p2.VertexCount, p2Indices);
            } else {
                BridgeTriLoop(p1, p2, p2Indices, color);
            }
        }

        public void Extrude(Polygon p, Vector3 offset, float scale = 1.0f, bool smooth = true) {
            if (smooth) {

            } else {
            }

        }

        public void Sweep(Polygon profile, Path path, Color32 color, bool cap = false, bool smooth = false, bool smoothCorners = true) {
            Polygon p1 = null;
            Polygon p2 = profile.Transform(Matrix4x4.TRS(
                path.GetVertex(0),
                Quaternion.FromToRotation(profile.Normal, path.GetTangent(0)),
                Vector3.one * path.GetScale(0)));

            if (cap) {
                AddPolygon(p2);
            }

            for (var i = 1; i < path.VertexCount; i++) {
                p1 = p2;
                p2 = profile.Transform(Matrix4x4.TRS(
                    path.GetVertex(i),
                    Quaternion.FromToRotation(profile.Normal, path.GetTangent(i)),
                    Vector3.one * path.GetScale(i)));

                if (smoothCorners) {

                } else {
                    BridgePolygons(p1, p2, color, smooth);
                }
            }

            if (cap) {
                AddPolygon(p2);
            }
        }

        public static Mesh MergeMeshes(List<Mesh> meshes) {
            var compoundMesh = new Mesh();

            var compoundVertices = new List<Vector3>();
            var compoundNormals = new List<Vector3>();
            var compoundColors = new List<Color>();
            var compoundTriangles = new List<int>();

            foreach (Mesh m in meshes) {
                if (compoundVertices.Count + m.vertices.Length > 65534) {
                    break;
                }
                compoundVertices.AddRange(m.vertices);
                compoundNormals.AddRange(m.normals);
                compoundColors.AddRange(m.colors);
                compoundTriangles.AddRange(m.triangles);

                if (compoundTriangles.Count - m.triangles.Length != 0) {
                    for (int t = 1; t <= m.triangles.Length; t++) {
                        compoundTriangles[compoundTriangles.Count - t] += compoundVertices.Count - m.vertices.Length;
                    }
                }
            }

            compoundMesh.SetVertices(compoundVertices);
            compoundMesh.SetNormals(compoundNormals);
            compoundMesh.SetColors(compoundColors);
            compoundMesh.SetTriangles(compoundTriangles, 0);
            compoundMesh.RecalculateBounds();
            compoundMesh.RecalculateNormals();

            return compoundMesh;
        }

        //Private Methods

        private void ClearBuffer() {
            _vertices.Clear();
            _normals.Clear();
            _colors32.Clear();
            _tris.Clear();
        }

        private int CheckSize(int size) {
            if (_vertices.Count + size > 65534) {
                //current mesh is too full to add to, finalize it and clear the buffer
                FinalizeLastMesh();
                return 0;
            }
            return _vertices.Count;
        }

        private void FinalizeLastMesh() {
            if (_vertices.Count > 0) {
                var lastMesh = new Mesh();

                lastMesh.SetVertices(_vertices);
                lastMesh.SetNormals(_normals);
                lastMesh.SetColors(_colors32);

                lastMesh.SetTriangles(_tris, 0);
                lastMesh.RecalculateBounds();

                if (_calcNormals) {
                    //lastMesh.RecalculateNormals();
                }

                if (_optimize) {
                    ;
                }

                _completedMeshes.Add(lastMesh);
                ClearBuffer();
            }
        }

        private void BridgeSmoothSegment(int p1Index1, int p1Index2, int P2Offset1, int p2Offset2, int p2Start, int p2Size) {
            _tris.Add(p1Index1);
            _tris.Add(p1Index2);
            _tris.Add(p2Start + P2Offset1);

            for (int j = P2Offset1; j != p2Offset2;) {
                _tris.Add(p2Start + j);
                _tris.Add(p1Index2);
                _tris.Add(p2Start + (j = ((j + 1) % p2Size)));
            }
        }

        private void BridgeSmoothLoop(int p1Start, int p1Size, int p2Start, int p2Size, int[] p2Indices) {
            for (int i = 0; i < p1Size; i++) {
                BridgeSmoothSegment(
                    p1Start + i,
                    p1Start + (i + 1) % p1Size,
                    p2Indices[i],
                    p2Indices[(i + 1) % p1Size],
                    p2Start, p2Size);
            }
        }

        private void BridgeTriLoop(Polygon p1, Polygon p2, int[] p2Indices, Color32 color) {
            var p1Size = p1.VertexCount;
            var p2Size = p2.VertexCount;
            for (int i = 0; i < p1Size; i++) {
                AddTriangle(
                    p1.GetVertex(i),
                    p1.GetVertex((i + 1) % p1Size),
                    p2.GetVertex(p2Indices[i]),
                    color);
                for (int j = p2Indices[i]; j != p2Indices[(i + 1) % p1Size]; j = (j + 1) % p2Size) {
                    AddTriangle(
                        p1.GetVertex((i + 1) % p1Size),
                        p2.GetVertex((j + 1) % p2Size),
                        p2.GetVertex(j),
                        color);
                }
            }
        }

        private static int[] VertexPairs(Polygon p1, Polygon p2) {
            if (p1.VertexCount > p2.VertexCount) {
                throw new System.ArgumentException();
            }
            var offset = p1.Center - p2.Center;

            var p1Size = p1.VertexCount;
            var p2Size = p2.VertexCount;

            var p1Flat = p1.FlatVertices(offset);
            var p2Flat = p2.FlatVertices(offset);

            var p2Indices = new int[p1Size];

            if (p1Size == p2Size) {
                var smallest = 0;
                var smallestDist = float.MaxValue;

                for (var i = 0; i < p2Size; i++) {
                    var dist = (p1Flat[0] - p2Flat[i]).SqrMagnitude();
                    if (dist < 0.01f) break;
                    if (dist < smallestDist) {
                        smallestDist = dist;
                        smallest = i;
                    }
                }

                for (var i = 0; i < p1Size; i++) {
                    p2Indices[i] = smallest;
                    smallest = (smallest + 1) % p2Size;
                }
                return p2Indices;
            }

            for (int i = 0; i < p1Size; i++) {
                Vector3 p1Vertex = (p1.GetVertex(i) + p1.GetVertex((i + 1) % p1Size)) / 2;
                float angle = 100;
                int index = -1;
                for (int j = 0; j < p2Size; j++) {
                    float thisAngle = Mathf.Abs(Geometry.VectorAngle(p1Vertex - p1.Center, p2.GetVertex(j) - p2.Center, offset));
                    if (thisAngle > 0 && thisAngle < angle) {
                        index = j;
                        angle = thisAngle;
                    }
                }
                p2Indices[i] = index;
            }

            return p2Indices;
        }
    }
}