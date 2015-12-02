using UnityEngine;
using System.Collections;

public class StarController : MonoBehaviour {

	public Transform mainTransform;

	public void Begin() {
	    Component[] starSystems = gameObject.GetComponentsInChildren<ParticleSystem>();
	    foreach (Component i in starSystems){
	        i.GetComponent<ParticleSystem>().Pause();
	    }
	}

	public void Start() {
		Invoke("Begin", 1);
	}

	public void Update() {
	    transform.position = mainTransform.position;
	}
}