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

            if(Mathf.Abs(Vector3.Dot(delta1, delta2)) < 0.001f) {
                return 0; //colinear points
            }

            Vector3 resultNormal = Vector3.Cross(delta1, delta2).normalized;

            return (Vector3.Dot(normal, resultNormal) >= 0) ? 1 : -1;
        }

        public static int WindingOrder(List<Vector3> points, Vector3 normal) {
            if (points.Count <= 3) {
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

        private List<int> AddPolygonSegment(List<int> vertices) {
            var result = new List<int>();
        }
    }
}