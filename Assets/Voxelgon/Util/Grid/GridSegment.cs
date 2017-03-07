using System;
using UnityEngine;

namespace Voxelgon.Util.Grid {

    public struct GridSegment : IGridObject {

        // FIELDS

        public readonly GridVector point1;
        public readonly GridVector point2;
        public readonly GridBounds bounds;


        // CONSTRUCTORS

        // create a new GridSegment from two points
        // uses the points hashes so that any two GridSegments produced from the same GridPoints
        // will produce the same struct, regardless of the order of the arguments
        public GridSegment(GridVector node1, GridVector node2) {
            if (node1 == node2) throw new ArgumentException("Nodes are equal");
            var hash1 = node1.GetHashCodeLong();
            var hash2 = node2.GetHashCodeLong();

            if (hash1 > hash2) {
                this.point1 = node1;
                this.point2 = node2;
            } else {
                this.point2 = node1;
                this.point1 = node2;
            }

            bounds = GridBounds.FromPoints(point1, point2);
        }


        // PROPERTIES

        // length of the GridSegment
        public float Length {
            get { return Mathf.Sqrt(SqrLength); }
        }

        // squared length of the GridSegment
        public float SqrLength {
            get { return (point1 - point2).sqrMagnitude; }
        }

        // Vector3 connecting the two points
        public Vector3 Tangent {
            get { return point1 - point2; }
        }

        // bounding box of the GridSegment
        public GridBounds Bounds {
            get { return bounds; }
        }

        // smallest vector with integer components that is a multiple of this GridSegment
        private GridVector Unit {
            get {
                int dx = point2.x - point1.x;
                int dy = point2.y - point1.y;
                int dz = point2.z - point1.z;

                uint gcd = MathVG.GCD(
                            (uint)System.Math.Abs(dx),
                            MathVG.GCD(
                                (uint)System.Math.Abs(dy),
                                (uint)System.Math.Abs(dz)));

                return new GridVector(dx / gcd, dy / gcd, dz / gcd);
            }
        }


        // METHODS


        // IGridObject
        // raycast onto this GridSegment
        // lines are infinitely thin, so cant collide with a ray
        // ALWAYS FALSE
        public bool Raycast(Ray ray, float maxDist = float.MaxValue) { return false; }


        // check if a point is on this line segment
        public bool Intersect(GridVector testPoint) {
            if (!bounds.Intersect(testPoint)) return false;
            var unit = Unit;
            return (unit.x % (testPoint.x - point1.x) == 0
                 && unit.y % (testPoint.y - point1.y) == 0
                 && unit.z % (testPoint.z - point1.z) == 0);
        }

        // check if two GridSegments are equal
        public override bool Equals(object obj) {
            if (obj is GridSegment) {
                return ((GridSegment)obj == this);
            }
            return false;
        }

        // get the hashcode of the GridSegment 
        public override int GetHashCode() {
            unchecked {
                int hash = 17;
                hash = (hash * 23) + point1.GetHashCode();
                hash = (hash * 23) + point2.GetHashCode();
                return hash;
            }
        }

        // string representation of this object
        public override string ToString() {
            return point1.ToString() + " " + point2.ToString();
        }

        // == operator
        public static bool operator ==(GridSegment a, GridSegment b) {
            return (a.point1 == b.point1 && a.point2 == b.point2);
        }

        // != operator
        public static bool operator !=(GridSegment a, GridSegment b) {
            return (a.point1 != b.point1 || a.point2 != b.point2);
        }
    }
}