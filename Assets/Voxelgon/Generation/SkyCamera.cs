using UnityEngine;

public class SkyCamera : MonoBehaviour {

    public Cubemap cubemap;
    public Material material;

    // Use this for initialization
    void Start () {
        var camera = GetComponent<Camera>();
        camera.RenderToCubemap(cubemap, 63);
        camera.targetTexture = null; // prevent free rendertexture error message
        gameObject.SetActive(false); // disable the camera and all children
    }
}
