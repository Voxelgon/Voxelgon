using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public float minAltitude = 5;
	public float maxAltitude = 85;

	private float altitude = (45);

	private Quaternion altitudeQuat = new Quaternion();

	private Matrix4x4 orthoSpace = new Matrix4x4();

	// Update is called once per frame
	void Update () {
		altitude = transform.parent.eulerAngles.x ;

		orthoSpace = Matrix4x4.TRS(Vector3.one, Quaternion.Euler(-altitude,0,0), Vector3.one);

		if (Input.GetAxis("Pan") == 1) {
			transform.parent.Translate(orthoSpace.MultiplyVector(new Vector3(Input.GetAxis("Mouse X"), 0, Input.GetAxis("Mouse Y"))));
		}
		if (Input.GetAxis("Orbit") == 1) {
			transform.parent.Rotate(Vector3.up * Input.GetAxis("Mouse X"),Space.World);
			transform.parent.Rotate(Vector3.right * (Mathf.Clamp(Input.GetAxis("Mouse Y"), minAltitude - altitude, maxAltitude - altitude)),Space.Self);
			Debug.Log(minAltitude-altitude);

		}
	}
}
