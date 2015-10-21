using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class test : MonoBehaviour, IPointerEnterHandler {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnPointerEnter(PointerEventData data){
		Debug.Log("hello, world!");
	}
}
