using UnityEngine;
using System.Collections;

namespace Voxelgon {
	public class math {	
		//returns the angle in degrees from the x axis to a line between origin and child
		//2d (x,z) only right now
		public static float TwoPointAngle(Vector3 origin, Vector3 child) {
			Vector3 deltaT = child - origin;

			float angle = Mathf.Atan(deltaT.z/deltaT.x) * Mathf.Rad2Deg;
			angle = (angle + 360) % 360;
			return angle;
		}
	}
}