using UnityEngine;
using UnityEngine.EventSystems;
using Voxelgon.Graphics;
using Voxelgon.Util.Grid;

namespace Voxelgon.Ship.Editor {
    public class ShipEditorGridSelected: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler{

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

        public void OnPointerClick(PointerEventData eventData) {
            if (eventData.button == PointerEventData.InputButton.Right) {
                //editor.RemoveNode((GridVector) transform.localPosition, gameObject);
                //GameObject.Destroy(gameObject,0.0f);
            } else if (eventData.button == PointerEventData.InputButton.Left && eventData.clickCount > 1) {
                editor.FinalizeTempWall();
            }
        }
    }
}
