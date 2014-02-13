using UnityEngine;
using System.Collections;

namespace Voxelgon {
	public class Math {	
		//returns the angle in degrees from the x axis to a line between origin and child
		//2d (x,z) only right now
		public static float TwoPointAngle(Vector3 origin, Vector3 child) {
			Vector3 deltaT = child - origin;

			float angle = Mathf.Atan(deltaT.z/deltaT.x) * Mathf.Rad2Deg;
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
	}
}