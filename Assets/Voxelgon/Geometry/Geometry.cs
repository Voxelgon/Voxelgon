using UnityEngine;
using System.Collections.Generic;


namespace Voxelgon.Geometry {

    public static class Geometry {

        // PUBLIC STATIC METHODS

        // returns the winding order of a triangle in 3D space around `normal`
        public static int TriangleWindingOrder(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 normal) {
            var vectorA = new Vector3(p1.x - p0.x, p1.y - p0.y, p1.z - p0.z); //unity's vector ops are slow???
            var vectorB = new Vector3(p2.x - p0.x, p2.y - p0.y, p2.z - p0.z);
            return VectorWindingOrder(vectorA, vectorB, normal);
        }

        // returns the winding order of a triangle in 2D space
        public static int TriangleWindingOrder2D(Vector2 p0, Vector2 p1, Vector2 p2) {
            var doubleArea = TriangleDoubleArea2D(p0, p1, p2);

            if (doubleArea > 0.001f) return 1;
            if (doubleArea < -0.001f) return -1;
            return 0;
        }

        // returns the winding order of two vectors in 3D space around `normal`
        public static int VectorWindingOrder(Vector3 vectorA, Vector3 vectorB, Vector3 normal) {
            var cross = Vector3.Cross(vectorA, normal);
            var dot = Vector3.Dot(cross, vectorB);

            if (dot > 0.0001f) return 1;
            if (dot < -0.0001f) return -1;
            return 0;
        }

        // returns the winding order of two vectors in 2D space
        public static int VectorWindingOrder2D(Vector2 VectorA, Vector2 VectorB) {
            float cross = (VectorA.x * VectorB.y) - (VectorB.x * VectorA.y);
            return (cross > 0) ? 1 : -1;
        }

        // returns the area of a triangle in 3D space
        public static float TriangleArea(Vector3 p0, Vector3 p1, Vector3 p2) {
            var vectorA = new Vector3(p1.x - p0.x, p1.y - p0.y, p1.z - p0.z); //unity's vector ops are slow???
            var vectorB = new Vector3(p2.x - p0.x, p2.y - p0.y, p2.z - p0.z);
            var crossed = Vector3.Cross(vectorA, vectorB);

            return crossed.magnitude / 2;
        }

        // returns the area of a triangle in 2D space
        public static float TriangleArea2D(Vector2 p0, Vector2 p1, Vector2 p2) {
            return TriangleDoubleArea2D(p0, p1, p2) / 2;
        }

        // returns 2 times the area of a triangle in 2D space
        public static float TriangleDoubleArea2D(Vector2 p0, Vector2 p1, Vector2 p2) {
            return (-p1.y * p2.x + p0.y * (-p1.x + p2.x) + p0.x * (p1.y - p2.y) + p1.x * p2.y);
        }

        // returns if a triangle in 3D space contains a given point
        // the point is projected onto the plane of the triangle
        public static bool TriangleContains(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p, Vector3 normal) {
            bool contains = (TriangleWindingOrder(p0, p1, p, normal) != -1
            && TriangleWindingOrder(p1, p2, p, normal) != -1
            && TriangleWindingOrder(p2, p0, p, normal) != -1);
            return contains;
        }

        // returns if a triangle in 2D space contains a given point
        // using barycentric coordinates
        public static bool TriangleContains2D(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p) {
            var s = p0.y * p2.x - p0.x * p2.y + (p2.y - p0.y) * p.x + (p0.x - p2.x) * p.y;
            var t = p0.x * p1.y - p0.y * p1.x + (p0.y - p1.y) * p.x + (p1.x - p0.x) * p.y;

            if ((s < 0) != (t < 0))
                return false;

            var a = TriangleDoubleArea2D(p0, p1, p2);
            if (a < 0) {
                return s < 0 && t < 0 && (s + t) <= a;
            }
            return s > 0 && t > 0 && (s + t) <= a;

        }

        // returns the normal vector of a triangle
        public static Vector3 TriangleNormal(Vector3 p0, Vector3 p1, Vector3 p2) {
            var vectorA = new Vector3(p1.x - p0.x, p1.y - p0.y, p1.z - p0.z); //unity's vector ops are slow???
            var vectorB = new Vector3(p2.x - p0.x, p2.y - p0.y, p2.z - p0.z);
            var crossed = Vector3.Cross(vectorA, vectorB);

            return crossed.normalized;
        }

        // returns the angle of the first vertex of a triangle
        public static float TriangleAngle(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 normal) {
            var vectorA = new Vector3(p1.x - p0.x, p1.y - p0.y, p1.z - p0.z); //unity's vector ops are slow???
            var vectorB = new Vector3(p2.x - p0.x, p2.y - p0.y, p2.z - p0.z);

            return VectorAngle(vectorA, vectorB, normal);
        }

        // returns the angle between two vectors
        public static float VectorAngle(Vector3 vectorA, Vector3 vectorB, Vector3 normal) {
            Vector3 cross = Vector3.Cross(vectorA, vectorB);
            float cos = Vector3.Dot(vectorA, vectorB) / (vectorA.magnitude * vectorB.magnitude);
            float angle = Mathf.Acos(cos) * ((Vector3.Dot(cross, normal) >= 0) ? 1 : -1);


            return angle;
        }

        // returns the average of several points
        public static Vector3 VectorAvg(Vector3[] points) {
            float sumX = 0;
            float sumY = 0;
            float sumZ = 0;
            foreach (Vector3 v in points) {
                sumX += v.x;
                sumY += v.y;
                sumZ += v.z;
            }

            float mult = 1 / points.Length;
            return new Vector3(sumX * mult, sumY * mult, sumZ * mult);
        }

        // flattens `vertices` onto a plane through `center` with the normal `normal`
        public static Vector2[] FlattenPoints(Vector3 center, Vector3[] vertices, Vector3 normal) {
            Vector2[] flattened = new Vector2[vertices.Length];
            Matrix4x4 matrix = Matrix4x4.TRS(
                center * -1,
                Quaternion.FromToRotation(normal, Vector3.forward),
                Vector3.one);
            for (var i = 0; i < vertices.Length; i++) {
                flattened[i] = matrix.MultiplyPoint3x4(vertices[i]);
            }

            return flattened;
        }

        // flattens `vertices` onto a plane through the origin with the normal `normal`
        public static Vector2[] FlattenPoints(Vector3[] vertices, Vector3 normal) {
            return FlattenPoints(Vector3.zero, vertices, normal);
        }

        // uses the shoelace algorithm on `vertices`, is 2*Area, and negative if clockwise
        public static float Shoelace(Vector2[] vertices) {
            float sum = 0;
            Vector2 p1 = vertices[vertices.Length - 1];
            Vector2 p2 = vertices[0];
            for (int i = 0; i < vertices.Length; i++) {
                sum += (p2.x - p1.x) * (p2.y + p1.y);
                p1 = p2;
                p2 = vertices[i];
            }
            return sum;
        }

        // returns the first normal found for a set of vertices in 3D 
        public static Vector3 PointsNormal(Vector3[] vertices) {
            if (vertices.Length <= 3) {
                return Vector3.zero;
            }

            Vector3 p1 = vertices[vertices.Length - 2];
            Vector3 p2 = vertices[vertices.Length - 1];
            Vector3 p3 = vertices[0];
            Vector3 normal = Vector3.zero;
            for (var i = 0; i < vertices.Length; i++) {
                normal = Geometry.TriangleNormal(p1, p2, p3);
                p1 = p2;
                p2 = p3;
                p3 = vertices[i];
                if (normal.sqrMagnitude > 0.001f) return normal;
            }

            return Vector3.zero;

        }

        public static void TransformPoints(Vector3[] vertices, Matrix4x4 matrix) {
            for (var i = 0; i < vertices.Length; i++) {
                vertices[i] = matrix.MultiplyPoint3x4(vertices[i]);
            }
        }

        public static Vector2 Miter(Vector2 normalA, Vector2 normalB) {
            float determinant = normalA.y * normalB.x - normalB.y * normalA.x;
            float bLength = (normalA.x * (normalA.x - normalB.x) + normalA.y * (normalA.y - normalB.y)) / determinant;

            return -Mathf.Sign(determinant) * new Vector2( normalB.y - (normalB.x * bLength), -normalB.x - (normalB.y * bLength));
        }

        // adds triangle indices for `vertices` to `tris`, and calls itself recursively to handle concave polygons
        // `index1` and `index2` should be 0 and 1 for the top level call
        public static int TriangulateSegment(Vector2[] vertices, List<int> tris, int index1, int index2, int offset = 0) {
            int index3 = index2 + 1;

            while (index3 < vertices.Length && index3 >= 0) {
                bool validTri = true;

                if (Geometry.TriangleWindingOrder2D(vertices[index1], vertices[index2], vertices[index3]) == 1) {
                    for (int i = index3 + 1; i < vertices.Length && validTri; i++) {
                        validTri &= !Geometry.TriangleContains2D(vertices[index1], vertices[index2], vertices[index3], vertices[i]);
                    }
                } else {
                    validTri = false;
                }

                if (validTri) {
                    tris.Add(offset + index1);
                    tris.Add(offset + index2);
                    tris.Add(offset + index3);

                    if (index1 != 0 && Geometry.TriangleWindingOrder2D(vertices[0], vertices[index1], vertices[index3]) == 1) {
                        return index3;
                    }

                    index2 = index3;
                    index3++;
                } else {
                    index3 = TriangulateSegment(vertices, tris, index2, index3);
                }
            }
            return -1;
        }
    }
}