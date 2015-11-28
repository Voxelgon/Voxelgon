namespace Voxelgon {
	public static class GameMode {
		public enum Mode {
			Edit,
			Flight
		}

		private static readonly Mode current = Mode.Flight;

		public static Mode Current {
			get {
				return current;
			}
		}

	}
}
