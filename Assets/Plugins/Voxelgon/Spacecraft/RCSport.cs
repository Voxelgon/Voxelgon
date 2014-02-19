using UnityEngine;
using System.Collections;

public class RCSport : MonoBehaviour {

	public GameObject particleSys;
	public Animation animator;

	public bool portEnabled;

	public ShipManager.PortFunction function;
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

	public void Update() {
		if ((ship.controlMatrix[function] == true) && (portEnabled == false)) {
			portEnabled = true;
			enable();
			Debug.Log("ENABLE");

		} else if((ship.controlMatrix[function] == false) && (portEnabled == true)) {
			portEnabled = false;
			disable();
		}
	}
}
