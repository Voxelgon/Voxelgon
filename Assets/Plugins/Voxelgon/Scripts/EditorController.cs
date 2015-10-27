using UnityEngine;
using System.Collections;
using Voxelgon.EventSystems;

public class EditorController : MonoBehaviour, IModeChangeHandler {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	public void OnModeChange (ModeChangeEventData eventData) {
		Debug.Log ("wello, yorld!");
	
	}
}
