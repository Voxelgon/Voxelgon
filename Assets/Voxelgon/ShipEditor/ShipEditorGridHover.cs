using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using Voxelgon;
using Voxelgon.UI;
using Voxelgon.EventSystems;

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
			Vector3 pos = transform.parent.parent.InverseTransformPoint(transform.position);

			if (editor.AddNode(pos)) {
				GameObject selectedNode = GameObject.CreatePrimitive(PrimitiveType.Cube);
				selectedNode.name = "selectedNode";

				MeshRenderer nodeRenderer = selectedNode.GetComponent<MeshRenderer>();
				nodeRenderer.material.shader = Shader.Find("Unlit/Color");
				nodeRenderer.material.color = ColorPallette.gridSelected;

				selectedNode.transform.parent = transform.parent.parent.parent;
				selectedNode.transform.localPosition = pos;
				selectedNode.transform.localScale = Vector3.one * 0.15f;

				selectedNode.GetComponent<BoxCollider>().size = Vector3.one * 1.5f;


				selectedNode.AddComponent<ShipEditorGridSelected>();
			}

		}
	}
}
