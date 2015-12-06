using UnityEngine;
using Voxelgon.Math;

namespace Voxelgon.Assets.Voxelgon.ShipEditor {
    public class ShipEditorGrid: MonoBehaviour {

        //ToDo: Should this be hiding component.renderer?
        private MeshRenderer renderer;

        public void Start() {

            renderer = gameObject.GetComponent<MeshRenderer>();
        }

        public void Update() {

            var editCursorPosition = ShipEditor.GetEditCursorPos(transform.position.y);
            var gridPosition = editCursorPosition.Round();
            var relativeCursorPosition = editCursorPosition - gridPosition;

            transform.position = gridPosition;
            renderer.material.mainTextureOffset = new Vector2(relativeCursorPosition.x / 10, relativeCursorPosition.z / 10);
        }
    }
}
