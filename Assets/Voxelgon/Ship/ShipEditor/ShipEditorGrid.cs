using UnityEngine;
using Voxelgon.Util.Geometry;

namespace Voxelgon.Ship.Editor {
    public class ShipEditorGrid: MonoBehaviour {

        private MeshRenderer renderer;
        public GameObject childlight;

        public void Start() {

            renderer = gameObject.GetComponent<MeshRenderer>();
        }

        public void Update() {


            Vector3 editCursorPosition = ShipEditor.GetEditCursorPos(transform.position.y);
            Vector3 gridPosition = editCursorPosition.Round();
            Vector3 relativeCursorPosition = editCursorPosition - gridPosition;


            
            transform.position = gridPosition;
            childlight.transform.localPosition = relativeCursorPosition;
            renderer.material.mainTextureOffset = new Vector2(relativeCursorPosition.x / 10, relativeCursorPosition.z / 10);
        }
    }
}
