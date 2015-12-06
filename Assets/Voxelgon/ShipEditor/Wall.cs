using System;
using System.Collections.Generic;
<<<<<<< HEAD
using System.Linq;
using UnityEngine;
using Voxelgon.Assets.Voxelgon.Math;

namespace Voxelgon.Assets.Voxelgon.ShipEditor {
=======
using Voxelgon.Math;
using Voxelgon.ShipEditor;

namespace Voxelgon.ShipEditor {
>>>>>>> d2b354f9a1be5ce77b5b224eb67a7d8de87b4434
    public class Wall {

        //Fields

        public readonly ShipEditor Editor;

<<<<<<< HEAD
        private readonly Mesh _simpleMesh = new Mesh();
        private readonly Mesh _complexMesh = new Mesh();

        private Plane _wallPlane;

        private float _thickness = 0.2f; //total _thickness of the wall
=======
        private readonly List<Vector3> vertices = new List<Vector3>();
        private readonly List<Vector3> tangents = new List<Vector3>();

        private readonly Mesh simpleMesh = new Mesh();
        private readonly Mesh complexMesh = new Mesh();

        private Plane wallPlane;

        private bool verticesChanged;

        private float thickness = 0.2f; //total thickness of the wall
>>>>>>> d2b354f9a1be5ce77b5b224eb67a7d8de87b4434

        //Constructors

        public Wall() {
<<<<<<< HEAD
            _wallPlane = new Plane();
=======
            wallPlane = new Plane();
>>>>>>> d2b354f9a1be5ce77b5b224eb67a7d8de87b4434
            Editor = GameObject.Find("ShipEditor").GetComponent<ShipEditor>();
        }

        public Wall(ShipEditor editor){
<<<<<<< HEAD
            _wallPlane = new Plane();
=======
            wallPlane = new Plane();
>>>>>>> d2b354f9a1be5ce77b5b224eb67a7d8de87b4434
            Editor = editor;
        }

        //Properties

<<<<<<< HEAD
        public int VertexCount => Vertices.Count;

        public bool IsPolygon => Vertices.Count > 2;

        public List<Vector3> Vertices { get; } = new List<Vector3>();

        public List<Vector3> Tangents { get; } = new List<Vector3>();

        public Mesh SimpleMesh {
            get {
                if (VerticesChanged) {
                    BuildSimpleMesh();
                }
                return _simpleMesh;
=======
        public int VertexCount {
            get { return vertices.Count; }
        }

        public bool IsPolygon {
            get { return vertices.Count > 2; }
        }

        public List<Vector3> Vertices {
            get { return vertices; }
        }

        public List<Vector3> Tangents {
            get { 
                return tangents;
            }
        }

        public Mesh SimpleMesh {
            get {
                if (verticesChanged) {
                    BuildSimpleMesh();
                }
                return simpleMesh;
>>>>>>> d2b354f9a1be5ce77b5b224eb67a7d8de87b4434
            }
        }
        
        public Mesh ComplexMesh {
            get {
                BuildComplexMesh();
<<<<<<< HEAD
                if (VerticesChanged) {
                    BuildSimpleMesh();
                }
                return _complexMesh; //return something, just for testing
=======
                if (verticesChanged) {
                    BuildSimpleMesh();
                }
                return complexMesh; //return something, just for testing
>>>>>>> d2b354f9a1be5ce77b5b224eb67a7d8de87b4434
            }
        }

        public Matrix4x4 WorldToPlaneMatrix {
            get {
                var worldToPlaneMatrix = new Matrix4x4();

<<<<<<< HEAD
                var offset = -1 * Vertices[0];
                var rotation = Quaternion.FromToRotation(_wallPlane.normal, Vector3.up);
=======
                var offset = -1 * vertices[0];
                var rotation = Quaternion.FromToRotation(wallPlane.normal, Vector3.up);
>>>>>>> d2b354f9a1be5ce77b5b224eb67a7d8de87b4434
                var scale = Vector3.one;

                worldToPlaneMatrix.SetTRS(offset, rotation, scale);
                return worldToPlaneMatrix;
            }
        }

<<<<<<< HEAD
        public Matrix4x4 PlaneToWorldMatrix => WorldToPlaneMatrix.inverse;

        public bool VerticesChanged { get; private set; }

        public float Thickness {
            get { 
                return _thickness;
=======
        public Matrix4x4 PlaneToWorldMatrix {
            get {
                return WorldToPlaneMatrix.inverse;
            }
        }

        public bool VerticesChanged {
            get { return verticesChanged; }
        }

        public float Thickness {
            get { 
                return thickness;
>>>>>>> d2b354f9a1be5ce77b5b224eb67a7d8de87b4434
            }

            set {
                if (value < 0.0 || value > 2.0) {
<<<<<<< HEAD
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                _thickness = value;
            }
        }

        public Vector3 Normal => IsPolygon ? _wallPlane.normal : Vector3.zero;
=======
                    throw new ArgumentOutOfRangeException("value");
                }
                thickness = value;
            }
        }

        public Vector3 Normal {
            get {
                if (IsPolygon) {
                    return wallPlane.normal;
                } 
                return Vector3.zero;
            }
        }
>>>>>>> d2b354f9a1be5ce77b5b224eb67a7d8de87b4434

        //Methods

        public bool ValidVertex(Vector3 vertex) {
            if (ContainsVertex(vertex)) {
                return false;
            }
            if (!IsPolygon) {
                return true;
            }
            return Mathf.Abs(_wallPlane.GetDistanceToPoint(vertex)) < 0.001;
        }

        private bool ContainsVertex(Vector3 vertex) {
<<<<<<< HEAD
            return Vertices.Contains(vertex);
        }

        public bool UpdateVertices(List<Vector3> nodes, ShipEditor.BuildMode mode) {
            if (mode != ShipEditor.BuildMode.Polygon) return false;

            Vertices.Clear();
            return nodes.All(AddVertex);
        }

        public Vector3 GetTangent(Vector3 vertex) {
            for (var i = 0; i < Vertices.Count; i++) {
                if ((Position) Vertices[i] == (Position) vertex) {
                    return GetTangent(i);
                }
            }
            throw new ArgumentException("given vertex is not present in this wall", nameof(vertex));
        }

        public Vector3 GetTangent(Vector3 v1, Vector3 v2) {
            for (int i = 0; i < Vertices.Count; i++) {
                if ((Position) Vertices[i] == (Position) v1) {
                    //if the next vertex is v2 (vertices in correct order)

                    if ((Position) Vertices[(i + 1) % Vertices.Count] == (Position) v2) {
=======
            return vertices.Contains(vertex);
        }

        public bool UpdateVertices(List<Vector3> nodes, ShipEditor.BuildMode mode) {
            if (mode == ShipEditor.BuildMode.Polygon) {
                vertices.Clear();
                foreach(Vector3 node in nodes) {
                    if (!AddVertex(node)) {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public Vector3 GetTangent(Vector3 vertex) {
            for (int i = 0; i < vertices.Count; i++) {
                if ((Position) vertices[i] == (Position) vertex) {
                    return GetTangent(i);
                }
            }
            throw new ArgumentException("given vertex is not present in this wall", "vertex");
        }

        public Vector3 GetTangent(Vector3 v1, Vector3 v2) {
            for (int i = 0; i < vertices.Count; i++) {
                if ((Position) vertices[i] == (Position) v1) {
                    //if the next vertex is v2 (vertices in correct order)

                    if ((Position) vertices[(i + 1) % vertices.Count] == (Position) v2) {
>>>>>>> d2b354f9a1be5ce77b5b224eb67a7d8de87b4434
                        return Vector3.Normalize(v2 - v1);
                    }

                    //if the previous vertex is v2 (vertices in reverse order)
<<<<<<< HEAD
                    if ((Position) Vertices[(i - 1 + Vertices.Count) % Vertices.Count] == (Position) v2) {
                        return Vector3.Normalize(v1 - v2);
                    }
                    throw new ArgumentException("given vertex is not present in this wall", nameof(v2));
                }
            } 
            throw new ArgumentException("given vertex is not present in this wall", nameof(v1));
        }

        public Vector3 GetTangent(int index) {
            if (index >= Vertices.Count) {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            return Vector3.Normalize(Vertices[(index + 1) % Vertices.Count] - Vertices[index]);
=======
                    if ((Position) vertices[(i - 1 + vertices.Count) % vertices.Count] == (Position) v2) {
                        return Vector3.Normalize(v1 - v2);
                    }
                    throw new ArgumentException("given vertex is not present in this wall", "v2");
                }
            } 
            throw new ArgumentException("given vertex is not present in this wall", "v1");
        }

        public Vector3 GetTangent(int index) {
            if (index >= vertices.Count) {
                throw new ArgumentOutOfRangeException("index");
            }
            return Vector3.Normalize(vertices[(index + 1) % vertices.Count] - vertices[index]);
>>>>>>> d2b354f9a1be5ce77b5b224eb67a7d8de87b4434
        }

        private Vector3 VectorP2W(Vector3 local) {
            return PlaneToWorldMatrix.MultiplyVector(local);
        }

        private Vector3 VectorW2P(Vector3 world) {
            return WorldToPlaneMatrix.MultiplyVector(world);
        }

        //Private Methods

        private void GenerateTangents() {
<<<<<<< HEAD
            Tangents.Clear();

            if (!IsPolygon) return;

            for (var i = 0; i < Vertices.Count; i++) {
                Tangents.Add(GetTangent(i));
=======
            tangents.Clear();

            if (IsPolygon) {
                for (int i = 0; i < vertices.Count; i++) {
                    tangents.Add(GetTangent(i));
                }
>>>>>>> d2b354f9a1be5ce77b5b224eb67a7d8de87b4434
            }
        }

        private bool AddVertex(Vector3 vertex) {
            if (!ValidVertex(vertex)) {
                Debug.Log("Invalid vertex!");
                return false;
            }

<<<<<<< HEAD
            Vertices.Add(vertex);
            VerticesChanged = true;

            if (Vertices.Count == 3) {
                _wallPlane = new Plane(Vertices[0], Vertices[1], Vertices[2]);
=======
            vertices.Add(vertex);
            verticesChanged = true;

            if (vertices.Count == 3) {
                wallPlane = new Plane(vertices[0], vertices[1], vertices[2]);
>>>>>>> d2b354f9a1be5ce77b5b224eb67a7d8de87b4434
            }
            
            return true;
        }

        private void BuildSimpleMesh() {
<<<<<<< HEAD
            _simpleMesh.Clear();

            if (!IsPolygon) return;

            var triCountSimple = 3 * (VertexCount - 2);
            var vertCountSimple = VertexCount;

            var meshVerts = new Vector3[vertCountSimple];
            var meshTris = new int[triCountSimple];
            var meshNorms = new Vector3[vertCountSimple];
            var meshColors = new Color[vertCountSimple];

            for (var i = 0; 3 * i < triCountSimple; i ++) {
                meshTris[3 * i] = 0;
                meshTris[3 * i + 1] = i + 1;
                meshTris[3 * i + 2] = i + 2;
            }

            for (var i = 0; i < vertCountSimple; i++) {
                meshColors[i] = Color.red;
                meshNorms[i] = _wallPlane.normal;
            }

            _simpleMesh.vertices = Vertices.ToArray();
            _simpleMesh.triangles = meshTris;
            _simpleMesh.normals = meshNorms;
            _simpleMesh.colors = meshColors;
            _simpleMesh.Optimize();

            VerticesChanged = false;
        }

        private void BuildComplexMesh() {
            _complexMesh.Clear();
=======
            simpleMesh.Clear();
                    
            if (IsPolygon) {

                int triCountSimple = 3 * (VertexCount - 2);
                int vertCountSimple = VertexCount;

                var meshVerts = new Vector3[vertCountSimple];
                var meshTris = new int[triCountSimple];
                var meshNorms = new Vector3[vertCountSimple];
                var meshColors = new Color[vertCountSimple];

                for (int i = 0; 3 * i < triCountSimple; i ++) {
                    meshTris[3 * i] = 0;
                    meshTris[3 * i + 1] = i + 1;
                    meshTris[3 * i + 2] = i + 2;
                }

                for (int i = 0; i < vertCountSimple; i++) {
                    meshColors[i] = Color.red;
                    meshNorms[i] = wallPlane.normal;
                }

                simpleMesh.vertices = vertices.ToArray();
                simpleMesh.triangles = meshTris;
                simpleMesh.normals = meshNorms;
                simpleMesh.colors = meshColors;
                simpleMesh.Optimize();

                verticesChanged = false;
            }
        }

        private void BuildComplexMesh() {
            complexMesh.Clear();
>>>>>>> d2b354f9a1be5ce77b5b224eb67a7d8de87b4434
            GenerateTangents();

            if (IsPolygon) {
                var frontEdgeOffsets = new float[VertexCount];
                var backEdgeOffsets  = new float[VertexCount];
                var frontVertices = new Vector3[VertexCount];
                var backVertices  = new Vector3[VertexCount];
                var caps = new List<int>();

                for (int i = 0; i < VertexCount; i++) {
                    List<Wall> neighbors = Editor.GetWallNeighbors(this, i);

                    if (neighbors.Count != 0) {
<<<<<<< HEAD
                        Vector3 binormal = Vector3.Cross(Tangents[i], Normal);

                        //
                        Debug.DrawRay(Vertices[i], Normal, Color.blue);
                        Debug.DrawRay(Vertices[i], Tangents[i], Color.yellow);
                        Debug.DrawRay(Vertices[i], binormal, Color.green);
                        Debug.DrawRay(Vertices[i], -1 * binormal, Color.cyan);
                        //

                        Matrix4x4 tangentSpace = Matrix4x4.TRS(Vector3.zero, Quaternion.FromToRotation(Tangents[i], Vector3.up), Vector3.one);
=======
                        Vector3 binormal = Vector3.Cross(tangents[i], Normal);

                        //
                        Debug.DrawRay(vertices[i], Normal, Color.blue);
                        Debug.DrawRay(vertices[i], tangents[i], Color.yellow);
                        Debug.DrawRay(vertices[i], binormal, Color.green);
                        Debug.DrawRay(vertices[i], -1 * binormal, Color.cyan);
                        //

                        Matrix4x4 tangentSpace = Matrix4x4.TRS(Vector3.zero, Quaternion.FromToRotation(tangents[i], Vector3.up), Vector3.one);
>>>>>>> d2b354f9a1be5ce77b5b224eb67a7d8de87b4434
                        Vector3 tangentSpaceBinormal = tangentSpace.MultiplyVector(binormal);

                        float rootAngle = Mathf.Atan2(-1 * tangentSpaceBinormal.x, -1 * tangentSpaceBinormal.z);
                        float frontAngle = Mathf.PI * -2;
                        float backAngle = Mathf.PI * 2;

                        foreach (Wall w in neighbors) {
                            Vector3 neighborNormal = w.Normal;
<<<<<<< HEAD
                            Vector3 neighborTangent = w.GetTangent(Vertices[i], Vertices[(i + 1) % VertexCount]);
=======
                            Vector3 neighborTangent = w.GetTangent(vertices[i], vertices[(i + 1) % VertexCount]);
>>>>>>> d2b354f9a1be5ce77b5b224eb67a7d8de87b4434

                            Vector3 wallBinormal = tangentSpace.MultiplyVector(Vector3.Cross(neighborTangent, neighborNormal));
                            float angle = Mathf.Atan2(wallBinormal.x, wallBinormal.z);

                            //
<<<<<<< HEAD
                            Debug.DrawRay(Vertices[i], neighborTangent, Color.magenta);
                            Debug.DrawRay(Vertices[i], Vector3.Cross(neighborTangent, neighborNormal), Color.gray);
=======
                            Debug.DrawRay(vertices[i], neighborTangent, Color.magenta);
                            Debug.DrawRay(vertices[i], Vector3.Cross(neighborTangent, neighborNormal), Color.gray);
>>>>>>> d2b354f9a1be5ce77b5b224eb67a7d8de87b4434
                            //

                            float deltaAngle = Mathf.Repeat(rootAngle - angle, Mathf.PI * 2);
                            if (deltaAngle > Mathf.PI) deltaAngle = deltaAngle - 2 * Mathf.PI;

                            if (deltaAngle > frontAngle) frontAngle = deltaAngle;
                            if (deltaAngle < backAngle) backAngle = deltaAngle;
                        }

<<<<<<< HEAD
                        frontEdgeOffsets[i] = Mathf.Tan(-frontAngle / 2) * _thickness / 2;
                        backEdgeOffsets[i] = Mathf.Tan(backAngle / 2) * _thickness / 2;
=======
                        frontEdgeOffsets[i] = Mathf.Tan(-frontAngle / 2) * thickness / 2;
                        backEdgeOffsets[i] = Mathf.Tan(backAngle / 2) * thickness / 2;
>>>>>>> d2b354f9a1be5ce77b5b224eb67a7d8de87b4434

                        if (!Mathf.Approximately(frontAngle, 0) && !Mathf.Approximately(backAngle, 0)) {
                            caps.Add(i);
                        }
                    } else {
                        frontEdgeOffsets[i] = 0;
                        backEdgeOffsets[i] = 0;
                        caps.Add(i);
                    }
                }

                // build frontVertices and BackVertices arrays
                for (int i = 0; i < VertexCount; i++) {
<<<<<<< HEAD
                    Vector3 tangent = Tangents[i];
                    Vector3 binormal = Vector3.Cross(tangent, Normal);
                    Vector3 localBinormal = VectorW2P(binormal);
                    Vector3 lastLocalBinormal = VectorW2P(Vector3.Cross(Tangents[(i - 1 + VertexCount) % VertexCount], Normal));
=======
                    Vector3 tangent = tangents[i];
                    Vector3 binormal = Vector3.Cross(tangent, Normal);
                    Vector3 localBinormal = VectorW2P(binormal);
                    Vector3 lastLocalBinormal = VectorW2P(Vector3.Cross(tangents[(i - 1 + VertexCount) % VertexCount], Normal));
>>>>>>> d2b354f9a1be5ce77b5b224eb67a7d8de87b4434

                    float deltaAngle = Mathf.Atan2(localBinormal.x, localBinormal.z) - Mathf.Atan2(lastLocalBinormal.x, lastLocalBinormal.z);

                    float frontA = frontEdgeOffsets[i];
                    float frontB = frontEdgeOffsets[(i - 1 + VertexCount) % VertexCount];
                    float frontC = (Mathf.Cos(deltaAngle) * frontA - frontB) / Mathf.Sin(deltaAngle);
<<<<<<< HEAD
                    frontVertices[i] = Vertices[i] + (_thickness / 2 * Normal) + (frontA * binormal) + (frontC * tangent);
=======
                    frontVertices[i] = vertices[i] + (thickness / 2 * Normal) + (frontA * binormal) + (frontC * tangent);
>>>>>>> d2b354f9a1be5ce77b5b224eb67a7d8de87b4434

                    float backA = backEdgeOffsets[i];
                    float backB = backEdgeOffsets[(i - 1 + VertexCount) % VertexCount];
                    float backC = (Mathf.Cos(deltaAngle) * backA - backB) / Mathf.Sin(deltaAngle);
<<<<<<< HEAD
                    backVertices[i] = Vertices[i] + (_thickness / -2 * Normal) + (backA * binormal) + (backC * tangent);					
=======
                    backVertices[i] = vertices[i] + (thickness / -2 * Normal) + (backA * binormal) + (backC * tangent);                    
>>>>>>> d2b354f9a1be5ce77b5b224eb67a7d8de87b4434
                }

                var complexVertices = new List<Vector3>();
                var complexNormals = new List<Vector3>();
                var complexColors = new List<Color>();
                var complexTriangles = new List<int>();

                int indexOffset = 0;

                for (int i = 0; i < VertexCount; i++) {
                    complexVertices.Add(frontVertices[i]);
                    complexNormals.Add(Normal);
                    complexColors.Add(Color.grey);
                    if (i >= 2) {
                        complexTriangles.Add(0);
                        complexTriangles.Add(i - 1);
                        complexTriangles.Add(i);
                    }
                }

                indexOffset += VertexCount;

                for (int i = 0; i < VertexCount; i++) {
                    complexVertices.Add(backVertices[i]);
                    complexNormals.Add(-1 * Normal);
                    complexColors.Add(Color.grey);
                    if (i >= 2) {
                        complexTriangles.Add(indexOffset + i);
                        complexTriangles.Add(indexOffset + i - 1);
                        complexTriangles.Add(indexOffset + 0);
                    }
                }

                indexOffset += VertexCount;

                foreach (int i in caps) {
<<<<<<< HEAD
                    Vector3 normal = Vector3.Cross(Tangents[i], Normal);
=======
                    Vector3 normal = Vector3.Cross(tangents[i], Normal);
>>>>>>> d2b354f9a1be5ce77b5b224eb67a7d8de87b4434
                    complexVertices.Add(frontVertices[i]);
                    complexNormals.Add(normal);
                    complexVertices.Add(frontVertices[(i + 1) % VertexCount]);
                    complexNormals.Add(normal);
                    complexVertices.Add(backVertices[i]);
                    complexNormals.Add(normal);
                    complexVertices.Add(backVertices[(i + 1) % VertexCount]);
                    complexNormals.Add(normal);

                    for (int j = 0; j < 4; j++) {
<<<<<<< HEAD
                        complexColors.Add(Color.grey);	
                        Debug.DrawRay(frontVertices[i], Vector3.Cross(Tangents[i], Normal));
=======
                        complexColors.Add(Color.grey);    
                        Debug.DrawRay(frontVertices[i], Vector3.Cross(tangents[i], Normal));
>>>>>>> d2b354f9a1be5ce77b5b224eb67a7d8de87b4434
                    }
                    complexTriangles.Add(indexOffset + 1);
                    complexTriangles.Add(indexOffset + 2);
                    complexTriangles.Add(indexOffset + 3);
                    complexTriangles.Add(indexOffset + 2);
                    complexTriangles.Add(indexOffset + 1);
                    complexTriangles.Add(indexOffset + 0);
                    indexOffset += 4;
                }

<<<<<<< HEAD
                _complexMesh.SetVertices(complexVertices);
                _complexMesh.SetNormals(complexNormals);
                _complexMesh.SetColors(complexColors);
                _complexMesh.SetTriangles(complexTriangles, 0);
                _complexMesh.RecalculateBounds();
                //_complexMesh.RecalculateNormals();
                //_complexMesh.Optimize();
=======
                complexMesh.SetVertices(complexVertices);
                complexMesh.SetNormals(complexNormals);
                complexMesh.SetColors(complexColors);
                complexMesh.SetTriangles(complexTriangles, 0);
                complexMesh.RecalculateBounds();
                //complexMesh.RecalculateNormals();
                //complexMesh.Optimize();
>>>>>>> d2b354f9a1be5ce77b5b224eb67a7d8de87b4434
            }
        }
    }
}
