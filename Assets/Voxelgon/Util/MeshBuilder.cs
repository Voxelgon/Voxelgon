using UnityEngine;
using System;
using System.Collections.Generic;
using Voxelgon.Util.Geometry;

namespace Voxelgon.Util {

    public class MeshBuilder {

        //Private Fields

        private readonly List<Vector3> _vertices;
        private readonly List<Color32> _colors32;

        private readonly List<int> _tris;

        private readonly List<Mesh> _completedMeshes;

        //Constructors

        public MeshBuilder(int VertexCount) {
            _vertices = new List<Vector3>(VertexCount);
            _colors32 = new List<Color32>(VertexCount);
            _tris = new List<int>(VertexCount);

            _completedMeshes = new List<Mesh>();
        }


        public MeshBuilder() : this(512) {}

        //Properties

        public List<Mesh> AllMeshes {
            get {
                FinalizeLastMesh();
                return _completedMeshes;
            }
        }

        public Mesh FirstMesh {
            get {
                if (AllMeshes.Count == 0) {
                    return new Mesh();
                }
                return AllMeshes[0];
            }
        }


        //Public Methods

        public void Clear() {
            ClearBuffer();
            _completedMeshes.Clear();
        }

        public void AddPolygon(Polygon p, Color32 color) {
            int offset = AddVertices(p, color);
            p.CopyTris(_tris, offset);
        }

        public void AddTriangle(Vector3 point0, Vector3 point1, Vector3 point2, Color32 color) {
            var triOffset = CheckSize(3);

            _vertices.Add(point0);
            _vertices.Add(point1);
            _vertices.Add(point2);

            for (var t = 0; t < 3; t++) {
                _colors32.Add(color);
                _tris.Add(triOffset + t);
            }
        }

        public void AddFragment(MeshFragment fragment) {
            var triOffset = CheckSize(fragment.VertexCount);

            fragment.CopyVertices(_vertices);
            fragment.CopyColors32(_colors32);
            fragment.CopyTris(_tris, triOffset);
        }

        public int AddVertices(Polygon p, Color32 color) {
            int size = p.VertexCount;
            int offset = CheckSize(size);

            for (var i = 0; i < size; i++) {
                _colors32.Add(color);
            }

            p.CopyVertices(_vertices);

            return offset;
        }

        public int AddVertices(List<Vector3> vertices, List<Color32> colors) {
            int size = vertices.Count;

            if (colors.Count != size) {
                throw new InvalidPolygonException();
            }

            int offset = CheckSize(size);

            _vertices.AddRange(vertices);
            _colors32.AddRange(colors);

            return offset;
        }

        public int AddVertices(Vector3[] vertices, Color32 color) {
            int size = vertices.Length;
            int offset = CheckSize(size);

            _vertices.AddRange(vertices);
            for (var i = 0; i < size; i++) {
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

            var pairs = PolygonPairs(p1, p2);
            var dirs = PolygonDirs(p1, p2, pairs);
            if (smooth) {
                int p1Start = AddVertices(p1, color);
                int p2Start = AddVertices(p2, color);

                BridgeSmoothLoop(p1Start, p1.VertexCount, p2Start, p2.VertexCount, pairs, dirs);
            } else {
                BridgeTriLoop(p1, p2, pairs, dirs, color);
            }
        }

        public void Extrude(Polygon p, Vector3 offset, float scale = 1.0f, bool smooth = true) {
            if (smooth) {

            } else {
            }

        }

        public void Sweep(Polygon profile, Path path, Color32 color, bool cap = false, bool smooth = false, bool smoothCorners = true) {
            Polygon p1;
            Polygon p2 = profile.Transform(Matrix4x4.TRS(
                path.GetVertex(0),
                Quaternion.FromToRotation(profile.Normal, path.GetTangent(0)),
                Vector3.one * path.GetScale(0)));
            int size = profile.VertexCount;

            if (cap) {
                AddPolygon(p2.Reverse(), color);
            }

            int p1Start = -1;
            int p2Start = -1;

            if (smooth && smoothCorners) {
                p2Start = AddVertices(p2, color);
            }

            for (var i = 1; i < path.VertexCount; i++) {
                p1 = p2;
                p2 = profile.Transform(Matrix4x4.TRS(
                    path.GetVertex(i),
                    Quaternion.FromToRotation(profile.Normal, path.GetTangent(i)),
                    Vector3.one * path.GetScale(i)));

                var dirs = PolygonDirs(p1, p2);

                if (smooth) {
                    if (smoothCorners) {
                        p1Start = p2Start;
                        p2Start = AddVertices(p2, color);
                    } else {
                        p1Start = AddVertices(p1, color);
                        p2Start = AddVertices(p2, color);
                    }
                    BridgeSmoothLoop(p1Start, p2Start, dirs, size);
                } else {
                    BridgeTriLoop(p1, p2, dirs, color);
                }
            }

            if (cap) {
                AddPolygon(p2, color);
            }
        }

        public MeshFragment ToFragment() {
            return new MeshFragment(_vertices, _colors32, _tris);
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
                lastMesh.SetColors(_colors32);

                lastMesh.SetTriangles(_tris, 0);
                lastMesh.RecalculateBounds();

                lastMesh.RecalculateNormals();

                _completedMeshes.Add(lastMesh);
                ClearBuffer();
            }
        }

        private void BridgeSmoothLoop(int p1Start, int p1Size, int p2Start, int p2Size, int[] pairs, bool[] dirs) {
            for (int i = 0; i < p1Size; i++) {
                int next = (i + 1) % p1Size;
                if (dirs[i]) {
                    _tris.Add(p1Start + i);
                    _tris.Add(p1Start + next);
                    _tris.Add(p2Start + pairs[i]);

                    for (int j = pairs[i]; j != pairs[next];) {
                        _tris.Add(p2Start + j);
                        _tris.Add(p1Start + next);
                        _tris.Add(p2Start + (j = ((j + 1) % p2Size)));
                    }
                } else {
                    _tris.Add(p1Start + i);
                    _tris.Add(p1Start + next);
                    _tris.Add(p2Start + pairs[next]);

                    for (int j = pairs[i]; j != pairs[next];) {
                        _tris.Add(p2Start + j);
                        _tris.Add(p1Start + i);
                        _tris.Add(p2Start + (j = ((j + 1) % p2Size)));
                    }
                }
            }
        }

        private void BridgeSmoothLoop(int p1Start, int p2Start, bool[] dirs, int size) {
            for (int i = 0; i < size; i++) {
                int next = (i + 1) % size;
                if (dirs[i]) {
                    _tris.Add(p2Start + i);
                    _tris.Add(p1Start + i);
                    _tris.Add(p1Start + next);

                    _tris.Add(p1Start + next);
                    _tris.Add(p2Start + next);
                    _tris.Add(p2Start + i);
                } else {
                    _tris.Add(p2Start + next);
                    _tris.Add(p1Start + i);
                    _tris.Add(p1Start + next);

                    _tris.Add(p1Start + i);
                    _tris.Add(p2Start + next);
                    _tris.Add(p2Start + i);
                }
            }
        }

        private void BridgeTriLoop(Polygon p1, Polygon p2, int[] pairs, bool[] dirs, Color32 color) {
            var p1Size = p1.VertexCount;
            var p2Size = p2.VertexCount;
            for (int i = 0; i < p1Size; i++) {
                int next = (i + 1) % p1Size;
                if (dirs[i]) {
                    AddTriangle(
                        p2.GetVertex(pairs[i]),
                        p1.GetVertex(i),
                        p1.GetVertex(next),
                        color);
                    for (int j = pairs[i]; j != pairs[next];) {
                        AddTriangle(
                            p2.GetVertex(j),
                            p1.GetVertex(next),
                            p2.GetVertex(j = (j + 1) % p2Size),
                            color);
                    }
                } else {
                    AddTriangle(
                        p2.GetVertex(pairs[next]),
                        p1.GetVertex(i),
                        p1.GetVertex(next),
                        color);
                    for (int j = pairs[i]; j != pairs[next];) {
                        AddTriangle(
                            p2.GetVertex(j),
                            p1.GetVertex(i),
                            p2.GetVertex(j = (j + 1) % p2Size),
                            color);
                    }
                }
            }
        }

        private void BridgeTriLoop(Polygon p1, Polygon p2, bool[] dirs, Color32 color) {
            if (p1.VertexCount != p2.VertexCount) {
                throw new InvalidPolygonException();
            }

            var size = p1.VertexCount;
            for (int i = 0; i < size; i++) {
                int next = (i + 1) % size;
                if (dirs[i]) {
                    AddTriangle(
                        p2.GetVertex(i),
                        p1.GetVertex(i),
                        p1.GetVertex(next),
                        color);
                    AddTriangle(
                        p1.GetVertex(next),
                        p2.GetVertex(next),
                        p2.GetVertex(i),
                        color);
                } else {
                    AddTriangle(
                        p2.GetVertex(next),
                        p1.GetVertex(i),
                        p1.GetVertex(next),
                        color);
                    AddTriangle(
                        p1.GetVertex(i),
                        p2.GetVertex(next),
                        p2.GetVertex(i),
                        color);
                }
            }
        }

        private static int[] PolygonPairs(Polygon p1, Polygon p2) {
            if (p1.VertexCount > p2.VertexCount) {
                throw new System.ArgumentException();
            }
            var offset = p1.Center - p2.Center;

            var p1Size = p1.VertexCount;
            var p2Size = p2.VertexCount;

            var p1Flat = p1.FlatVertices(offset);
            var p2Flat = p2.FlatVertices(offset);

            var p2Indices = new int[p1Size];

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

            if (p1Size == p2Size) {
                for (var i = 0; i < p1Size; i++) {
                    p2Indices[i] = (smallest + i) % p2Size;
                }
                return p2Indices;
            } else {
                int avg = (p2Size / p1Size);
                float err = (p2Size / p2Size) - avg;
                float acc = 0;
                int j = smallest;

                for (var i = 0; i < p1Size; i++) {
                    p2Indices[i] = (smallest + j) % p2Size;
                    j += avg;
                    acc += err;
                    if (acc > 1) {
                        acc--;
                        j++;
                    }
                }
                return p2Indices;
            }
        }

        private static bool[] PolygonDirs(Polygon p1, Polygon p2) {
            if (p1.VertexCount != p2.VertexCount) {
                throw new System.ArgumentException();
            }
            var offset = p1.Center - p2.Center;
            var size = p1.VertexCount;

            var p2dirs = new bool[size];

            for (var i = 0; i < size; i++) {
                var edge1 = p1.GetEdge(i);
                var edge2 = p2.GetEdge(i);
                var cross = Vector3.Cross(edge1, edge2);
                p2dirs[i] = (Vector3.Dot(cross, offset) < 0);
            }

            return p2dirs;
        }

        private static bool[] PolygonDirs(Polygon p1, Polygon p2, int[] pairs) {
            var offset = p1.Center - p2.Center;
            var size = p1.VertexCount;

            var p2dirs = new bool[size];

            for (var i = 0; i < size; i++) {
                var edge1 = p1.GetEdge(i);
                var edge2 = p2.GetEdge(pairs[i]);
                var cross = Vector3.Cross(edge1, edge2);
                p2dirs[i] = (Vector3.Dot(cross, offset) < 0);
            }

            return p2dirs;
        }
    }
}