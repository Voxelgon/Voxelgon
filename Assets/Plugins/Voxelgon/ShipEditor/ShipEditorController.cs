using UnityEngine;
using System.Collections;
using Voxelgon;
using Voxelgon.EventSystems;
using Voxelgon.ShipEditor;

namespace Voxelgon.ShipEditor {

public class ShipEditorController : MonoBehaviour, IModeChangeHandler {
		public void OnModeChange (ModeChangeEventData eventData) {
		}

		public void Update() {
			if (Input.GetButtonDown("ChangeFloor")) {
				transform.Translate(Vector3.up * 2 * (int) Input.GetAxis("ChangeFloor"));
			}
		}

		public static void Foo() {}
	}
}
