using UnityEngine;
using System;
using System.Collections.Generic;
using Voxelgon.ShipEditor;

namespace Voxelgon.ShipEditor {
	public class Wall {

		//Fields

		public readonly ShipEditor Editor;

		private readonly List<Vector3> vertices = new List<Vector3>();

		private readonly Mesh simpleMesh = new Mesh();
		private readonly Mesh complexMesh = new Mesh();

		private Plane wallPlane;

		private bool verticesChanged;

		//Constructors

		public Wall() {
			wallPlane = new Plane();
			Editor = GameObject.Find("ShipEditor").GetComponent<ShipEditor>();
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
				return simpleMesh; //return something, just for testing
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

		//Methods

		public bool ValidVertex(Vector3 vertex)
        {
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

		//Private Methods

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

			if (IsPolygon) {
				for (int i = 0; i < vertices.Count; i++) {
					Vector3 normal = wallPlane.normal;
					Vector3 tangent = vertices[(i + 1) % vertices.Count] - vertices[i]; 
					tangent.Normalize();
					Vector3 binormal = Vector3.Cross(tangent, normal);

					Debug.DrawRay(vertices[i], normal, Color.blue);
					Debug.DrawRay(vertices[i], tangent, Color.yellow);
					Debug.DrawRay(vertices[i], binormal, Color.green);
				}

			}

		}
	}
}
