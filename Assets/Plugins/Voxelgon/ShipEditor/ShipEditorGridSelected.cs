using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using Voxelgon;
using Voxelgon.UI;
using Voxelgon.EventSystems;

namespace Voxelgon.ShipEditor {
	public class ShipEditorGridSelected: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler{

		public void Start() {
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
				ShipEditor.previewWall.RemoveVertex(transform.localPosition);
				GameObject.Destroy(gameObject,0.0f);
			}
		}
	}
}
