using UnityEngine;
using System;
using Voxelgon.Util;

namespace Voxelgon.Geometry {

    public struct GridSegment {

        // FIELDS

        public readonly GridVector point1;
        public readonly GridVector point2;

        // CONSTRUCTORS

        // create a new WallEdge from two points
        // uses the points hashes so that any two WallEdges produced from the same GridPoints
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
        }


        // PROPERTIES

        // length of the WallEdge
        public float Length {
            get { return Mathf.Sqrt(SqrLength); }
        }

        // squared length of the WallEdge
        public float SqrLength {
            get { return (point1 - point2).sqrMagnitude; }
        }

        // Vector3 connecting the two points
        public Vector3 Tangent {
            get { return point2 - point1; }
        }

        public Vector3 Center {
            get { return ((Vector3)point1 + (Vector3)point2) / 2; }
        }

        public Bounds Bounds {
            get { 
                return ((Vector3) point1).CalcBounds((Vector3) point2);
            }
        }

        // smallest vector with integer components that is a multiple of this WallEdge
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

        // check if two WallEdges are equal
        public override bool Equals(object obj) {
            if (obj is GridSegment) {
                return ((GridSegment)obj == this);
            }
            return false;
        }

        // get the hashcode of the WallEdge 
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