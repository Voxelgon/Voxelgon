using System;
using UnityEngine;

namespace Voxelgon.Util.Grid {

    public struct GridSegment : IGridObject {

        public readonly GridPoint point1;
        public readonly GridPoint point2;
        public readonly GridBounds bounds;

        public GridSegment(GridPoint node1, GridPoint node2) {
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

        public float Length {
            get { return Mathf.Sqrt(SqrLength); }
        }

        public float SqrLength {
            get { return (point1 - point2).sqrMagnitude; }
        }

        public Vector3 Tangent {
            get { return point1 - point2; }
        }

        public GridBounds Bounds {
            get { return bounds; }
        }

        public bool ContainsNode(GridPoint node) {
            return true;

        }

        public override bool Equals(object obj) {
            if (obj.GetType() == typeof(GridSegment)) {
                return ((GridSegment)obj == this);
            }
            return false;
        }

        public override int GetHashCode() {
            unchecked {
                int hash = 17;
                hash = (hash * 23) + point1.GetHashCode();
                hash = (hash * 23) + point2.GetHashCode();
                return hash;
            }
        }

        public override string ToString() {
            return point1.ToString() + " " + point2.ToString();
        }

        public static bool operator ==(GridSegment a, GridSegment b) {
            return (a.point1 == b.point1 && a.point2 == b.point2);
        }

        public static bool operator !=(GridSegment a, GridSegment b) {
            return (a.point1 != b.point1 || a.point2 != b.point2);
        }
    }
}