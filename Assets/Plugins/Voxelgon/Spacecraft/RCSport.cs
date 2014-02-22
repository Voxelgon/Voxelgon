using UnityEngine;
using System.Collections;

public class RCSport : MonoBehaviour {

	public GameObject particleSys;
	public Animation animator;

	public int engaged;

	public ShipManager.PortRotFunction rotFunction;
	public ShipManager.PortTransFunction transFunction;

	public ShipManager ship;

	public void Start() {
		particleSys = gameObject.GetComponentInChildren<ParticleSystem>().gameObject;
		animator = particleSys.GetComponent<Animation>();
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
		if((ship.rotControls[rotFunction] > engaged) || (ship.transControls[transFunction] > engaged)) {
			engaged = 1;

			enable();

		} else if((ship.rotControls[rotFunction] < engaged) && (ship.transControls[transFunction] < engaged)) {
			engaged = 0;

			disable();
		}
	}

	public void FixedUpdate() {
		CheckInput();
	}
}
