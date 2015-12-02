using UnityEngine;

namespace Voxelgon.Math {
    public static class Math {

        //0: points are colinear
        //1: points are wound clockwise
        //2: points are wound counter-clockwise
        public static int WindingOrder(Vector2 p1, Vector2 p2, Vector2 p3) {
            float val = (p2.y - p1.y) * (p3.x - p2.x) - (p2.x - p1.x) * (p3.y - p2.y); //evil vector dot product hacking

            if (Mathf.Approximately(val, 0.0f)) {
                return 1;
            }

            return (val > 0) ? 1:2;
        }

        //returns the angle in degrees from the x axis to a line between origin and child
        //2d (x,z) only right now
        public static float TwoPointAngle(Vector3 origin, Vector3 child) {
            Vector3 deltaT = child - origin;
            float angle = (Mathf.Atan2(deltaT.x, deltaT.z) * Mathf.Rad2Deg) + 90;

            if(deltaT.y < 0) {
                angle = angle + 180;
            }

            angle = (angle + 360) % 360;
            return angle;
        }

        //returns the difference in angle between two points relative to the origin in degrees.
        //2d (x,z) only right now
        public static float ThreePointAngle(Vector3 origin, Vector3 child1, Vector3 child2) {
            float angle1 = Math.TwoPointAngle(origin, child1);
            float angle2 = Math.TwoPointAngle(origin, child2);
            float angle = (angle2-angle1);

            return angle;
        }

        //gives angle relative to vector through child from origin
        //2d (x,z) only right now
        public static float RelativeAngle(Vector3 origin, Transform child) {
            float baseAngle = Math.TwoPointAngle(origin, child.localPosition);
            float childAngle = child.localEulerAngles.y;
            //Debug.Log("angle from origin " + baseAngle);
            //Debug.Log("angle of child " + childAngle);

            return 180 - (((childAngle - baseAngle)+360)%360);
        }

        public static Vector3 Modulus(this Vector3 vector, float mod) {
            vector.x %= mod;
            vector.y %= mod;
            vector.z %= mod;
            return vector;
        }

        public static Vector3 Round(this Vector3 vector) {
            vector.x = Mathf.RoundToInt(vector.x);
            vector.y = Mathf.RoundToInt(vector.y);
            vector.z = Mathf.RoundToInt(vector.z);
            return vector;
        }
    }
}
