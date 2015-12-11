//#define PRINTDEBUG

using UnityEngine;
using System.Collections.Generic;

namespace Voxelgon.MeshBuilder {
    public class MeshBuilder {

        //Private Fields

        private readonly string _name;
        private readonly bool _useUVs;
        private readonly bool _optimize;
        private readonly bool _calcNormals;

        private List<Vector3> _vertices;
        private List<Vector3> _normals;
        private List<Color32> _colors32;
        private List<Vector4> _tangents;
        private List<Vector2> _uvs;
        private List<int>     _tris;

        private List<Mesh> _completedMeshes;

        private bool _printDebug;


        //Constructors

        public MeshBuilder(string name = "mesh", bool useUVs = true, bool optimize = true, bool calcNormals = true) {
            _vertices = new List<Vector3>();
            _normals  = new List<Vector3>();
            _colors32 = new List<Color32>();
            _tangents = new List<Vector4>();
            _uvs      = new List<Vector2>();
            _tris     = new List<int>();

            _completedMeshes = new List<Mesh>();

            _name = name;
            _useUVs = useUVs;
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
                return  _completedMeshes;
            }
        }

        public Mesh FirstMesh {
            get {
                return AllMeshes[0];
            }
        }

        //Public Static Methods

        public static int WindingOrder(Vector3 point1, Vector3 point2, Vector3 point3, Vector3 normal) {
            Vector3 delta1 = (point2 - point1).normalized;
            Vector3 delta2 = (point3 - point1).normalized;

            Vector3 resultNormal = Vector3.Cross(delta1, delta2);

            if (resultNormal.sqrMagnitude < 0.01f) {
                return 0;
            }

            return (Vector3.Dot(normal, resultNormal) >= 0) ? 1 : -1;
        }

        public static int WindingOrder(List<Vector3> points, Vector3 normal) {
            if (points.Count < 3) {
                return 0;
            }
            
            if (points.Count == 3) {
                return WindingOrder(points[0],
                                    points[1],
                                    points[2],
                                    normal);
            }

            float sum = 0;

            for (int i = 0; i < points.Count; i++) {
                Vector3 point1 = points[i];
                Vector3 point2 = points[(i + 1) % points.Count];
                Vector3 point3 = points[(i + 2) % points.Count];

                Vector3 delta1 = (point2 - point1).normalized;
                Vector3 delta2 = (point3 - point2).normalized;

                Vector3 cross = Vector3.Cross(delta1, delta2);
                float angle = Mathf.Asin(cross.magnitude);
                if (Vector3.Dot(normal, cross) < 0) angle *= -1; 

                sum += angle;
            }

            return (sum > 0) ? 1 : -1;
        }

        //Public Methods

        public void ClearBuffer() {
            _vertices.Clear();
            _normals.Clear();
            _colors32.Clear();
            _tangents.Clear();
            _uvs.Clear();
            _tris.Clear();
        }

        public int TriangleWindingOrder(List<int> indices) {
            Vector3 normal = (_normals[indices[0]] 
                            + _normals[indices[1]]
                            + _normals[indices[2]])
                            .normalized;

            return WindingOrder(_vertices[indices[0]],
                                _vertices[indices[1]],
                                _vertices[indices[2]],
                                normal);
        }

        public int TriangleWindingOrder(int index1, int index2, int index3, Vector3 normal) {
            return WindingOrder(_vertices[index1],
                                _vertices[index2],
                                _vertices[index3],
                                normal);
        }

        public int PolygonSegment(int origin, int root, int polyStart, int polyEnd, Vector3 normal) {
            if (_printDebug) Debug.Log("Starting new polygon at " + origin);

            int index2 = polyStart;
            int index3 = polyStart + 1;

            while (index3 < polyEnd && index3 != -1) {
                if (_printDebug) Debug.Log("checking triangle possibility " + origin + " " + index2 + " " + index3);

                bool valid = true;

                if (TriangleWindingOrder(origin, index2, index3, normal) == 1) {
                    for (int i = index3 + 1; i < polyEnd; i++) {
                        valid &= !TriangleContains(origin, index2, index3, i, normal);
                    }
                } else {
                    valid = false;
                }

                if (valid) {
                    if (_printDebug) Debug.Log("valid! " + origin + " " + index2 + " " + index3);

                    AddTriangle(origin, index2, index3);

                    if (root != origin && TriangleWindingOrder(root, origin, index3, normal) == 1) {
                        return index3;
                    }

                    index2 = index3;
                    index3++;
                } else {
                    if (_printDebug) Debug.Log("invalid! " + origin + " " + index2 + " " + index3);

                    index3 = PolygonSegment(index2, root, index3, polyEnd, normal);
                }

            }
            return -1;
        }

        public void AddPolygon(List<Vector3> vertices, Vector3 normal) {
            int size = vertices.Count;
            int offset = AddVertices(vertices, normal);
            PolygonSegment(offset, offset, offset + 1, offset + size, normal);
        }

        public void AddTriangle(int vertex1, int vertex2, int vertex3) {
            _tris.Add(vertex1);
            _tris.Add(vertex2);
            _tris.Add(vertex3);
        }

        public int AddVertices(List<Vector3> vertices, Vector3 normal) {
            int size = vertices.Count;
            int offset = _vertices.Count;

            if (_vertices.Count + size > 65534) {
                //current mesh is too full to add to, finalize it and clear the buffer
                FinalizeLastMesh();
                offset = 0;
            }

            _vertices.AddRange(vertices);

            for (int i = 0; i < size; i++) {
                _normals.Add(normal);
            }

            return offset;
        }

        public bool TriangleContains(int index1, int index2, int index3, int pointIndex, Vector3 normal) {
            return (TriangleWindingOrder(index1, index2, pointIndex, normal) != -1
                 && TriangleWindingOrder(index2, index3, pointIndex, normal) != -1
                 && TriangleWindingOrder(index3, index1, pointIndex, normal) != -1);
        }

        //Private Methods

        private void FinalizeLastMesh() {
            if (_vertices.Count > 0) {
                var lastMesh = new Mesh();

                lastMesh.SetVertices(_vertices);
                lastMesh.SetNormals(_normals);
                lastMesh.SetColors(_colors32);

                if(_useUVs) {
                    lastMesh.SetTangents(_tangents);
                    lastMesh.SetUVs(0, _uvs);
                }

                lastMesh.SetTriangles(_tris, 0);
                lastMesh.RecalculateBounds();

                if(_calcNormals) {
                    lastMesh.RecalculateNormals();
                }

                if(_optimize) {
                    lastMesh.Optimize();
                }

                _completedMeshes.Add(lastMesh);
                ClearBuffer();
            }
        }

    }
}