using UnityEngine;
using UnityEngine.EventSystems;
using Voxelgon.Graphics;

<<<<<<< HEAD
namespace Voxelgon.Assets.Voxelgon.ShipEditor {
	public class ShipEditorGridSelected: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler{
=======
namespace Voxelgon.ShipEditor {
    public class ShipEditorGridSelected: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler{
>>>>>>> d2b354f9a1be5ce77b5b224eb67a7d8de87b4434

        private ShipEditor editor;

        public void Start() {
            editor = GameObject.Find("ShipEditor").GetComponent<ShipEditor>();
        }

        public void Update() {
        }

        public void OnPointerEnter(PointerEventData eventData){
            gameObject.GetComponent<Renderer>().material.color = ColorPallette.gridSelectedHover;
        }

        public void OnPointerExit(PointerEventData eventData){
            gameObject.GetComponent<Renderer>().material.color = ColorPallette.gridSelected;
            
        }

<<<<<<< HEAD
		public void OnPointerClick(PointerEventData eventData) {
			if (eventData.button == PointerEventData.InputButton.Right) {
				editor.RemoveNode(transform.localPosition, gameObject);
				Destroy(gameObject,0.0f);
			} else if (eventData.button == PointerEventData.InputButton.Left && eventData.clickCount > 1) {
				editor.FinalizeTempWall();
			}
		}
	}
=======
        public void OnPointerClick(PointerEventData eventData) {
            if (eventData.button == PointerEventData.InputButton.Right) {
                editor.RemoveNode(transform.localPosition, gameObject);
                GameObject.Destroy(gameObject,0.0f);
            } else if (eventData.button == PointerEventData.InputButton.Left && eventData.clickCount > 1) {
                editor.FinalizeTempWall();
            }
        }
    }
>>>>>>> d2b354f9a1be5ce77b5b224eb67a7d8de87b4434
}
