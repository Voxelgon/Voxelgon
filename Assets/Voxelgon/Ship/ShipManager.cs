using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Voxelgon;

[RequireComponent (typeof (Rigidbody))]

public class ShipManager : MonoBehaviour {

    public float portYawCutoff = 30;    //angle +/- before the port is no longer for rotation

    //input Variables
    public float linInput;
    public float latInput;
    public float yawInput;

    public bool killRot;
    public bool killTrans;

    //translation/yaw
    public float yaw;
    public float lin;
    public float lat;

    public float brakingYaw = 0;
    public float brakingLin = 0;
    public float brakingLat = 0;

    private void FixedUpdate() {
        yawInput = Input.GetAxis("Yaw");
        linInput = Input.GetAxis("Thrust");
        latInput = Input.GetAxis("Strafe");

        if(Input.GetButtonUp("Kill Rotation")) {
            killRot = !killRot;
        }

        if(Input.GetButtonUp("Kill Translation")) {
            killTrans = !killTrans;
        }

        //TODO: PID controller for this
        if((killRot) && (yawInput == 0) && (Mathf.Abs(GetComponent<Rigidbody>().angularVelocity.y) > 0.01)) {
            brakingYaw = -GetComponent<Rigidbody>().angularVelocity.y;

        } else {
            brakingYaw = 0;
        }

        if((killTrans) && (linInput == 0) && (latInput == 0) && (Mathf.Abs(transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).x) > 0.1)) {
            brakingLin = (int) -Mathf.Sign(transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).x);

        } else {
            brakingLin = 0;
        }

        if((killTrans) && (latInput == 0)&& (linInput == 0) && (Mathf.Abs(transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).z) > 0.1)) {
            brakingLat = (int) Mathf.Sign(transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).z);

        } else {
            brakingLat = 0;
        }

        yaw = yawInput + brakingYaw;
        lin = linInput + brakingLin;
        lat = latInput + brakingLat;
    }
}
