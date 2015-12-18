using UnityEngine;


namespace Voxelgon.Geometry {

    public static class Geometry {

        public static int TriangleWindingOrder(Vector3 point1, Vector3 point2, Vector3 point3, Vector3 normal) {
            Vector3 vectorA = (point2 - point1);
            Vector3 vectorB = (point3 - point1);

            return VectorWindingOrder(vectorA, vectorB, normal);
        }

        public static int VectorWindingOrder(Vector3 vectorA, Vector3 vectorB, Vector3 normal) {
            if (Vector3.Angle(vectorA, vectorB) < 0.01f
                || Vector3.Angle(vectorA, vectorB) > 179.99f) {
                return 0;
            }

            Vector3 resultNormal = Vector3.Cross(vectorA, vectorB);

            return (Vector3.Dot(normal, resultNormal) >= 0) ? 1 : -1; 
        }

        public static bool TriangleContains(Vector3 point1, Vector3 point2, Vector3 point3, Vector3 point, Vector3 normal) {
            return (TriangleWindingOrder(point1, point2, point, normal) != -1
            && TriangleWindingOrder(point2, point3, point, normal) != -1
            && TriangleWindingOrder(point3, point1, point, normal) != -1);
        }

        public static Vector3 TriangleNormal(Vector3 point1, Vector3 point2, Vector3 point3) {
            Vector3 vectorA = (point2 - point1);
            Vector3 vectorB = (point3 - point1);

            return Vector3.Cross(vectorA, vectorB).normalized;
        }

        public static float TriangleAngle(Vector3 point1, Vector3 point2, Vector3 point3, Vector3 normal) {
            Vector3 vectorA = (point2 - point1);
            Vector3 vectorB = (point3 - point1);

            return VectorAngle(vectorA, vectorB, normal);
        }

        public static float VectorAngle(Vector3 vectorA, Vector3 vectorB, Vector3 normal) {
            Vector3 cross = Vector3.Cross(vectorA, vectorB);
            float cos = Vector3.Dot(vectorA, vectorB) / (vectorA.magnitude * vectorB.magnitude);
            float angle = Mathf.Acos(cos) * ((Vector3.Dot(cross, normal) >= 0) ? 1 : -1);

            return angle;
        }


    }
}