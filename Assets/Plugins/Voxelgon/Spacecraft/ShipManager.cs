using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Voxelgon;

[RequireComponent (typeof (Rigidbody))]

public class ShipManager : MonoBehaviour {

	public float portYawCutoff = 30;	//angle +/- before the port is no longer for rotation
	//input Variables
	public float linInput;
	public float latInput;
	public float yawInput;

	//Setup Variables for gathering Ports
	public enum PortRotFunction {
		Left,
		Right,
		None
	}

	public enum PortTransFunction {
		Left,
		Right,
		Forward,
		Back
	}

	//translation/yaw
	public int yaw;
	public int lin;
	public int lat;

	//dictionaries of ports for reference
	public Dictionary<PortRotFunction, List<GameObject> > rotPorts = new Dictionary<PortRotFunction, List<GameObject> > ();
	public Dictionary<PortTransFunction, List<GameObject> > transPorts = new Dictionary<PortTransFunction, List<GameObject> > ();

	//dictionaries of ints for ports to access for control purposes
	public Dictionary<PortRotFunction, int> rotControls = new Dictionary<PortRotFunction, int> ();
	public Dictionary<PortTransFunction, int> transControls = new Dictionary<PortTransFunction, int> ();


	public void SetupPorts(){
		
		//Yaw port lists
		rotPorts.Add(PortRotFunction.Left, new List<GameObject>());
		rotPorts.Add(PortRotFunction.Right, new List<GameObject>());
		
		//Linear port lists
		transPorts.Add(PortTransFunction.Forward, new List<GameObject>());
		transPorts.Add(PortTransFunction.Back, new List<GameObject>());
		
		//Lateral port lists
		transPorts.Add(PortTransFunction.Left, new List<GameObject>());
		transPorts.Add(PortTransFunction.Right, new List<GameObject>());
		

		//Yaw control lists
		rotControls.Add(PortRotFunction.Left, new int ());
		rotControls.Add(PortRotFunction.Right, new int ());
		rotControls.Add(PortRotFunction.None, new int ());
		
		//Linear control lists
		transControls.Add(PortTransFunction.Forward, new int ());
		transControls.Add(PortTransFunction.Back, new int ());
		
		//Lateral control lists
		transControls.Add(PortTransFunction.Left, new int ());
		transControls.Add(PortTransFunction.Right, new int ());


		Vector3 origin = transform.rigidbody.centerOfMass;
		Component[] PortScripts = gameObject.GetComponentsInChildren(typeof(RCSport));

		foreach(Component port in PortScripts) {
			
			float angle = Voxelgon.Math.RelativeAngle(origin, port.transform);
			float childAngle = port.transform.localEulerAngles.y;

			RCSport portScript = port.GetComponent<RCSport>();
			portScript.ship = this;

			Debug.Log(angle + ":" + portScript.name);
			//Rotation
			if((angle > portYawCutoff) && (angle < 180 - portYawCutoff)){
				
				//30 degrees to 150 degrees
				rotPorts[PortRotFunction.Right].Add(port.gameObject);

				portScript.rotFunction = PortRotFunction.Right;

			} else if((angle < (-1 * portYawCutoff)) && (angle > (-1 * (180 - portYawCutoff)))){
				
				//-150 degrees to -30 degrees
				rotPorts[PortRotFunction.Left].Add(port.gameObject);

				portScript.rotFunction = PortRotFunction.Left;

			} else {

				//not suitable for rotation
				portScript.rotFunction = PortRotFunction.None;
			}


			//Translation
			if((childAngle > 315) || (childAngle < 45)) {

				//0 degrees
				transPorts[PortTransFunction.Forward].Add(port.gameObject);

				portScript.transFunction = PortTransFunction.Forward;

			} else if((childAngle > 45) && (childAngle < 135)) {

				//90 degrees
				transPorts[PortTransFunction.Right].Add(port.gameObject);

				portScript.transFunction = PortTransFunction.Right;

			} else if((childAngle > 135) && (childAngle < 225)) {

				//180 degrees
				transPorts[PortTransFunction.Back].Add(port.gameObject);

				portScript.transFunction = PortTransFunction.Back;

			} else if((childAngle > 225) && (childAngle < 315)) {

				//270 degrees
				transPorts[PortTransFunction.Left].Add(port.gameObject);

				portScript.transFunction = PortTransFunction.Left;

			}
		}
	}
	

	//updates input variables
	public void UpdateInputs() {
		yawInput = Input.GetAxis("Yaw");	
		linInput = Input.GetAxis("Thrust");
		latInput = Input.GetAxis("Strafe");

		yaw = (int) yawInput;
		lin = (int) linInput;
		lat = (int) latInput;

		//           up
		//           +1
		//	  	     /\
		//           ||
		// left -1 <====> +1 right
		//           ||
		//           \/
		//           -1
		//          down

		rotControls[PortRotFunction.Right] = yaw;
		rotControls[PortRotFunction.Left] = -yaw;

		transControls[PortTransFunction.Forward] = lin;
		transControls[PortTransFunction.Back] = -lin;

		transControls[PortTransFunction.Right] = lat;
		transControls[PortTransFunction.Left] = -lat;
	}


	//Called every frame
	public void FixedUpdate() {
		UpdateInputs();
	}


	//Startup Script
	public void Start() {
	SetupPorts();
	}
}