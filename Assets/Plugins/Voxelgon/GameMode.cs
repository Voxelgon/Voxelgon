using UnityEngine;
using System.Collections;

namespace Voxelgon {
	public static class GameMode{

		public static Gamemode current;

		public enum Gamemode {
			menu,
			edit,
			flight
		}
	}
}
