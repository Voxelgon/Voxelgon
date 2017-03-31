using UnityEngine;

namespace Voxelgon.Util.Geometry {

    public struct Cylinder : IBoundable {

        // FIELDS

        public Vector3 point1;
        public Vector3 point2;
        public float radius;

        private Bounds _bounds;


        // PROPERTIES

        public Bounds Bounds {
            get { return _bounds; }
        }


        // CONSTRUCTORS

        public Cylinder(Vector3 point1, Vector3 point2, float radius) {
            this.point1 = point1;
            this.point2 = point2;
            this.radius = radius;

            var center = point1.Average(point2);
            _bounds = point1.CalcBounds(point2);
            _bounds.Expand(radius);
        }


        // METHODS

        public static bool RaycastCylinder(Vector3 point1, Vector3 point2, float radius, Ray ray, out float distance) {
            distance = 0;
            Vector3 d = point2 - point1; // axis of the cylinder
            Vector3 m = ray.origin - point1; // relative ray origin
            Vector3 n = ray.direction;
            var md = Vector3.Dot(m, d);
            var nd = Vector3.Dot(n, d);
            var dd = d.sqrMagnitude;

            // Test if segment fully outside either endcap of cylinder
            if ((md < 0.0f && md + nd < 0.0f) // ray outside `p1` side of cylinder
             || (md > dd && md + nd > dd)) {  // ray outside `p2` side of cylinder
                return false;
            }

            var mn = Vector3.Dot(m, n);
            var a = dd - (nd * nd);
            var k = m.sqrMagnitude - (radius * radius);
            var c = (dd * k) - (md * md);

            if (Mathf.Abs(a) < Mathf.Epsilon) {
                // Segment runs parallel to cylinder axis
                if (c > 0.0f) {
                    // ’a’ outside cylinder
                    return false;
                }

                // Now known that segment intersects cylinder; figure out how it intersects
                if (md < 0.0f) distance = -mn;        // Intersect ray against `p1` endcap
                else if (md > dd) distance = nd - mn; // Intersect ray against `p2` endcap
                else distance = 0;  // ’a’ lies inside cylinder
                return true;
            }

            var b = dd * mn - nd * md;
            var discr = b * b - a * c;

            if (discr < 0.0f) {
                return false;
            }

            distance = (-b - Mathf.Sqrt(discr)) / a;
            if (distance < 0) return false;
            if (md + (distance * nd) < 0.0f) {
                // Intersection outside cylinder on `p1` side
                if (nd <= 0.0f) return false; // Segment pointing away from endcap
                distance = -md / nd;
                // Keep intersection if Dot(S(t) - p, S(t) - p) <= r^2
                return distance * (k + 2) * (mn + distance) <= 0.0f;
            } else if (md + distance * nd > dd) {
                // Intersection outside cylinder on `p2` side
                if (nd >= 0.0f) return false; // Segment pointing away from endcap
                distance = (dd - md) / nd;
                // Keep intersection if Dot(S(t) - q, S(t) - q) <= r^2
                return k + dd - 2 * md + distance * (2 * (mn - nd) + distance) <= 0.0f;
            }
            return true;
        }

        public bool Raycast(Ray ray) {
            float distance;
            return RaycastCylinder(point1, point2, radius, ray, out distance);
        }

        public bool Raycast(Ray ray, out float distance) {
            return RaycastCylinder(point1, point2, radius, ray, out distance);
        }
    }
}