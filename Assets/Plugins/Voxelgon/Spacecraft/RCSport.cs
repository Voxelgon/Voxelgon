using UnityEngine;
using System.Collections;
using Voxelgon;

public class RCSport : MonoBehaviour {

	public GameObject particleSys;
	public Animation animator;

	public int engaged;

	public ShipManager.PortRotFunction rotFunction;
	public ShipManager.PortTransFunction transFunction;

	public ShipManager ship;
	public Rigidbody rbdy;
	public Vector3 forceVector = new Vector3();

	public void Start() {
		particleSys = gameObject.GetComponentInChildren<ParticleSystem>().gameObject;
		animator = particleSys.GetComponent<Animation>();

		rbdy = transform.parent.rigidbody;

		forceVector = Voxelgon.Math.QuatToVector(transform.rotation);
	}

	public void enable() {
		animator.Play("ThrusterEnable");
		animator.CrossFadeQueued("ThrusterEffects");
	}

	public void disable() {
		animator.Stop("ThrusterEffects");
		animator.Play("ThrusterDisable");
	}

	public void CheckInput() {
		if(((ship.rotControls[rotFunction] + 2 * ship.transControls[transFunction]) > 0) && (engaged == 0)) {
			engaged = 1;

			enable();

		} else if(((ship.rotControls[rotFunction] + 2 * ship.transControls[transFunction]) < 1) && (engaged == 1)) {
			engaged = 0;

			disable();
		}
	}

	public void FixedUpdate() {
		CheckInput();
		if (engaged == 1) {
			forceVector = transform.TransformDirection(new Vector3(1,0,0));
			rbdy.AddForceAtPosition(forceVector * 1, transform.position);
			Debug.Log(forceVector);
		}
	}
}
