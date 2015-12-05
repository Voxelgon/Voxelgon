using System;
using UnityEngine;

namespace Assets.Voxelgon.Ship
{
    [RequireComponent (typeof (Rigidbody))]

    public class ShipManager : MonoBehaviour {
        private const double FloatCompareTolerance = 0.0000001;

        //angle +/- before the port is no longer for rotation
        public float PortYawCutoff = 30;    

        //input Variables
        public float LinInput;
        public float LatInput;
        public float YawInput;

        public bool KillRot;
        public bool KillTrans;

        //translation/Yaw
        public float Yaw;
        public float Lin;
        public float Lat;

        public float BrakingYaw;
        public float BrakingLin;
        public float BrakingLat;

        private void FixedUpdate() {
            YawInput = Input.GetAxis("Yaw");
            LinInput = Input.GetAxis("Thrust");
            LatInput = Input.GetAxis("Strafe");

            if(Input.GetButtonUp("Kill Rotation")) {
                KillRot = !KillRot;
            }

            if(Input.GetButtonUp("Kill Translation")) {
                KillTrans = !KillTrans;
            }

            //TODO: PID controller for this
            if((KillRot) && (Math.Abs(YawInput) < FloatCompareTolerance) && (Mathf.Abs(GetComponent<Rigidbody>().angularVelocity.y) > 0.01)) {
                BrakingYaw = -GetComponent<Rigidbody>().angularVelocity.y;

            } else {
                BrakingYaw = 0;
            }

            if(KillTrans 
                && (Math.Abs(LinInput) < FloatCompareTolerance) 
                && (Math.Abs(LatInput) < FloatCompareTolerance) 
                && (Mathf.Abs(transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).x) > 0.1)) {
                BrakingLin = (int) -Mathf.Sign(transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).x);

            } else {
                BrakingLin = 0;
            }

            if(KillTrans 
                && (Math.Abs(LatInput) < FloatCompareTolerance)
                && (Math.Abs(LinInput) < FloatCompareTolerance) 
                && (Mathf.Abs(transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).z) > 0.1)) {
                BrakingLat = (int) Mathf.Sign(transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).z);

            } else {
                BrakingLat = 0;
            }

            Yaw = YawInput + BrakingYaw;
            Lin = LinInput + BrakingLin;
            Lat = LatInput + BrakingLat;
        }
    }
}
