using UnityEngine;
using Voxelgon.Geometry;
using Voxelgon.Util;

namespace Voxelgon.Geometry2D {
    public struct Segment2D {
        #region Fields

        private readonly Vector2 _point1;
        private readonly Vector2 _point2;

        #endregion

        #region Constructor

        public Segment2D(Vector2 point1, Vector2 point2) {
            _point1 = point1;
            _point2 = point2;
        }

        #endregion

        #region Properties

        /// <summary>
        /// First point
        /// </summary>
        public Vector2 Point1 {
            get { return _point1; }
        }

        /// <summary>
        /// Second point
        /// </summary>
        public Vector2 Point2 {
            get { return _point2; }
        }

        /// <summary>
        /// Vector from Point1 to Point1
        /// </summary>
        public Vector2 Delta {
            get { return _point2 - _point1; }
        }

        /// <summary>
        /// Length of the segment Squared
        /// </summary>
        public float SqrLength {
            get { return Delta.sqrMagnitude; }
        }

        /// <summary>
        /// Length of the segment
        /// </summary>
        public float Length {
            get { return Delta.magnitude; }
        }

        /// <summary>
        /// Slope of the segment
        /// </summary>
        public float Slope {
            get {
                var delta = Delta;
                if (Mathf.Abs(delta.x) < 0.0001f) {
                    return (delta.y > 0) ? float.PositiveInfinity : float.NegativeInfinity;
                }
                return delta.y / delta.x;
            }
        }

        /// <summary>
        /// AABB around the segment
        /// </summary>
        public Bounds2D Bounds {
            get { return new Bounds2D(_point1, _point2); }
        }

        /// <summary>
        /// Raycast onto the line segment
        /// </summary>
        /// <param name="ray">Ray to test with</param>
        /// <returns>if the ray hit the segment</returns>
        public bool Raycast(Ray2D ray) {
            return RaycastSegment(ray, _point1, _point2);
        }

        /// <summary>
        /// Raycast onto the line segment
        /// </summary>
        /// <param name="ray">Ray to test with</param>
        /// <param name="distance">distance output</param>
        /// <returns>if the ray hit the segment</returns>
        public bool Raycast(Ray2D ray, out float distance) {
            return RaycastSegment(ray, _point1, _point2, out distance);
        }

        /// <summary>
        /// Raycast onto the line segment
        /// </summary>
        /// <param name="ray">Ray to test with</param>
        /// <param name="intersection">intersection point output</param>
        /// <returns>if the ray hit the segment</returns>
        public bool Raycast(Ray2D ray, out Vector2 intersection) {
            return RaycastSegment(ray, _point1, _point2, out intersection);
        }

        /// <summary>
        /// Find the intersection point of two segments
        /// </summary>
        /// <param name="other">Segment2D to test against</param>
        /// <param name="intersection">intersection point output</param>
        /// <returns>if the segments intersect</returns>
        public bool Intersect(Segment2D other, out Vector2 intersection) {
            return IntersectSegments(_point1, _point2, other._point1, other._point2, out intersection);
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Raycast onto a line segment
        /// </summary>
        /// <param name="ray">Ray2D to test with</param>
        /// <param name="point1">first point of segment</param>
        /// <param name="point2">second point of segment</param>
        /// <param name="distance">distance output</param>
        /// <returns>if the ray hit the segment</returns>
        public static bool RaycastSegment(Ray2D ray, Vector2 point1, Vector2 point2, out float distance) {
            var v1 = ray.origin - point1;
            var v2 = point2 - point1;
            var v3 = ray.direction.Ortho();

            var denom = 1 / Vector2.Dot(v2, v3);
            var t1 = denom * Vector3.Cross(v2, v1).z * -1;
            var t2 = denom * Vector2.Dot(v1, v3);

            distance = t1;
            return (t1 > 0 && t2 > 0 && t2 < 1);
        }

        /// <summary>
        /// Raycast onto a line segment
        /// </summary>
        /// <param name="ray">Ray2D to test with</param>
        /// <param name="point1">first point of segment</param>
        /// <param name="point2">second point of segment</param>
        /// <param name="intersection">intersection point output</param>
        /// <returns>if the ray hit the segment</returns>
        public static bool RaycastSegment(Ray2D ray, Vector2 point1, Vector2 point2, out Vector2 intersection) {
            float distance;
            var result = RaycastSegment(ray, point1, point2, out distance);

            intersection = ray.direction * distance;
            return result;
        }

        /// <summary>
        /// Raycast onto a line segment
        /// </summary>
        /// <param name="ray">Ray2D to test with</param>
        /// <param name="point1">first point of segment</param>
        /// <param name="point2">second point of segment</param>
        /// <returns>if the ray hit the segment</returns>
        public static bool RaycastSegment(Ray2D ray, Vector2 point1, Vector2 point2) {
            float temp;
            return RaycastSegment(ray, point1, point2, out temp);
        }

        /// <summary>
        /// Find the intersection of two line segments
        /// </summary>
        /// <param name="a1">first point of segment A</param>
        /// <param name="a2">second point of segment A</param>
        /// <param name="b1">first point of segment B</param>
        /// <param name="b2">seconf point of segment B</param>
        /// <param name="intersection">intersection point</param>
        /// <returns>if the two segments intersect</returns>
        public static bool IntersectSegments(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, out Vector2 intersection) {
            var v1 = a1 - b1;
            var v2 = b2 - b1;
            var v3 = (a2 - a1).Ortho();

            var denom = 1 / Vector2.Dot(v2, v3);
            var t1 = denom * Vector3.Cross(v2, v1).z * -1;
            var t2 = denom * Vector2.Dot(v1, v3);

            intersection = b1 + (v2 * t2);
            return (t1 > 0 && t1 < 1 && t2 > 0 && t2 < 1);
        }

        #endregion
    }
}