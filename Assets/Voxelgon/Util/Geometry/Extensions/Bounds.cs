using UnityEngine;

namespace Voxelgon.Util.Geometry {

    public static class BoundsExtension {

        public static float SurfaceArea(this Bounds bounds) {
            return 2 * (
                (bounds.size.x * bounds.size.y)
              + (bounds.size.y * bounds.size.z)
              + (bounds.size.z * bounds.size.x));
        }

        public static float Volume(this Bounds bounds) {
            return bounds.size.x * bounds.size.y * bounds.size.z;
        }

        public static void DrawDebug(this Bounds bounds, Color color, float duration) {
            Debug.DrawLine(new Vector3(bounds.min.x, bounds.min.y, bounds.min.z), 
                            new Vector3(bounds.max.x, bounds.min.y, bounds.min.z), color, duration);
            Debug.DrawLine(new Vector3(bounds.min.x, bounds.min.y, bounds.min.z), 
                            new Vector3(bounds.min.x, bounds.max.y, bounds.min.z), color, duration);
            Debug.DrawLine(new Vector3(bounds.min.x, bounds.min.y, bounds.min.z), 
                            new Vector3(bounds.min.x, bounds.min.y, bounds.max.z), color, duration);

            Debug.DrawLine(new Vector3(bounds.max.x, bounds.min.y, bounds.min.z), 
                            new Vector3(bounds.max.x, bounds.max.y, bounds.min.z), color, duration);
            Debug.DrawLine(new Vector3(bounds.max.x, bounds.min.y, bounds.min.z), 
                            new Vector3(bounds.max.x, bounds.min.y, bounds.max.z), color, duration);

            Debug.DrawLine(new Vector3(bounds.min.x, bounds.max.y, bounds.min.z), 
                            new Vector3(bounds.max.x, bounds.max.y, bounds.min.z), color, duration);
            Debug.DrawLine(new Vector3(bounds.min.x, bounds.max.y, bounds.min.z), 
                            new Vector3(bounds.min.x, bounds.max.y, bounds.max.z), color, duration);

            Debug.DrawLine(new Vector3(bounds.min.x, bounds.min.y, bounds.max.z), 
                            new Vector3(bounds.min.x, bounds.max.y, bounds.max.z), color, duration);
            Debug.DrawLine(new Vector3(bounds.min.x, bounds.min.y, bounds.max.z), 
                            new Vector3(bounds.max.x, bounds.min.y, bounds.max.z), color, duration);

            Debug.DrawLine(new Vector3(bounds.min.x, bounds.max.y, bounds.max.z), 
                            new Vector3(bounds.max.x, bounds.max.y, bounds.max.z), color, duration);
            Debug.DrawLine(new Vector3(bounds.max.x, bounds.min.y, bounds.max.z), 
                            new Vector3(bounds.max.x, bounds.max.y, bounds.max.z), color, duration);
            Debug.DrawLine(new Vector3(bounds.max.x, bounds.max.y, bounds.min.z), 
                            new Vector3(bounds.max.x, bounds.max.y, bounds.max.z), color, duration);
        }
    }
}