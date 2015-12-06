using UnityEngine;
using Voxelgon.Math;

<<<<<<< HEAD
namespace Voxelgon.Assets.Voxelgon.ShipEditor {
    public class ShipEditorGrid: MonoBehaviour {

        //ToDo: Should this be hiding component.renderer?
=======
namespace Voxelgon.ShipEditor {
    public class ShipEditorGrid: MonoBehaviour {

>>>>>>> d2b354f9a1be5ce77b5b224eb67a7d8de87b4434
        private MeshRenderer renderer;

        public void Start() {

            renderer = gameObject.GetComponent<MeshRenderer>();
        }

        public void Update() {

            var editCursorPosition = ShipEditor.GetEditCursorPos(transform.position.y);
            var gridPosition = editCursorPosition.Round();
            var relativeCursorPosition = editCursorPosition - gridPosition;

<<<<<<< HEAD
=======
            Vector3 editCursorPosition = ShipEditor.GetEditCursorPos(transform.position.y);
            Vector3 gridPosition = editCursorPosition.Round();
            Vector3 relativeCursorPosition = editCursorPosition - gridPosition;


            
>>>>>>> d2b354f9a1be5ce77b5b224eb67a7d8de87b4434
            transform.position = gridPosition;
            renderer.material.mainTextureOffset = new Vector2(relativeCursorPosition.x / 10, relativeCursorPosition.z / 10);
        }
    }
}
