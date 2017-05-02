using System;


namespace Voxelgon.Geometry {

    public class InvalidMeshException: Exception {

        public InvalidMeshException() {
        }

        public InvalidMeshException(String message)
            : base(message) {
        }

        public InvalidMeshException(String message, Exception inner)
            : base(message, inner) {
        }
    }
}