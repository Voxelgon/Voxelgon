using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemCamera : MonoBehaviour {

	private Camera _camera;
	public float SystemScale;

	// Use this for initialization
	void Start () {
		_camera = Camera.main;
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
		transform.position = _camera.transform.position * SystemScale;
		transform.rotation = _camera.transform.rotation;
	}
}
