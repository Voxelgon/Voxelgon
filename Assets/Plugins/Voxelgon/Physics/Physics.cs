using UnityEngine;
using System.Collections;

namespace Voxelgon {
	public class Physics { 
		public static float elasticDeltaV(Rigidbody r1,Rigidbody r2) {
			float u1 = r1.velocity.magnitude;
			float u2 = r2.velocity.magnitude;
			float m1 = r1.mass;
			float m2 = r2.mass;

			float v1 = (u1*(m1-m2)+(2*(m2*u2)))/(m1+m2);
			return(v1);
		}
	}
}