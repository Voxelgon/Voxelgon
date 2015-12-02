using UnityEngine;
using Voxelgon;

public class MeshColorsTest : MonoBehaviour {

    public MeshFilter filter;
    public Texture2D texture;
	// Use this for initialization
	void Start () {
        filter.mesh = Colormaps.ColorMesh(filter.mesh, texture);

	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
