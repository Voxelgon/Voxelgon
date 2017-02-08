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
            _vertices = new List<Vector3>();
            _normals = new List<Vector3>();
            _colors32 = new List<Color32>();
            _tris = new List<int>();

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

        public void ClearBuffer() {
            _vertices.Clear();
            _normals.Clear();
            _colors32.Clear();
            _tris.Clear();
        }

        public void AddPolygon(Polygon p) {
            int offset = AddVertices(p.Vertices, p.Normals, p.Colors);
            foreach (int i in p.ToTriangleIndices()) {
                _tris.Add(offset + i);
            }
        }

        public int AddVertices(List<Vector3> vertices, List<Vector3> normals, List<Color32> colors) {
            int size = vertices.Count;

            if (normals.Count != size && colors.Count != size) {
                throw new InvalidPolygonException();
            }

            int offset = _vertices.Count;

            if (_vertices.Count + size > 65534) {
                //current mesh is too full to add to, finalize it and clear the buffer
                FinalizeLastMesh();
                offset = 0;
            }

            _vertices.AddRange(vertices);
            _normals.AddRange(normals);
            _colors32.AddRange(colors);

            return offset;
        }

        public void BridgePolygons(Polygon p1, Polygon p2, bool smooth = true) {
            float ratio = p1.VertexCount / p2.VertexCount;
            float error = ratio;

            if (ratio > 1) {
                BridgePolygons(p2, p1, smooth);
                return;
            }

            var offset = p1.Center - p2.Center;

            p1 = p1.EnsureClockwise(offset);
            p2 = p2.EnsureClockwise(offset);

            int p1Size = p1.VertexCount;
            int p2Size = p2.VertexCount;

            var p2Indices = new int[p1Size];
            for (int i = 0; i < p1Size; i++) {
                Vector3 p1Vertex = (p1.GetVertex(i) + p1.GetVertex((i + 1) % p1Size)) / 2;
                float angle = 100;
                int index = -1;
                for (int j = 0; j < p2Size; j++) {
                    float thisAngle = Mathf.Abs(Geometry.VectorAngle(p1Vertex - p1.Center, p2.GetVertex(j) - p2.Center, offset));
                    thisAngle = (thisAngle + (Mathf.PI * 2)) % (Mathf.PI * 2);
                    if (thisAngle > 0 && thisAngle < angle) {
                        index = j;
                        angle = thisAngle;
                    }
                }
                p2Indices[i] = index;
            }

            if (smooth) {
                var p1Normals = p1.Normals;
                var p2Normals = p2.Normals;
/*
                for (int i = 0; i < p1Size; i++) {
                    var delta = p2.GetVertex(p2Indices[i]) - p1.GetVertex(i);
                    var normal = p1.GetVertexNormal(i);
                    Vector3.OrthoNormalize(ref delta, ref normal);
                    p1Normals[i] = normal;

                    for (int j = p2Indices[i]; j != p2Indices[(i + 1) % p1Size]; j = (j + 1) % p2Size) {
                        var delta2 = p1.GetVertex((j + 1) % p2Size) - p2.GetVertex(j);
                        var normal2 = p2.GetVertexNormal(j);
                        Vector3.OrthoNormalize(ref delta2, ref normal2);
                        p2Normals[j] = normal2;
                    }
                }
                */

                var vertices = new List<Vector3>();
                vertices.AddRange(p1.Vertices);
                vertices.AddRange(p2.Vertices);

                var normals = new List<Vector3>();
                normals.AddRange(p1Normals);
                normals.AddRange(p2Normals);

                var colors = new List<Color32>();
                colors.AddRange(p1.Colors);
                colors.AddRange(p2.Colors);

                int p1Start = AddVertices(vertices, normals, colors);
                int p2Start = p1Start + p1Size;

                for (int i = 0; i < p1Size; i++) {
                    _tris.Add(p1Start + i);
                    _tris.Add(p2Start + p2Indices[i]);
                    _tris.Add(p1Start + ((i + 1) % p1Size));
                    for (int j = p2Indices[i]; j != p2Indices[(i + 1) % p1Size]; j = (j + 1) % p2Size) {
                        _tris.Add(p2Start + j);
                        _tris.Add(p2Start + ((j + 1) % p2Size));
                        _tris.Add(p1Start + ((i + 1) % p1Size));
                    }
                }
            } else {
                for (int i = 0; i < p1Size; i++) {
                    AddPolygon(new Triangle(
                            p1.GetVertex(i),
                            p2.GetVertex(p2Indices[i]),
                            p1.GetVertex((i + 1) % p1Size),
                            p1.GetColor(i)
                        ));
                    for (int j = p2Indices[i]; j != p2Indices[(i + 1) % p1Size]; j = (j + 1) % p2Size) {
                        AddPolygon(new Triangle(
                                p2.GetVertex(j),
                                p2.GetVertex((j + 1) % p2Size),
                                p1.GetVertex((i + 1 + p1Size) % p1Size),
                                p1.GetColor(i)
                            ));
                    }
                }
            }
        }

        public void Extrude(Polygon p, Vector3 offset, float scale = 1.0f, bool smooth = true) {

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

        private void FinalizeLastMesh() {
            if (_vertices.Count > 0) {
                var lastMesh = new Mesh();

                lastMesh.SetVertices(_vertices);
                lastMesh.SetNormals(_normals);
                lastMesh.SetColors(_colors32);

                lastMesh.SetTriangles(_tris, 0);
                lastMesh.RecalculateBounds();

                if (_calcNormals) {
                    lastMesh.RecalculateNormals();
                }

                if (_optimize) {
                    ;
                }

                _completedMeshes.Add(lastMesh);
                ClearBuffer();
            }
        }

    }
}