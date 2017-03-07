using UnityEngine;

namespace Voxelgon.Util.Grid {

    public struct GridPoint {

        // FIELDS

        public readonly short x;
        public readonly short y;
        public readonly short z;


        // CONSTRUCTORS

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


        //PROPERTIES

        public long MortonCode {
            get {
                long morton = MortonSplit(z);
                morton = (morton << 1) | MortonSplit(y);
                morton = (morton << 1) | MortonSplit(x);
                return morton;
            }
        }


        // METHODS

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
            hash |= (ushort)(y << 16);
            hash |= (ushort)(z << 32);
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

        private static long MortonSplit(short n) {
            // from: http://www.forceflow.be/2013/10/07/morton-encodingdecoding-through-bit-interleaving-implementations/

            long s = n + short.MaxValue;
            //s = (s | s << 32) & 0x1f00000000ffff;
            s = (s | s << 16) & 0x1f0000ff0000ff;
            s = (s | s << 8)  & 0x100f00f00f00f00f;
            s = (s | s << 4)  & 0x10c30c30c30c30c3;
            s = (s | s << 2)  & 0x1249249249249249;
            return s;
        }
    }
}