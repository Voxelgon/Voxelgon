using UnityEngine;
using System.Collections;
using Voxelgon;
using Voxelgon.ShipEditor;

public class ShipEditorMeshPreview : MonoBehaviour {

	private Mesh previewMesh; 

	private MeshFilter filter;

	// Use this for initialization
	void Start () {
		filter = gameObject.GetComponent<MeshFilter>();
	}
	
	// Update is called once per frame
	void Update () {
		filter.mesh = ShipEditor.previewWall.GetMesh();

	}
	
}
