using UnityEngine;

public class StarController : MonoBehaviour {


	public void Begin() {
		var map = new Cubemap(2048, TextureFormat.RGB24, true);
		if (gameObject.GetComponent<Camera>().RenderToCubemap(map)) {
			map.SmoothEdges(200);
			Camera.main.gameObject.GetComponent<Skybox>().material.SetTexture("_Tex", map);
		}
		Destroy(gameObject);
	}

	public void Start() {
		Invoke("Begin", 0.1f);
	}
}