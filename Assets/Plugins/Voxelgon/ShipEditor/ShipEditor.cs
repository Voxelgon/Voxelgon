using UnityEngine;
using System.Collections;
using Voxelgon;
using Voxelgon.EventSystems;
using Voxelgon.ShipEditor;

namespace Voxelgon.ShipEditor {

public class ShipEditor : MonoBehaviour, IModeChangeHandler {

		private Wall tempWall;

		private ArrayList nodes = new ArrayList();


		public void OnModeChange (ModeChangeEventData eventData) {
		}

		public void Start() {
			tempWall = new Wall();
		}

		public void Update() {
			if (Input.GetButtonDown("ChangeFloor")) {
				transform.Translate(Vector3.up * 2 * (int) Input.GetAxis("ChangeFloor"));
			}
		}



		public bool AddNode(Vector3 node) {
			if (ValidNode(node)) {
				nodes.Add(node);
				return true;
			}
			return false;
		}

		public bool RemoveNode(Vector3 node) {
			if (nodes.Contains(node)) {
				nodes.Remove(node);
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

		public Mesh GetMesh() {
			return tempWall.GetMesh();
		}

		public static Vector3 GetEditCursorPos(float y) {
			Ray cursorRay = Camera.main.ScreenPointToRay(Input.mousePosition);

			float xySlope = cursorRay.direction.y / cursorRay.direction.x;
			float zySlope = cursorRay.direction.y / cursorRay.direction.z;

			float deltaY = cursorRay.origin.y - y;

			float xIntercept = cursorRay.origin.x + deltaY / -xySlope;
			float zIntercept = cursorRay.origin.z + deltaY / -zySlope;

			Vector3 interceptPoint = new Vector3(xIntercept, y, zIntercept);

			return interceptPoint; 
		}

		public static Vector3 GetEditCursorPos() {
			return GetEditCursorPos(0);
		}

	}
}
