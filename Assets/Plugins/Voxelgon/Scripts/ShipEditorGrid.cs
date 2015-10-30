using UnityEngine;
using System.Collections;
using Voxelgon;
using Voxelgon.EventSystems;

namespace Voxelgon.ShipEditor {
	public class ShipEditorGrid: MonoBehaviour {
		private MeshRenderer renderer;

		public void Start() {
			renderer = gameObject.GetComponent<MeshRenderer>();
		}

		public void Update() {

			Vector3 editCursorPosition = ShipEditor.GetEditCursorPos(transform.position.y);
			Vector3 gridPosition = editCursorPosition.Round();
			Vector3 relativeCursorPosition = editCursorPosition - gridPosition;
			
			transform.position = gridPosition;
			renderer.material.mainTextureOffset = new Vector2(relativeCursorPosition.x / 10, relativeCursorPosition.z / 10);
		}
	}
}
