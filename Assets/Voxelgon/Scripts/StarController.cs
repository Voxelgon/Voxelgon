using UnityEngine;

public class StarController : MonoBehaviour {


	public void Begin() {
		var map = new Cubemap(1024, TextureFormat.RGB24, false);
		if (gameObject.GetComponent<Camera>().RenderToCubemap(map)) {
			Camera.main.gameObject.GetComponent<Skybox>().material.SetTexture("_Tex", map);
		}
		Destroy(gameObject);
	}

	public void Start() {
		Invoke("Begin", 0.1f);
	}
}