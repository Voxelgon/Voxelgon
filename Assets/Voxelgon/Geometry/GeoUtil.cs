using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;


namespace Voxelgon.Geometry {
    public static class GeoUtil {
        // PUBLIC STATIC METHODS

        // returns the intersection position for two lines, where p is a point on the line and d is its direction
        public static Vector2 LineIntersection(Vector2 p1, Vector2 d1, Vector2 p2, Vector2 d2) {
            //https://gamedev.stackexchange.com/questions/44720/line-intersection-from-parametric-equation
            float u = (d1.x * (p2.y - p1.y) + d1.y * (p1.x - p2.x)) / (d2.x * d1.y - d2.y * d1.x);
            return p1 + (u * d1);
        }

        // returns the winding order of a triangle in 3D space around `normal`
        public static int WindingOrder(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 normal) {
            var vectorA = new Vector3(p1.x - p0.x, p1.y - p0.y, p1.z - p0.z); //unity's vector ops are slow???
            var vectorB = new Vector3(p2.x - p0.x, p2.y - p0.y, p2.z - p0.z);
            return VectorWindingOrder(vectorA, vectorB, normal);
        }

        // returns the winding order of a triangle in 2D space
        public static int WindingOrder(Vector2 p0, Vector2 p1, Vector2 p2) {
            return VectorWindingOrder(p1 - p0, p2 - p0);
        }

        public static bool Clockwise(Vector2 p0, Vector2 p1, Vector2 p2) {
            return WindingOrder(p0, p1, p2) == 1;
        }

        // returns the winding order of two vectors in 3D space around `normal`
        public static int VectorWindingOrder(Vector3 vectorA, Vector3 vectorB, Vector3 normal) {
            var cross = Vector3.Cross(vectorA, normal);
            var dot = Vector3.Dot(cross, vectorB);

            if (Mathf.Approximately(0, dot)) return 0;
            return (dot > 0) ? 1 : -1;
        }

        // returns the winding order of two vectors in 2D space
        public static int VectorWindingOrder(Vector2 vectorA, Vector2 vectorB) {
            float cross = (vectorB.x * vectorA.y) - (vectorA.x * vectorB.y);
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
        public static float TriangleArea(Vector2 p0, Vector2 p1, Vector2 p2) {
            return Triangle2Area(p0, p1, p2) / 2;
        }

        // returns 2 times the area of a triangle in 2D space
        public static float Triangle2Area(Vector2 p0, Vector2 p1, Vector2 p2) {
            return (p0.x - p1.x) * (p2.y - p0.y) - (p0.x - p2.x) * (p1.y - p0.y);
        }

        // returns if a triangle in 3D space contains a given point
        // the point is projected onto the plane of the triangle
        public static bool TriangleContains(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p, Vector3 normal) {
            bool contains = (WindingOrder(p0, p1, p, normal) != -1
                             && WindingOrder(p1, p2, p, normal) != -1
                             && WindingOrder(p2, p0, p, normal) != -1);
            return contains;
        }

        // returns if a triangle in 2D space contains a given point
        // using barycentric coordinates
        public static bool TriangleContains(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p) {
            var s = p0.y * p2.x - p0.x * p2.y + (p2.y - p0.y) * p.x + (p0.x - p2.x) * p.y;
            var t = p0.x * p1.y - p0.y * p1.x + (p0.y - p1.y) * p.x + (p1.x - p0.x) * p.y;

            if ((s < 0) != (t < 0))
                return false;

            //var a = TriangleDoubleArea2D(p0, p1, p2);
            var a = -p1.y * p2.x + p0.y * (p2.x - p1.x) + p0.x * (p1.y - p2.y) + p1.x * p2.y;

            if (a < 0) {
                //return s < 0 && t < 0 && (s + t) >= a;
                s *= -1;
                t *= -1;
                a *= -1;
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
            if (points.Length == 0) throw new ArgumentException();

            float sumX = 0;
            float sumY = 0;
            float sumZ = 0;
            foreach (Vector3 v in points) {
                sumX += v.x;
                sumY += v.y;
                sumZ += v.z;
            }

            float mult = 1.0f / points.Length;
            return new Vector3(sumX * mult, sumY * mult, sumZ * mult);
        }

        // returns the average of several points
        public static Vector3 VectorAvg(IEnumerable<Vector3> points) {
            float sumX = 0;
            float sumY = 0;
            float sumZ = 0;

            float n = 0;
            foreach (var v in points) {
                sumX += v.x;
                sumY += v.y;
                sumZ += v.z;
                n++;
            }

            float mult = 1 / n;
            return new Vector3(sumX * mult, sumY * mult, sumZ * mult);
        }

        // returns the average of several points
        public static Vector2 VectorAvg(IEnumerable<Vector2> points) {
            float sumX = 0;
            float sumY = 0;

            float n = 0;
            foreach (var v in points) {
                sumX += v.x;
                sumY += v.y;
                n++;
            }

            float mult = 1 / n;
            return new Vector3(sumX * mult, sumY * mult);
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
            var p1 = vertices[vertices.Length - 2];
            var p2 = vertices[vertices.Length - 1];

            foreach (var v in vertices) {
                sum += (p2.x - p1.x) * (p2.y + p1.y);
                p1 = p2;
                p2 = v;
            }
            return sum;
        }

        // uses the shoelace algorithm on `vertices`, is 2*Area, and negative if clockwise
        public static float Shoelace(IList<Vector2> vertices) {
            float sum = 0;
            var p1 = vertices[vertices.Count - 2];
            var p2 = vertices[vertices.Count - 1];

            foreach (var v in vertices) {
                sum += (p2.x - p1.x) * (p2.y + p1.y);
                p1 = p2;
                p2 = v;
            }
            return sum;
        }

        // returns the first normal found for a set of vertices in 3D 
        public static Vector3 PointsNormal(Vector3[] vertices) {
            if (vertices.Length < 3) {
                return Vector3.zero;
            }

            var p1 = vertices[vertices.Length - 2];
            var p2 = vertices[vertices.Length - 1];
            var p3 = vertices[0];

            foreach (var v in vertices) {
                var normal = TriangleNormal(p1, p2, p3);
                p1 = p2;
                p2 = p3;
                p3 = v;
                if (normal.sqrMagnitude > 0.001f) return normal;
            }

            return Vector3.zero;
        }

        public static void TransformPoints(Vector3[] vertices, Matrix4x4 matrix) {
            for (var i = 0; i < vertices.Length; i++) {
                vertices[i] = matrix.MultiplyPoint3x4(vertices[i]);
            }
        }

        // length along the end of normalB perpendicular to it, 
        // useful if you dont need the whole vector (e.g. normalB is vertical)
        public static float MiterLength(Vector2 normalA, Vector2 normalB) {
            float determinant = normalA.y * normalB.x - normalB.y * normalA.x;
            float length = (normalA.x * (normalA.x - normalB.x) + normalA.y * (normalA.y - normalB.y)) / determinant;

            return length;
        }

        // returns the point at the intersection to the surfaces at the ends of normalA and normalB
        public static Vector2 Miter(Vector2 normalA, Vector2 normalB) {
            var length = MiterLength(normalA, normalB);
            var miter = new Vector2(normalB.x - (normalB.y * length), normalB.y + (normalB.x * length));

            return miter;
        }

        // adds triangle indices for `vertices` to `tris`, and calls itself recursively to handle concave polygons
        // `index1` and `index2` should be 0 and 1 for the top level call
        public static int TriangulateSegment(Vector2[] vertices, List<int> tris, int index1, int index2,
            int offset = 0) {
            int index3 = index2 + 1;

            while (index3 < vertices.Length && index3 >= 0) {
                bool validTri = true;

                if (WindingOrder(vertices[index1], vertices[index2], vertices[index3]) == 1) {
                    for (int i = index3 + 1; i < vertices.Length && validTri; i++) {
                        validTri &= !TriangleContains(vertices[index1], vertices[index2], vertices[index3],
                            vertices[i]);
                    }
                }
                else {
                    validTri = false;
                }

                if (validTri) {
                    tris.Add(offset + index1);
                    tris.Add(offset + index2);
                    tris.Add(offset + index3);

                    if (index1 != 0 &&
                        WindingOrder(vertices[0], vertices[index1], vertices[index3]) == 1) {
                        return index3;
                    }

                    index2 = index3;
                    index3++;
                }
                else {
                    index3 = TriangulateSegment(vertices, tris, index2, index3);
                }
            }
            return -1;
        }

        // adds triangle indices for `vertices` to `tris`, and calls itself recursively to handle concave polygons
        // `index1` and `index2` should be 0 and 1 for the top level call
        public static int TriangulateSegment(IList<Vector2> vertices, IList<int> tris, int index1, int index2,
            int offset = 0) {
            int index3 = index2 + 1;

            while (index3 < vertices.Count && index3 >= 0) {
                bool validTri = true;

                if (WindingOrder(vertices[index1], vertices[index2], vertices[index3]) == 1) {
                    for (int i = index3 + 1; i < vertices.Count && validTri; i++) {
                        validTri &= !TriangleContains(vertices[index1], vertices[index2], vertices[index3],
                            vertices[i]);
                    }
                }
                else {
                    validTri = false;
                }

                if (validTri) {
                    tris.Add(offset + index1);
                    tris.Add(offset + index2);
                    tris.Add(offset + index3);

                    if (index1 != 0 &&
                        WindingOrder(vertices[0], vertices[index1], vertices[index3]) == 1) {
                        return index3;
                    }

                    index2 = index3;
                    index3++;
                }
                else {
                    index3 = TriangulateSegment(vertices, tris, index2, index3);
                }
            }
            return -1;
        }

        // Creates a new AABB from two AABBs
        public static Bounds CalcBounds(Bounds box1, Bounds box2) {
            var bounds = new Bounds();
            var min = new Vector3(
                Mathf.Min(box1.min.x, box2.min.x),
                Mathf.Min(box1.min.y, box2.min.y),
                Mathf.Min(box1.min.z, box2.min.z));
            var max = new Vector3(
                Mathf.Max(box1.max.x, box2.max.x),
                Mathf.Max(box1.max.y, box2.max.y),
                Mathf.Max(box1.max.z, box2.max.z));
            bounds.SetMinMax(min, max);
            return bounds;
        }

        // Creates a new AABB from a list of IBoundables
        public static Bounds CalcBounds(IEnumerable<Bounds> collection) {
            var min = new Vector3();
            var max = new Vector3();

            var array = collection as Bounds[] ?? collection.ToArray();

            for (int i = 0; i < 3; i++) {
                min[i] = array.Min(o => o.min[i]);
                max[i] = array.Max(o => o.max[i]);
            }

            var bounds = new Bounds();
            bounds.SetMinMax(min, max);
            return bounds;
        }

        // Creates a new AABB from a list of vectors 
        public static Bounds CalcBounds(IEnumerable<Vector3> collection) {
            var min = new Vector3();
            var max = new Vector3();

            var array = collection as Vector3[] ?? collection.ToArray();

            for (int i = 0; i < 3; i++) {
                min[i] = array.Min(o => o[i]);
                max[i] = array.Max(o => o[i]);
            }

            var bounds = new Bounds();
            bounds.SetMinMax(min, max);
            return bounds;
        }
    }
}