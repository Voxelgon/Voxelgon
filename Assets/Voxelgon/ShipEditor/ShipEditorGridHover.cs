using UnityEngine;
using UnityEngine.EventSystems;
using Voxelgon.Graphics;

namespace Voxelgon.ShipEditor {
    public class ShipEditorGridHover: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler{

        private BoxCollider collider;
        private ShipEditor editor;


        public void Start() {
            editor = GameObject.Find("ShipEditor").GetComponent<ShipEditor>();
            collider = gameObject.GetComponent<BoxCollider>();
        }

        public void Update() {
            transform.localPosition = new Vector3(0, 2 * Input.GetAxis("ShiftUp"), 0);
            collider.center = new Vector3(0, -2 * Input.GetAxis("ShiftUp") * (1 / transform.localScale.y), 0);
        }

        public void OnPointerEnter(PointerEventData eventData){
            MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
            renderer.enabled = true;
            renderer.material.color = ColorPallette.gridHover;
        }

        public void OnPointerExit(PointerEventData eventData){
            gameObject.GetComponent<Renderer>().enabled = false;
            
        }

        public void OnPointerClick(PointerEventData eventData) {
            Vector3 pos = editor.transform.parent.InverseTransformPoint(transform.position);

            editor.AddNode(pos);
        }
    }
}
