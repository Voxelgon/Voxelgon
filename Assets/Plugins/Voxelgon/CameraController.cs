using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public float panSensitivity = 0.8f;
	public float orbitSensitivity = 3f;
	public float zoomSensativity = 1f;
	public float zoomExpo = 1.1f;

	public float minAltitude = 5;
	public float maxAltitude = 85;

	public float minZoom = 1;
	public float maxZoom = 1000;

	private float zoom = 10;
	private float altitude = 45;

	private Quaternion altitudeQuat = new Quaternion();

	private Matrix4x4 orthoSpace = new Matrix4x4();

	// Update is called once per frame
	void Update () {

		////Camera movement////

		//set values of temporary variables
		altitude = transform.parent.eulerAngles.x ;
		zoom = -transform.localPosition.z ;
		orthoSpace = Matrix4x4.TRS(Vector3.one, Quaternion.Euler(-altitude,0,0), Vector3.one);

		//pan camera (default to RMB)
		if (Input.GetAxis("Pan") == 1) {
			transform.parent.Translate(orthoSpace.MultiplyVector(panSensitivity * new Vector3(Input.GetAxis("Mouse X"), 0, Input.GetAxis("Mouse Y"))));
		}

		//orbit camera (default to MMB)
		if (Input.GetAxis("Orbit") == 1) {
			transform.parent.Rotate(Vector3.up * orbitSensitivity * Input.GetAxis("Mouse X"),Space.World);
			transform.parent.Rotate(Vector3.right * (Mathf.Clamp(orbitSensitivity * Input.GetAxis("Mouse Y"), minAltitude - altitude, maxAltitude - altitude)));
		}

		//zoom camera (default to Scroll Wheel)
		transform.Translate( Vector3.back * Mathf.Clamp(Input.GetAxis("zoom") * zoomSensativity * Mathf.Pow(zoom, zoomExpo), minZoom - zoom, maxZoom - zoom));

		////Click Handling////
	}
}
