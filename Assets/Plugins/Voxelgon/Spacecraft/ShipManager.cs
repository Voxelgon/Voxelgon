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
	public float linInput;
	public float latInput;
	public float yawInput;

	//Setup Variables for gathering Ports
	public enum PortFunction {
		YawLeft,
		YawRight,
		TransLeft,
		TransRight,
		TransForw,
		TransBack
	}

	//translation/yaw
	public int yaw;
	public int lin;
	public int lat;

	//dictionary of ports
	public Dictionary<PortFunction, List<GameObject> > ports = new Dictionary<PortFunction, List<GameObject> > ();

	//dictionary of boolians for ports to access
	public Dictionary<PortFunction, bool> controlMatrix = new Dictionary<PortFunction, bool> ();


	public void SetupPorts(){
		
		//Yaw port lists
		ports.Add(PortFunction.YawLeft, new List<GameObject>());
		ports.Add(PortFunction.YawRight,new List<GameObject>());
		
		//Linear port lists
		ports.Add(PortFunction.TransForw, new List<GameObject>());
		ports.Add(PortFunction.TransBack, new List<GameObject>());
		
		//Lateral ports lists
		ports.Add(PortFunction.TransLeft, new List<GameObject>());
		ports.Add(PortFunction.TransRight,new List<GameObject>());
		

		//Yaw control lists
		controlMatrix.Add(PortFunction.YawLeft, new bool ());
		controlMatrix.Add(PortFunction.YawRight,new bool ());
		
		//Linear control lists
		controlMatrix.Add(PortFunction.TransForw, new bool ());
		controlMatrix.Add(PortFunction.TransBack, new bool ());
		
		//Lateral control lists
		controlMatrix.Add(PortFunction.TransLeft, new bool ());
		controlMatrix.Add(PortFunction.TransRight,new bool ());


		Vector3 origin = transform.rigidbody.centerOfMass;
		//Debug.Log(origin);
		
		Component[] PortScripts = gameObject.GetComponentsInChildren(typeof(RCSport));
		
		foreach(Component port in PortScripts) {
			float angle = Voxelgon.Math.RelativeAngle(origin, port.transform);
			float childAngle = port.transform.localEulerAngles.y;
			Debug.Log(angle);

			RCSport portScript = port.GetComponent<RCSport>();
			portScript.ship = gameObject;

			//tag port appropriately
			if((angle > portYawCutoff) && (angle < 180 - portYawCutoff)){
				
				//30 degrees to 150 degrees
				ports[PortFunction.YawLeft].Add(port.gameObject);
				Debug.Log("This port is for turning Left!");

			} else if((angle < (-1 * portYawCutoff)) && (angle > (-1 * (180 - portYawCutoff)))){
				
				//-150 degrees to -30 degrees
				ports[PortFunction.YawRight].Add(port.gameObject);
				Debug.Log("This port is for turning right!");

			} else {

				if((childAngle > 315 + portTransCutoff) || (childAngle < 45 - portTransCutoff)) {

					//0 degrees
					ports[PortFunction.TransForw].Add(port.gameObject);
					Debug.Log("This port is for translating forward!");

				} else if((childAngle > 45 + portTransCutoff) && (childAngle < 135 - portTransCutoff)) {

					//90 degrees
					ports[PortFunction.TransRight].Add(port.gameObject);
					Debug.Log("This port is for translating right!");

				} else if((childAngle > 135 + portTransCutoff) && (childAngle < 225 - portTransCutoff)) {

					//180 degrees
					ports[PortFunction.TransBack].Add(port.gameObject);
					Debug.Log("This port is for translating back!");

				} else if((childAngle > 225 + portTransCutoff) && (childAngle < 315 - portTransCutoff)) {

					//270 degrees
					ports[PortFunction.TransLeft].Add(port.gameObject);
					Debug.Log("This port is for translating left!");
				}
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

		//yaw
		if(yaw == 1) {

			//yaw right
			controlMatrix[PortFunction.YawRight] = true;
			controlMatrix[PortFunction.YawLeft] = false;
		} else if(yaw == -1) {

			//yaw left
			controlMatrix[PortFunction.YawRight] = false;
			controlMatrix[PortFunction.YawLeft] = true;
		} else {

			//no yaw
			controlMatrix[PortFunction.YawRight] = false;
			controlMatrix[PortFunction.YawLeft] = false;
		}

		//linear/thrust
		if(yaw == 1) {

			//thrust forward
			controlMatrix[PortFunction.TransForw] = true;
			controlMatrix[PortFunction.TransBack] = false;
		} else if(yaw == -1) {

			//thrust back
			controlMatrix[PortFunction.TransForw] = false;
			controlMatrix[PortFunction.TransBack] = true;
		} else {

			//no thrust
			controlMatrix[PortFunction.TransForw] = false;
			controlMatrix[PortFunction.TransBack] = false;
		}

		//lateral/strafe
		if(yaw == 1) {

			//thrust right
			controlMatrix[PortFunction.TransRight] = true;
			controlMatrix[PortFunction.TransLeft] = false;
		} else if(yaw == -1) {

			//thrust left
			controlMatrix[PortFunction.TransRight] = false;
			controlMatrix[PortFunction.TransLeft] = true;
		} else {

			//no thrust
			controlMatrix[PortFunction.TransRight] = false;
			controlMatrix[PortFunction.TransLeft] = false;
		}
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