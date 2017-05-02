using UnityEngine;

namespace Voxelgon.Geometry {

    public static class Vector3Extensions {

        public static Vector3 Round(this Vector3 vector) {
            vector.x = Mathf.RoundToInt(vector.x);
            vector.y = Mathf.RoundToInt(vector.y);
            vector.z = Mathf.RoundToInt(vector.z);
            return vector;
        }

        public static Vector3 Modulus(this Vector3 vector, float mod) {
            vector.x %= mod;
            vector.y %= mod;
            vector.z %= mod;
            return vector;
        }

        public static bool Approximately(this Vector3 vector, Vector3 other) {
            return (Mathf.Approximately(vector.x, other.x)
                 && Mathf.Approximately(vector.y, other.y)
                 && Mathf.Approximately(vector.z, other.z));
        }

        public static Vector3 Average(this Vector3 vector1, Vector3 vector2) {
            return new Vector3((vector1.x + vector2.x) / 2,
                               (vector1.y + vector2.y) / 2,
                               (vector1.z + vector2.z) / 2);
        }

        public static Vector3 Abs(this Vector3 vector) {
            return new Vector3(Mathf.Abs(vector.x),
                               Mathf.Abs(vector.y),
                               Mathf.Abs(vector.z));
        }

        public static Bounds CalcBounds(this Vector3 vector1, Vector3 vector2) {
            var bounds = new Bounds();

            var min = new Vector3(
                Mathf.Min(vector1.x, vector2.x),
                Mathf.Min(vector1.y, vector2.y),
                Mathf.Min(vector1.z, vector2.z));
            var max = new Vector3(
                Mathf.Min(vector1.x, vector2.x),
                Mathf.Min(vector1.y, vector2.y),
                Mathf.Min(vector1.z, vector2.z));
            bounds.SetMinMax(min, max);
            return bounds;
        }

        public static Vector2 xy(this Vector3 v) {
            return new Vector2(v.x, v.y);
        }

        public static Vector3 xz(this Vector3 v) {
            return new Vector2(v.x,v.z);
        }

        public static Vector3 yz(this Vector3 v) {
            return new Vector2(v.y, v.z);
        }
    }
}
