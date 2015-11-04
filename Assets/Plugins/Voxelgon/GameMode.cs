using UnityEngine;
using System.Collections;

namespace Voxelgon {
	public static class GameMode {
		public enum Mode {
			edit,
			flight
		}
		public static Mode current;
	}
}
