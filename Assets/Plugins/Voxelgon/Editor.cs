using UnityEngine;
using System.Collections;

namespace Voxelgon {
	public static class Editor {
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