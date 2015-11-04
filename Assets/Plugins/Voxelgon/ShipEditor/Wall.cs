using UnityEngine;
using System;
using System.Collections;
using Voxelgon;
using Voxelgon.ShipEditor;

namespace Voxelgon.ShipEditor {
	public class Wall {

		//Fields

		private ArrayList vertices = new ArrayList();

		private bool verticesChanged = false;

		private Mesh wallMesh = new Mesh();
		private Plane wallPlane;

		//Methods

		public bool VerticesChanged() {
			if (!verticesChanged) return false;

			return true;
		}

		public bool AddVertex(Vector3 vertex) {
			if (vertices.Contains(vertex)) return false;

			if (vertices.Count >= 3) {
				wallPlane = new Plane((Vector3) vertices[0], (Vector3) vertices[1], (Vector3) vertices[2]);
				if (!Mathf.Approximately(0, wallPlane.GetDistanceToPoint(vertex))) return false;
			}

			vertices.Add(vertex);
			verticesChanged = true;
			return true;
		}

		public bool RemoveVertex(Vector3 vertex) {
			if (!vertices.Contains(vertex)) return false;

			vertices.Remove(vertex);
			verticesChanged = true;
			return true;
		}

		public bool ContainsVertex(Vector3 vertex) {
			return vertices.Contains(vertex);
		}

		public bool ValidVertex(Vector3 vertex) {
			if (ContainsVertex(vertex)) {
				return false;
			} else if (!IsPolygon()) {
				return true;
			} else {
				return Mathf.Approximately(0, wallPlane.GetDistanceToPoint(vertex));
			}
		}



		public bool IsPolygon() {
			return vertices.Count < 3;
		}

		public void UpdateMesh() {
			if (vertices.Count < 3) {
				wallMesh.Clear(); 
			} else if (VerticesChanged()) {
				verticesChanged = false;
				Vector3[] verts;
				verts = (Vector3[]) vertices.ToArray(typeof(Vector3));

				int triCount = (verts.Length - 2);
				int vertCount = (triCount * 3);

				Vector3[] meshVerts = new Vector3[vertCount];
				int[] meshTris = new int[vertCount];
				Vector3[] meshNorms = new Vector3[vertCount];
				Color[] meshColors = new Color[vertCount];

				for (int i = 0; i < triCount; i++) {
					meshVerts[(3 * i)] = verts[0];
					meshVerts[(3 * i) + 1] = verts[i + 1];
					meshVerts[(3 * i) + 2] = verts[i + 2];

					meshTris[(3 * i)] = (3 * i);
					meshTris[(3 * i) + 1] = (3 * i) + 1;
					meshTris[(3 * i) + 2] = (3 * i) + 2;

					meshNorms[(3 * i)] = wallPlane.normal;
					meshNorms[(3 * i) + 1] = wallPlane.normal;
					meshNorms[(3 * i) + 2] = wallPlane.normal;

					meshColors[(3 * i)] = Color.red;
					meshColors[(3 * i) + 1] = Color.red;
					meshColors[(3 * i) + 2] = Color.red;
				}

				wallMesh.vertices = meshVerts;
				wallMesh.triangles = meshTris;
				wallMesh.normals = meshNorms;
				wallMesh.colors = meshColors;
				wallMesh.Optimize();

			}
		}

		public Mesh GetMesh() {
			UpdateMesh();
			return wallMesh;
		}
	}
}
