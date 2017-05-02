using UnityEngine;

namespace Voxelgon.Geometry {

    public static class Vector2Extensions {

        public static Vector3 xy(this Vector2 v) {
            return new Vector3(v.x, v.y, 0);
        }

        public static Vector3 xz(this Vector2 v) {
            return new Vector3(v.x, 0, v.y);
        }

        public static Vector3 yz(this Vector2 v) {
            return new Vector3(0, v.x, v.y);
        }

        public static Vector2 Ortho(this Vector2 v) {
            return new Vector2(v.y, -v.x);
        }
    }
}
