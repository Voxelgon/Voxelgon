using UnityEngine;
using System.Collections;

namespace Voxelgon {
	public struct Position {
		short x;
		short y;
		short z;

		public Position(short x, short y, short z) {
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public Position(float x, float y, float z) {
			this.x = (short) x;
			this.y = (short) y;
			this.z = (short) z;
		}

		public Position(Vector3 v) {
			this.x = (short) v.x;
			this.y = (short) v.y;
			this.z = (short) v.z;
		}

		public static explicit operator Position(Vector3 v) {
			return new Position(v);
		}

		public static explicit operator Vector3(Position pos) {
			return new Vector3(pos.x, pos.y, pos.z);
		}
	}
}