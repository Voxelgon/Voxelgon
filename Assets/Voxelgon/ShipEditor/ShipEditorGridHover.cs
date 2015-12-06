using UnityEngine;
using UnityEngine.EventSystems;
using Voxelgon.Graphics;

<<<<<<< HEAD
namespace Voxelgon.Assets.Voxelgon.ShipEditor {
    public class ShipEditorGridHover: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler{

        //ToDo: Should this be hiding Componenet.collider or should it be a 'new' collider?
=======
namespace Voxelgon.ShipEditor {
    public class ShipEditorGridHover: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler{

>>>>>>> d2b354f9a1be5ce77b5b224eb67a7d8de87b4434
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
<<<<<<< HEAD
            //ToDo: Should this be hiding Component.renderer? 
            var renderer = gameObject.GetComponent<MeshRenderer>();
=======
            MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
>>>>>>> d2b354f9a1be5ce77b5b224eb67a7d8de87b4434
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
