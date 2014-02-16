using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Voxelgon;

[RequireComponent (typeof (Rigidbody))]

public class ShipManager : MonoBehaviour {

	public float portTransCutoff = 5;

	//input Variables
	public float linAxis;
	public float latAxis;
	public float yawAxis;

	//Setup Variables for gathering Ports
	public enum Direction{
		YawLeft,
		YawRight,
		TransLeft,
		TransRight,
		TransForw,
		TransBack
	}

	//dictionary of ports
	public Dictionary<Direction, List<GameObject> > portGroups = new Dictionary<Direction, List<GameObject> > ();

	public void SetupPorts(){

		portGroups.Add( Direction.YawLeft, new List<GameObject>() );
		portGroups.Add( Direction.YawRight, new List<GameObject>() );
		portGroups.Add( Direction.TransLeft, new List<GameObject>() );
		portGroups.Add( Direction.TransRight, new List<GameObject>() );
		portGroups.Add( Direction.TransForw, new List<GameObject>() );
		portGroups.Add( Direction.TransBack, new List<GameObject>() );

		Vector3 origin = transform.rigidbody.centerOfMass;
		//Debug.Log(origin);
		
		Component[] PortScripts = gameObject.GetComponentsInChildren(typeof(RCSport));
		
		foreach(Component i in PortScripts) {
			float angle = Voxelgon.Math.RelativeAngle(origin, i.transform);
			//Debug.Log(angle);

			if(angle > portTransCutoff){
				portGroups[Direction.YawLeft].Add(i.gameObject);
				//Debug.Log("This port is for turning Left!");

			} else if(angle < (-1 * portTransCutoff)){
				portGroups[Direction.YawRight].Add(i.gameObject);
				//Debug.Log("This port is for turning right!");

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