using UnityEngine;
using System.Collections;

namespace Voxelgon {
	public class GameMode {
		public enum GameMode {
			menu,
			edit,
			flight
		}
		public GameMode currrent;
	}
}
