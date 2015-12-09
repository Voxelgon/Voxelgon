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

            if(resultNormal.magnitude < 0.001f) {
                return 0; //colinear points
            }

            if(Vector3.Angle(normal, resultNormal) < 90.0f) {
                return 1; //clockwise winding order
            }

            if(Vector3.Angle(normal, resultNormal) > 90.0f) {
                return -1; //counterclockwise winding order
            }
        }

        public static int WindingOrder(List<Vector3> points, Vector3 normal) {
            return WindingOrder(points[0], points[1], points[2], normal);
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

        //Private Methods

        private static int Next(int n, int count) {
            return (n + 1) % count;
        }

        private static int Prev(int n, int count) {
            return (n + 1 + count) % count;
        }

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

        private List<int> AddPolygonSegment(List<int> vertices) {
            var result = new List<int>();
        }
    }
}