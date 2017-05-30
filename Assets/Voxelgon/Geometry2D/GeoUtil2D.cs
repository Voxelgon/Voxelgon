using System.Collections.Generic;
using UnityEngine;

namespace Voxelgon.Geometry2D {
    public class GeoUtil2D {
        public static float SqrDistance(Vector2 p1, Vector2 p2) {
            var dx = (p1.x - p2.x);
            var dy = (p1.y - p2.y);
            return (dx * dx + dy * dy);
        }

        public static Vector2 LineIntersection(Vector2 p1, Vector2 d1, Vector2 p2, Vector2 d2) {
            //https://gamedev.stackexchange.com/questions/44720/line-intersection-from-parametric-equation
            float u = (d1.x * (p2.y - p1.y) + d1.y * (p1.x - p2.x)) / (d2.x * d1.y - d2.y * d1.x);
            return p1 + (u * d1);
        }

        public static int WindingOrder(Vector2 p0, Vector2 p1, Vector2 p2) {
            return VectorWindingOrder(p1 - p0, p2 - p0);
        }

        public static bool Clockwise(Vector2 p0, Vector2 p1, Vector2 p2) {
            return WindingOrder(p0, p1, p2) >= 0;
        }

        public static int VectorWindingOrder(Vector2 vectorA, Vector2 vectorB) {
            float cross = (vectorB.x * vectorA.y) - (vectorA.x * vectorB.y);
            return (cross > 0) ? 1 : -1;
        }

        public static float TriangleArea(Vector2 p0, Vector2 p1, Vector2 p2) {
            return Triangle2Area(p0, p1, p2) / 2;
        }

        public static float Triangle2Area(Vector2 p0, Vector2 p1, Vector2 p2) {
            return (p0.x - p1.x) * (p2.y - p0.y) - (p0.x - p2.x) * (p1.y - p0.y);
        }

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

        public static float MiterLength(Vector2 normalA, Vector2 normalB) {
            float determinant = normalA.y * normalB.x - normalB.y * normalA.x;
            float length = (normalA.x * (normalA.x - normalB.x) + normalA.y * (normalA.y - normalB.y)) / determinant;

            return length;
        }

        public static Vector2 Miter(Vector2 normalA, Vector2 normalB) {
            var length = MiterLength(normalA, normalB);
            var miter = new Vector2(normalB.x - (normalB.y * length), normalB.y + (normalB.x * length));

            return miter;
        }

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

                    if (index1 != 0 && WindingOrder(vertices[0], vertices[index1], vertices[index3]) == 1) {
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

                    if (index1 != 0 && WindingOrder(vertices[0], vertices[index1], vertices[index3]) == 1) {
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
    }
}