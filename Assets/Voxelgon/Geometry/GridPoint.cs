using UnityEngine;

namespace Voxelgon.Geometry {
    public struct GridPoint {
        public readonly short x;
        public readonly short y;
        public readonly short z;

        public GridPoint(short x, short y, short z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public GridPoint(float x, float y, float z) {
            this.x = (short)x;
            this.y = (short)y;
            this.z = (short)z;
        }

        public GridPoint(Vector3 v) {
            this.x = (short)v.x;
            this.y = (short)v.y;
            this.z = (short)v.z;
        }

        public override bool Equals(object obj) {
            if (obj.GetType() == typeof(GridPoint)) {
                return ((GridPoint)obj == this);
            }
            return false;
        }

        public override int GetHashCode() {
            unchecked {
                int hash = 17;
                hash = (hash * 23) + z;
                hash = (hash * 23) + y;
                hash = (hash * 23) + z;
                return hash;
            }
        }

        public long GetHashCodeLong() {
            long hash = x;
            hash |= (ushort) (y << 16);
            hash |= (ushort) (z << 32);
            return hash;
            //only 6 of 8 bytes but whatever
        }

        public override string ToString() {
            return "<" + x + ", " + y + ", " + z + ">";
        }

        public static explicit operator GridPoint(Vector3 v) {
            return new GridPoint(v);
        }

        public static explicit operator Vector3(GridPoint pos) {
            return new Vector3(pos.x, pos.y, pos.z);
        }

        public static Vector3 operator -(GridPoint a, GridPoint b) {
            return new Vector3(b.x - a.x, a.y - b.y, a.z - b.z);
        }

        public static bool operator ==(GridPoint a, GridPoint b) {
            return (a.x == b.x && a.y == b.y && a.z == b.z);
        }

        public static bool operator !=(GridPoint a, GridPoint b) {
            return (a.x != b.x || a.y != b.y || a.z != b.z);
        }
    }
}