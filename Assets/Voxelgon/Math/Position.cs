using UnityEngine;

namespace Voxelgon.Math {
    public struct Position {
        
        public bool Equals(Position other)
        {
            return _x == other._x && _y == other._y && _z == other._z;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Position && Equals((Position) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _x.GetHashCode();
                hashCode = (hashCode*397) ^ _y.GetHashCode();
                hashCode = (hashCode*397) ^ _z.GetHashCode();
                return hashCode;
            }
        }

        readonly short _x;
        readonly short _y;
        readonly short _z;

        public Position(short x, short y, short z) {
            _x = x;
            _y = y;
            _z = z;
        }

        //ToDo: Consider what float equality means here
        public Position(float x, float y, float z) {
            _x = (short) x;
            _y = (short) y;
            _z = (short) z;
        }

        public Position(Vector3 v) {
            _x = (short) v.x;
            _y = (short) v.y;
            _z = (short) v.z;
        }

        public static explicit operator Position(Vector3 v) {
            return new Position(v);
        }

        public static explicit operator Vector3(Position pos) {
            return new Vector3(pos._x, pos._y, pos._z);
        }

        public static bool operator ==(Position a, Position b) {
            return (a._x == b._x && a._y == b._y && a._z == b._z);
        }

        public static bool operator !=(Position a, Position b) {
            return (a._x != b._x || a._y != b._y || a._z != b._z);
        }
    }
}