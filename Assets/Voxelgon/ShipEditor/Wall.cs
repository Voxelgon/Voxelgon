using UnityEngine;
using System;
using System.Collections.Generic;
using Voxelgon.Math;
using Voxelgon.ShipEditor;

namespace Voxelgon.ShipEditor {
    public class Wall {

        //Fields

        public readonly ShipEditor Editor;

        private readonly List<Vector3> vertices = new List<Vector3>();
        private readonly List<Vector3> tangents = new List<Vector3>();

        private readonly Mesh simpleMesh = new Mesh();
        private readonly Mesh complexMesh = new Mesh();

        private Plane wallPlane;

        private bool verticesChanged;

        private float thickness = 0.2f; //total thickness of the wall

        //Constructors

        public Wall() {
            wallPlane = new Plane();
        }

        public Wall(ShipEditor editor){
            wallPlane = new Plane();
            Editor = editor;
        }

        //Properties

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
            }
        }
        
        public Mesh ComplexMesh {
            get {
                BuildComplexMesh();
                if (verticesChanged) {
                    BuildSimpleMesh();
                }
                return complexMesh; //return something, just for testing
            }
        }

        public Matrix4x4 WorldToPlaneMatrix {
            get {
                var worldToPlaneMatrix = new Matrix4x4();

                var offset = -1 * vertices[0];
                var rotation = Quaternion.FromToRotation(wallPlane.normal, Vector3.up);
                var scale = Vector3.one;

                worldToPlaneMatrix.SetTRS(offset, rotation, scale);
                return worldToPlaneMatrix;
            }
        }

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
            }

            set {
                if (value < 0.0 || value > 2.0) {
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

        //Methods

        public bool ValidVertex(Vector3 vertex) {
            if (ContainsVertex(vertex)) {
                return false;
            }
            if (!IsPolygon) {
                return true;
            }
            return Mathf.Abs(wallPlane.GetDistanceToPoint(vertex)) < 0.001;
        }

        private bool ContainsVertex(Vector3 vertex) {
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
                        return Vector3.Normalize(v2 - v1);
                    }

                    //if the previous vertex is v2 (vertices in reverse order)
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
        }

        private Vector3 VectorP2W(Vector3 local) {
            return PlaneToWorldMatrix.MultiplyVector(local);
        }

        private Vector3 VectorW2P(Vector3 world) {
            return WorldToPlaneMatrix.MultiplyVector(world);
        }

        //Private Methods

        private void GenerateTangents() {
            tangents.Clear();

            if (IsPolygon) {
                for (int i = 0; i < vertices.Count; i++) {
                    tangents.Add(GetTangent(i));
                }
            }
        }

        private bool AddVertex(Vector3 vertex) {
            if (!ValidVertex(vertex)) {
                Debug.Log("Invalid vertex!");
                return false;
            }

            vertices.Add(vertex);
            verticesChanged = true;

            if (vertices.Count == 3) {
                wallPlane = new Plane(vertices[0], vertices[1], vertices[2]);
            }
            
            return true;
        }

        private void BuildSimpleMesh() {
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
                        Vector3 binormal = Vector3.Cross(tangents[i], Normal);

                        //
                        Debug.DrawRay(vertices[i], Normal, Color.blue);
                        Debug.DrawRay(vertices[i], tangents[i], Color.yellow);
                        Debug.DrawRay(vertices[i], binormal, Color.green);
                        Debug.DrawRay(vertices[i], -1 * binormal, Color.cyan);
                        //

                        Matrix4x4 tangentSpace = Matrix4x4.TRS(Vector3.zero, Quaternion.FromToRotation(tangents[i], Vector3.up), Vector3.one);
                        Vector3 tangentSpaceBinormal = tangentSpace.MultiplyVector(binormal);

                        float rootAngle = Mathf.Atan2(-1 * tangentSpaceBinormal.x, -1 * tangentSpaceBinormal.z);
                        float frontAngle = Mathf.PI * -2;
                        float backAngle = Mathf.PI * 2;

                        foreach (Wall w in neighbors) {
                            Vector3 neighborNormal = w.Normal;
                            Vector3 neighborTangent = w.GetTangent(vertices[i], vertices[(i + 1) % VertexCount]);

                            Vector3 wallBinormal = tangentSpace.MultiplyVector(Vector3.Cross(neighborTangent, neighborNormal));
                            float angle = Mathf.Atan2(wallBinormal.x, wallBinormal.z);

                            //
                            Debug.DrawRay(vertices[i], neighborTangent, Color.magenta);
                            Debug.DrawRay(vertices[i], Vector3.Cross(neighborTangent, neighborNormal), Color.gray);
                            //

                            float deltaAngle = Mathf.Repeat(rootAngle - angle, Mathf.PI * 2);
                            if (deltaAngle > Mathf.PI) deltaAngle = deltaAngle - 2 * Mathf.PI;

                            if (deltaAngle > frontAngle) frontAngle = deltaAngle;
                            if (deltaAngle < backAngle) backAngle = deltaAngle;
                        }

                        frontEdgeOffsets[i] = Mathf.Tan(-frontAngle / 2) * thickness / 2;
                        backEdgeOffsets[i] = Mathf.Tan(backAngle / 2) * thickness / 2;

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
                    Vector3 tangent = tangents[i];
                    Vector3 binormal = Vector3.Cross(tangent, Normal);
                    Vector3 localBinormal = VectorW2P(binormal);
                    Vector3 lastLocalBinormal = VectorW2P(Vector3.Cross(tangents[(i - 1 + VertexCount) % VertexCount], Normal));

                    float deltaAngle = Mathf.Atan2(localBinormal.x, localBinormal.z) - Mathf.Atan2(lastLocalBinormal.x, lastLocalBinormal.z);

                    if (Mathf.Approximately(deltaAngle, 0)){
                        frontVertices[i] = vertices[i] + (thickness / 2 * Normal) + (frontEdgeOffsets[i] * binormal);
                        backVertices[i] = vertices[i] + (thickness / -2 * Normal) + (backEdgeOffsets[i] * binormal);

                    } else {
                        float frontA = frontEdgeOffsets[i];
                        float frontB = frontEdgeOffsets[(i - 1 + VertexCount) % VertexCount];
                        float frontC = (Mathf.Cos(deltaAngle) * frontA - frontB) / Mathf.Sin(deltaAngle);
                        frontVertices[i] = vertices[i] + (thickness / 2 * Normal) + (frontA * binormal) + (frontC * tangent);

                        float backA = backEdgeOffsets[i];
                        float backB = backEdgeOffsets[(i - 1 + VertexCount) % VertexCount];
                        float backC = (Mathf.Cos(deltaAngle) * backA - backB) / Mathf.Sin(deltaAngle);
                        backVertices[i] = vertices[i] + (thickness / -2 * Normal) + (backA * binormal) + (backC * tangent);                    
                    }
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
                    Vector3 normal = Vector3.Cross(tangents[i], Normal);
                    complexVertices.Add(frontVertices[i]);
                    complexNormals.Add(normal);
                    complexVertices.Add(frontVertices[(i + 1) % VertexCount]);
                    complexNormals.Add(normal);
                    complexVertices.Add(backVertices[i]);
                    complexNormals.Add(normal);
                    complexVertices.Add(backVertices[(i + 1) % VertexCount]);
                    complexNormals.Add(normal);

                    for (int j = 0; j < 4; j++) {
                        complexColors.Add(Color.grey);    
                        Debug.DrawRay(frontVertices[i], Vector3.Cross(tangents[i], Normal));
                    }
                    complexTriangles.Add(indexOffset + 1);
                    complexTriangles.Add(indexOffset + 2);
                    complexTriangles.Add(indexOffset + 3);
                    complexTriangles.Add(indexOffset + 2);
                    complexTriangles.Add(indexOffset + 1);
                    complexTriangles.Add(indexOffset + 0);
                    indexOffset += 4;
                }

                complexMesh.SetVertices(complexVertices);
                complexMesh.SetNormals(complexNormals);
                complexMesh.SetColors(complexColors);
                complexMesh.SetTriangles(complexTriangles, 0);
                complexMesh.RecalculateBounds();
                //complexMesh.RecalculateNormals();
                //complexMesh.Optimize();
            }
        }
    }
}
