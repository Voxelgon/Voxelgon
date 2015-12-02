using UnityEngine;
using Voxelgon.ShipEditor;

public class ShipEditorMeshPreview : MonoBehaviour {

	private MeshFilter filter;
	private ShipEditor editor;


	// Use this for initialization
	void Start () {
		filter = gameObject.GetComponent<MeshFilter>();
		editor = GameObject.Find("ShipEditor").GetComponent<ShipEditor>();
	}
	
	// Update is called once per frame
	void Update () {
		editor.UpdateTempWall();
		filter.mesh = editor.TempWall.ComplexMesh;

	}
	
}
