using UnityEngine;

namespace Voxelgon.Ship.Editor {
    public class ShipEditorLight : MonoBehaviour {

        public ShipEditor editor;

        [RangeAttribute(0, 2)]
        public float surfaceOffset = 0.5f;
        public LayerMask mask;

		public bool dampMotion;
        [RangeAttribute(0, 0.1f)]
        public float dampTime = 0.01f;

        private Vector3 _velocity;
        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {
            var cursorRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            var floor = editor.transform.localPosition.y;
            RaycastHit hitInfo;
            Vector3 newPos;

            if (Physics.SphereCast(cursorRay.origin, surfaceOffset, cursorRay.direction, out hitInfo, float.MaxValue, mask.value)
             && hitInfo.point.y > floor) {
                newPos = hitInfo.point + hitInfo.normal * surfaceOffset;
            } else {
                newPos = transform.localToWorldMatrix * ShipEditor.CalcCursorPosition(floor);
            }

			if (dampMotion) {
				transform.position = Vector3.SmoothDamp(transform.position, newPos, ref _velocity, dampTime);
			} else {
				transform.position = newPos;
			}
        }
    }
}