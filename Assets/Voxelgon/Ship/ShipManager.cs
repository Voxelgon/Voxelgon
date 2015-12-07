using UnityEngine;

namespace Voxelgon.Ship
{
    [RequireComponent (typeof (Rigidbody))]

    public class ShipManager : MonoBehaviour
    {

        private const double Tolerance = 0.00001;

        /// <summary>
        /// The port yaw cutoff, set to angle +/- before the port is no longer for rotation
        /// </summary>
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
            if(KillRot 
                && (System.Math.Abs(YawInput) < Tolerance) 
                && (Mathf.Abs(GetComponent<Rigidbody>().angularVelocity.y) > 0.01)) {

                BrakingYaw = -GetComponent<Rigidbody>().angularVelocity.y;

            } else {
                BrakingYaw = 0;
            }

            if(KillTrans 
                && (System.Math.Abs(LinInput) < Tolerance) 
                && (System.Math.Abs(LatInput) < Tolerance) 
                && (Mathf.Abs(transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).x) > 0.1)) {

                BrakingLin = (int) -Mathf.Sign(transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).x);

            } else {
                BrakingLin = 0;
            }

            if(KillTrans 
                && (System.Math.Abs(LatInput) < Tolerance)
                && (System.Math.Abs(LinInput) < Tolerance) 
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
