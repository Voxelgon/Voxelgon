using System;


namespace Voxelgon.Geometry {

    public class InvalidPolygonException : Exception {

        public InvalidPolygonException() {
        }

        public InvalidPolygonException(String message)
            : base(message) {
        }

        public InvalidPolygonException(String message, Exception inner)
            : base(message, inner) {
        }
    }
}