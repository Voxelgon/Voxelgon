using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Voxelgon;

namespace Voxelgon.Math {
	public static class Geometry {

		//merges a list of meshes into a single mesh (limited at 65534 vertices)
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

		//public static List<Mesh> MergeMeshesWithOverflow(List<Mesh> meshes, int vertexMax) {



	}
}