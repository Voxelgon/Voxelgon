using UnityEngine;
using System.Collections.Generic;
using Voxelgon;
using Voxelgon.Graphics;
using Voxelgon.EventSystems;
using Voxelgon.ShipEditor;

namespace Voxelgon.ShipEditor {

public class ShipEditor : MonoBehaviour, IModeChangeHandler {

		//Fields

		private Wall tempWall;
		private List<Wall> walls;

		private Dictionary<Position, List<Wall>> wallVertices;

		private List<Vector3> nodes = new List<Vector3>();
		private List<GameObject> nodeObjects = new List<GameObject>();

		private Mesh simpleHullMesh;
		private bool nodesChanged;
		private bool wallsChanged;

		private BuildMode mode = BuildMode.Polygon;

		//Properties

		public Wall TempWall {
			get { return tempWall; }
		}

		public List<Wall> Walls {
			get { return walls; }
		}

		public BuildMode Mode {
			get { return mode; }
			set { mode = value; }
		}

		public bool NodesChanged {
			get { return nodesChanged; }
		}

		public bool WallsChanged {
			get { return wallsChanged; }
		}

		public Mesh SimpleHullMesh {
			get {
				if (wallsChanged && walls.Count > 0) {
					List<Mesh> wallMeshes;
					wallMeshes = new List<Mesh>();
					foreach (Wall w in walls) {
						wallMeshes.Add(w.SimpleMesh);
					}
					simpleHullMesh.Clear();
					simpleHullMesh = Geometry.MergeMeshes(wallMeshes);
				}

				wallsChanged = false;
				return simpleHullMesh;
			}
		}

		//Enums

		public enum BuildMode {
			Polygon,
			Rectangle
		}

		//Methods

		public void OnModeChange (ModeChangeEventData eventData) {
		}

		public void Start() {
			tempWall = new Wall();
			simpleHullMesh = new Mesh();
			walls = new List<Wall>();
			wallVertices = new Dictionary<Position, List<Wall>>();

		}

		public void Update() {
			if (Input.GetButtonDown("ChangeFloor")) {
				transform.Translate(Vector3.up * 2 * (int) Input.GetAxis("ChangeFloor"));
			}
		}

		public bool AddNode(Vector3 node) {
			if (ValidNode(node)) {
				GameObject selectedNode = GameObject.CreatePrimitive(PrimitiveType.Cube);
				selectedNode.name = "selectedNode";

				MeshRenderer nodeRenderer = selectedNode.GetComponent<MeshRenderer>();
				nodeRenderer.material.shader = Shader.Find("Unlit/Color");
				nodeRenderer.material.color = ColorPallette.gridSelected;

				selectedNode.transform.parent = transform.parent;
				selectedNode.transform.localPosition = node;
				selectedNode.transform.localScale = Vector3.one * 0.15f;

				selectedNode.GetComponent<BoxCollider>().size = Vector3.one * 1.5f;
				selectedNode.AddComponent<ShipEditorGridSelected>();

				nodes.Add(node);
				nodeObjects.Add(selectedNode);
				nodesChanged = true;
				return true;
			}
			return false;
		}

		public bool RemoveNode(Vector3 node, GameObject obj) {
			if (nodes.Contains(node)) {
				nodes.Remove(node);
				nodeObjects.Remove(obj);
				nodesChanged = true;
				return true;
			}
			return false;
		}

		public bool ValidNode(Vector3 node) {
			return (tempWall.ValidVertex(node) && !ContainsNode(node));
		}

		public bool ContainsNode(Vector3 node) {
			return nodes.Contains(node);
		}

		public void AddWall(Wall wall) {
			foreach (Vector3 v in wall.Vertices) {
				var p = (Position) v;
				if (!wallVertices.ContainsKey(p)) {
					wallVertices.Add(p, new List<Wall>());
				}
				wallVertices[p].Add(wall);
			}
			walls.Add(wall);
			wallsChanged = true;
		}

		public void RemoveWall(Wall wall) {
			foreach (Vector3 v in wall.Vertices) {
				var p = (Position) v;
				if (wallVertices.ContainsKey(p)) {
					wallVertices[p].Remove(wall);

					if (wallVertices[p].Count == 0) {
						wallVertices.Remove(p);
					}
				}
			}
			walls.Remove(wall);
			wallsChanged = true;
		}

		public void FinalizeTempWall() {
			AddWall(tempWall);
			tempWall = new Wall(this);

			foreach (GameObject g in nodeObjects) {
				Destroy(g);
			}
			nodeObjects.Clear();
			nodes.Clear();

			nodesChanged = true;
		}

		public bool UpdateTempWall() {
			if (nodesChanged && tempWall.UpdateVertices(nodes, mode)) {
				nodesChanged = false;
				return true;
			}
			return false;
		}


		public List<Wall> GetWallNeighbors(Wall wall) {
			List<Wall> lastList = wallVertices[(Position) wall.Vertices[wall.VertexCount - 1]];
			List<Wall> neighbors;
			neighbors = new List<Wall>();

			foreach (Vector3 v in wall.Vertices) {
				var p = (Position) v;
				foreach(Wall w in wallVertices[p]) {
					if (lastList.Contains(w)) {
						neighbors.Add(w);
					}
					lastList = wallVertices[p];
				}
			}
			return neighbors;
		}

		public List<Wall> GetWallNeighbors(Wall wall, int edge) {
			List<Wall> l1 = wallVertices[(Position) wall.Vertices[edge]];
			List<Wall> l2 = wallVertices[(Position) wall.Vertices[(edge + 1) % wall.VertexCount]];
			List<Wall> neighbors;
			neighbors = new List<Wall>();

			foreach (Wall w in l1) {
				if (l2.Contains(w)) {
					neighbors.Add(w);
				}
			}
			return neighbors;
		}

		public static Vector3 GetEditCursorPos(float y) {
			Ray cursorRay = Camera.main.ScreenPointToRay(Input.mousePosition);

			float xySlope = cursorRay.direction.y / cursorRay.direction.x;
			float zySlope = cursorRay.direction.y / cursorRay.direction.z;

			float deltaY = cursorRay.origin.y - y;

			float xIntercept = cursorRay.origin.x + deltaY / -xySlope;
			float zIntercept = cursorRay.origin.z + deltaY / -zySlope;

			var interceptPoint = new Vector3(xIntercept, y, zIntercept);

			return interceptPoint; 
		}

		public static Vector3 GetEditCursorPos() {
			return GetEditCursorPos(0);
		}

	}
}
