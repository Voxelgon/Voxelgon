using System;
using UnityEngine;

namespace Voxelgon.Geometry2D {
    public struct Matrix2x2 {
        private float _m00;
        private float _m01;
        private float _m10;
        private float _m11;

        public Matrix2x2(Vector2 col0, Vector2 col1) {
            _m00 = col0.x;
            _m01 = col1.x;
            _m10 = col0.y;
            _m11 = col1.y;
        }

        public Matrix2x2(float m00, float m01, float m10, float m11) {
            _m00 = m00;
            _m01 = m01;
            _m10 = m10;
            _m11 = m11;
        }

        public float this[int row, int column] {
            get {
                switch (row) {
                    case 0:
                        switch (column) {
                            case 0: return _m00;
                            case 1: return _m01;
                        }
                        break;
                    case 1:
                        switch (column) {
                            case 0: return _m10;
                            case 1: return _m11;
                        }
                        break;
                }
                throw new IndexOutOfRangeException();
            }

            set {
                if (row > 1 || row < 0 || column > 1 || column < 0) throw new IndexOutOfRangeException();
                switch (row) {
                    case 0:
                        switch (column) {
                            case 0: _m00 = value; break;
                            case 1: _m01 = value; break;
                        }
                        break;
                    case 1:
                        switch (column) {
                            case 0: _m10 = value; break;
                            case 1: _m11 = value; break;
                        }
                        break;
                }
            }
        }

        // returns the identity matrix (Read Only).
        public static Matrix2x2 Identity {
            get { return new Matrix2x2(1, 0, 0, 1); }
        }

        // returns a matrix with all elements set to zero (Read Only).
        public static Matrix2x2 Zero {
            get { return new Matrix2x2(0, 0, 0, 0); }
        }

        // returns the determinant of the matrix.
        public float Determinant {
            get { return (_m00 * _m11) - (_m01 * _m10); }
        }

        // returns the inverse of this matrix (Read Only).
        public Matrix2x2 Inverse {
            get {
                float c = 1 / Determinant;
                return new Matrix2x2(c * _m11, -c * _m10, -c * _m01, c * _m00);
            }
        }

        // is this the identity matrix?
        public bool IsIdentity {
            get { return Equals(Identity); }
        }

        // returns the transpose of this matrix (Read Only).
        public Matrix2x2 Transpose {
            get { return new Matrix2x2(_m00, _m10, _m01, _m11); }
        }


        // METHODS

        // returns a rotation matrix
        public static Matrix2x2 Rotation(float angle) {
            var cos = Mathf.Cos(angle);
            var sin = Mathf.Sin(angle);
            return new Matrix2x2(cos, sin, -sin, cos);
        }

        // returns a column of the matrix
        public Vector2 GetColumn(int i) {
            if (i < 0 || i > 1) throw new IndexOutOfRangeException();
            switch (i) {
                case 0: return new Vector2(_m00, _m10);
                case 1: return new Vector2(_m01, _m11);
            }
            throw new IndexOutOfRangeException();
        }

        // returns a row of the matrix
        public Vector2 GetRow(int i) {
            switch (i) {
                case 0: return new Vector2(_m00, _m01);
                case 1: return new Vector2(_m10, _m11);
            }
            throw new IndexOutOfRangeException();
        }

        // solves the matrix for the output `v`
        public Vector2 Solve(Vector2 v) {
            return Inverse * v;
        }

        public Vector3 Multiply(Vector2 v) {
            return new Vector2((v.x * _m00) + (v.y * _m01), (v.x * _m10) + (v.y * _m11));
        }

        // returns the hash code of the matrix
        public override int GetHashCode() {
            unchecked {
                int hash = 17;
                hash = hash * 23 + GetRow(0).GetHashCode();
                hash = hash * 23 + GetRow(1).GetHashCode();
                return hash;
            }
        }

        // are the matrices equal?
        public override bool Equals(object other) {
            return GetHashCode() == other.GetHashCode();
        }

        public static Matrix2x2 operator *(Matrix2x2 m, float c) {
            return new Matrix2x2(m._m00 * c, m._m01 * c, m._m10 * c, m._m11 * c);
        }

        public static Matrix2x2 operator *(float c, Matrix2x2 m) {
            return m * c;
        }

        public static Vector2 operator *(Matrix2x2 lhs, Vector2 v) {
            return lhs.Multiply(v);
        }

        public static Matrix2x2 operator *(Matrix2x2 lhs, Matrix2x2 rhs) {
            return new Matrix2x2(lhs * rhs.GetColumn(0), lhs * rhs.GetColumn(1));
        }

        public static Matrix2x2 operator /(Matrix2x2 m, float c) {
            return m * (1 / c);
        }

        public static bool operator ==(Matrix2x2 lhs, Matrix2x2 rhs) {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Matrix2x2 lhs, Matrix2x2 rhs) {
            return !lhs.Equals(rhs);
        }

        public static explicit operator Matrix4x4(Matrix2x2 m) {
            var m4x4 = Matrix4x4.zero;
            m4x4.m00 = m._m00;
            m4x4.m01 = m._m01;
            m4x4.m10 = m._m10;
            m4x4.m11 = m._m11;
            return m4x4;
        }

        public static explicit operator Matrix2x2(Matrix4x4 m) {
            return new Matrix2x2(m.m00, m.m01, m.m10, m.m11);
        }
    }
}