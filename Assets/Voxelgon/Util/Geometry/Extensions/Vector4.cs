using UnityEngine;

namespace Voxelgon.Util.Geometry {

    public static class Vector4Extensions {

        public static Vector4 Round(this Vector4 vector) {
            vector.x = Mathf.RoundToInt(vector.x);
            vector.y = Mathf.RoundToInt(vector.y);
            vector.z = Mathf.RoundToInt(vector.z);
            vector.w = Mathf.RoundToInt(vector.w);
            return vector;
        }

        public static Vector4 Modulus(this Vector4 vector, float mod) {
            vector.x %= mod;
            vector.y %= mod;
            vector.z %= mod;
            vector.w %= mod;
            return vector;
        }

        public static bool Approximately(this Vector4 vector, Vector4 other) {
            return (Mathf.Approximately(vector.x, other.x)
                 && Mathf.Approximately(vector.y, other.y)
                 && Mathf.Approximately(vector.z, other.z)
                 && Mathf.Approximately(vector.w, other.w));
        }

        public static Vector4 Average(this Vector4 vector1, Vector4 vector2) {
            return new Vector4((vector1.x + vector2.x) / 2,
                               (vector1.y + vector2.y) / 2,
                               (vector1.z + vector2.z) / 2,
                               (vector1.w + vector2.w) / 2);
        }

        public static Vector4 Abs(this Vector4 vector) {
            return new Vector4(Mathf.Abs(vector.x),
                               Mathf.Abs(vector.y),
                               Mathf.Abs(vector.z),
                               Mathf.Abs(vector.w));
        }

        public static Vector4 Sqrt(this Vector4 vector) {
            return new Vector4(Mathf.Sqrt(vector.x),
                               Mathf.Sqrt(vector.y),
                               Mathf.Sqrt(vector.z),
                               Mathf.Sqrt(vector.w));
        }

        public static Vector2 xy(this Vector4 v) {
            return new Vector2(v.x, v.y);
        }

        public static Vector4 xz(this Vector4 v) {
            return new Vector2(v.x, v.z);
        }

        public static Vector4 yz(this Vector4 v) {
            return new Vector2(v.y, v.z);
        }
    }
}
