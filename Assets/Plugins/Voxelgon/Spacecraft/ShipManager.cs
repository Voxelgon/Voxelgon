using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Voxelgon;

[RequireComponent (typeof (Rigidbody))]

public class ShipManager : MonoBehaviour {

	public float portYawCutoff = 30;	//angle +/- before the port is no longer for rotation
	public float portTransCutoff = 15;	//angle inside the 90 degree cone for each translation direction 
	//input Variables
	public float linAxis;
	public float latAxis;
	public float yawAxis;

	//Setup Variables for gathering Ports
	public enum YawState {
		YawLeft,
		YawRight
	}

	public enum TransState {
		TransLeft,
		TransRight,
		TransForw,
		TransBack
	}

	//dictionaries of ports
	public Dictionary<YawState, List<GameObject> > yawPorts = new Dictionary<YawState, List<GameObject> > ();
	public Dictionary<TransState, List<GameObject> > transPorts = new Dictionary<TransState, List<GameObject> > ();

	public void SetupPorts(){
		
		//Yaw port lists
		yawPorts.Add(YawState.YawLeft, new List<GameObject>());
		yawPorts.Add(YawState.YawRight,new List<GameObject>());
		
		//Translation port lists
		transPorts.Add(TransState.TransLeft, new List<GameObject>());
		transPorts.Add(TransState.TransRight,new List<GameObject>());
		transPorts.Add(TransState.TransForw, new List<GameObject>());
		transPorts.Add(TransState.TransBack, new List<GameObject>());

		Vector3 origin = transform.rigidbody.centerOfMass;
		//Debug.Log(origin);
		
		Component[] PortScripts = gameObject.GetComponentsInChildren(typeof(RCSport));
		
		foreach(Component port in PortScripts) {
			float angle = Voxelgon.Math.RelativeAngle(origin, port.transform);
			float childAngle = port.transform.localEulerAngles.y;
			Debug.Log(angle);

			if((angle > portYawCutoff) && (angle < 180 - portYawCutoff)){
				yawPorts[YawState.YawLeft].Add(port.gameObject);
				Debug.Log("This port is for turning Left!");

			} else if((angle < (-1 * portYawCutoff)) && (angle > (-1 * (180 - portYawCutoff)))){
				yawPorts[YawState.YawRight].Add(port.gameObject);
				Debug.Log("This port is for turning right!");

			} else {
				if((childAngle > 315 + portTransCutoff) || (childAngle < 45 - portTransCutoff)) {

					//0 degrees
					transPorts[TransState.TransForw].Add(port.gameObject);
					Debug.Log("This port is for translating forward!");
				} else if((childAngle > 45 + portTransCutoff) && (childAngle < 135 - portTransCutoff)) {

					//90 degrees
					transPorts[TransState.TransRight].Add(port.gameObject);
					Debug.Log("This port is for translating right!");
				} else if((childAngle > 135 + portTransCutoff) && (childAngle < 225 - portTransCutoff)) {

					//180 degrees
					transPorts[TransState.TransBack].Add(port.gameObject);
					Debug.Log("This port is for translating back!");
				} else if((childAngle > 225 + portTransCutoff) && (childAngle < 315 - portTransCutoff)) {

					//270 degrees
					transPorts[TransState.TransLeft].Add(port.gameObject);
					Debug.Log("This port is for translating left!");
				}
			}
		}
	}
	
	//updates input variables
	public void UpdateInputs() {
		linAxis = Input.GetAxis("Thrust");
		latAxis = Input.GetAxis("Strafe");
		yawAxis = Input.GetAxis("Yaw");		
	}

	//Called every frame
	public void Update() {
		UpdateInputs();
	}

	//Startup Script
	public void Start() {
	SetupPorts();
	}
}