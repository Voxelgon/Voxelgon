using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Voxelgon.Util.Grid {

    public struct GridBounds : IGridObject {

        // FIELDS

        public readonly GridVector min;
        public readonly GridVector max;


        // CONSTRUCTORS

        private GridBounds(GridVector min, GridVector max) {
            this.min = min;
            this.max = max;
        }


        // PROPERTIES

        // IGridObject
        public GridBounds Bounds {
            get { return this; }
        }

        public int xSize {
            get { return max.x - min.x; }
        }

        public int ySize {
            get { return max.z - min.z; }
        }

        public int zSize {
            get { return max.z - min.z; }
        }

        public int Volume {
            get {
                return (xSize) * (ySize) * (zSize);
            }
        }

        public int SurfaceArea {
            get {
                return 2 * ((xSize * ySize) + (ySize * zSize) + (zSize * xSize));
            }
        }

        public GridVector GridCenter {
            get { return new GridVector((min.x + max.x) / 2, (min.y + max.y) / 2, (min.z + max.z) / 2); }
        }

        public Vector3 Center {
            get { return new Vector3((min.x + max.x) / 2.0f, (min.y + max.y) / 2.0f, (min.z + max.z) / 2.0f); }
        }


        // METHODS

        public static GridBounds FromPoints(GridVector point1, GridVector point2) {
            short minX;
            short minY;
            short minZ;

            short maxX;
            short maxY;
            short maxZ;

            if (point1.x < point2.x) {
                minX = point1.x;
                maxX = point2.x;
            } else {
                minX = point2.x;
                maxX = point1.x;
            }

            if (point1.y < point2.y) {
                minY = point1.y;
                maxY = point2.y;
            } else {
                minY = point2.y;
                maxY = point1.y;
            }

            if (point1.z < point2.z) {
                minZ = point1.z;
                maxZ = point2.z;
            } else {
                minZ = point2.z;
                maxZ = point1.z;
            }

            return new GridBounds(new GridVector(minX, minY, minZ), new GridVector(maxX, maxY, maxZ));
        }

        public static GridBounds Combine(GridBounds bounds1, GridBounds bounds2) {
            return new GridBounds(
                        new GridVector(
                            Math.Min(bounds1.min.x, bounds2.min.x),
                            Math.Min(bounds1.min.y, bounds2.min.y),
                            Math.Min(bounds1.min.z, bounds2.min.z)),
                        new GridVector(
                            Math.Max(bounds1.max.x, bounds2.max.x),
                            Math.Max(bounds1.max.y, bounds2.max.y),
                            Math.Max(bounds1.max.z, bounds2.max.z)));
        }

        public static GridBounds Combine<T>(List<T> objects) where T : IGridObject {
            if (objects.Count == 0) throw new ArgumentOutOfRangeException("objects", "Empty list!");
            var minX = objects.Min(o => o.Bounds.min.x);
            var minY = objects.Min(o => o.Bounds.min.y);
            var minZ = objects.Min(o => o.Bounds.min.z);

            var maxX = objects.Max(o => o.Bounds.max.x);
            var maxY = objects.Max(o => o.Bounds.max.y);
            var maxZ = objects.Max(o => o.Bounds.max.z);

            return new GridBounds(
                        new GridVector(minX, minY, minZ),
                        new GridVector(maxX, maxY, maxZ));
        }


        // IGridObject
        // raycast onto this bounding box
        // uses a minimum distance of 0 and default maximum distance of float.MaxValue
        // algorithm from http://psgraphics.blogspot.com/2016/02/new-simple-ray-box-test-from-andrew.html
        public bool Raycast(Ray ray, float maxDist = float.MaxValue) {
            return Raycast(ray, 0, maxDist);
        }

        // raycast onto this bounding box with the given minimum and maximum distances
        public bool Raycast(Ray ray, float minDist, float maxDist) {
            for (int axis = 0; axis < 3; axis++) {
                float invD = 1.0f / ray.direction[axis];
                float t0;
                float t1;
                if (invD >= 0.0f) {
                    t0 = (min[axis] - ray.origin[axis]) * invD;
                    t1 = (max[axis] - ray.origin[axis]) * invD;
                } else {
                    t1 = (min[axis] - ray.origin[axis]) * invD;
                    t0 = (max[axis] - ray.origin[axis]) * invD;
                }

                minDist = Mathf.Max(t0, minDist);
                maxDist = Mathf.Min(t1, maxDist);
                if (maxDist <= minDist) return false;
            }
            return true;
        }

        // check if given point is in this AABB
        public bool Intersect(GridVector p) {
            return (p.x >= min.x && p.x <= max.x
                 && p.y >= min.y && p.y <= max.y
                 && p.z >= min.z && p.z <= max.z);
        }

        // compute hashcode for hashMaps
        public override int GetHashCode() {
            unchecked {
                int hash = 17;
                hash = (hash * 23) + min.GetHashCode();
                hash = (hash * 23) + max.GetHashCode();
                return hash;
            }
        }

        // check if two GridBounds are equal
        public override bool Equals(object obj) {
            if (obj is GridBounds) {
                return ((GridBounds)obj == this);
            }
            return false;
        }

        // draw this GridBounds into the world with given color and duration
        public void DrawDebug(Color color, float duration) {
            Debug.DrawLine(new Vector3(min.x, min.y, min.z), new Vector3(max.x, min.y, min.z), color, duration);
            Debug.DrawLine(new Vector3(min.x, min.y, min.z), new Vector3(min.x, max.y, min.z), color, duration);
            Debug.DrawLine(new Vector3(min.x, min.y, min.z), new Vector3(min.x, min.y, max.z), color, duration);

            Debug.DrawLine(new Vector3(max.x, min.y, min.z), new Vector3(max.x, max.y, min.z), color, duration);
            Debug.DrawLine(new Vector3(max.x, min.y, min.z), new Vector3(max.x, min.y, max.z), color, duration);

            Debug.DrawLine(new Vector3(min.x, max.y, min.z), new Vector3(max.x, max.y, min.z), color, duration);
            Debug.DrawLine(new Vector3(min.x, max.y, min.z), new Vector3(min.x, max.y, max.z), color, duration);

            Debug.DrawLine(new Vector3(min.x, min.y, max.z), new Vector3(min.x, max.y, max.z), color, duration);
            Debug.DrawLine(new Vector3(min.x, min.y, max.z), new Vector3(max.x, min.y, max.z), color, duration);

            Debug.DrawLine(new Vector3(min.x, max.y, max.z), new Vector3(max.x, max.y, max.z), color, duration);
            Debug.DrawLine(new Vector3(max.x, min.y, max.z), new Vector3(max.x, max.y, max.z), color, duration);
            Debug.DrawLine(new Vector3(max.x, max.y, min.z), new Vector3(max.x, max.y, max.z), color, duration);
        }

        public static bool operator ==(GridBounds a, GridBounds b) {
            return (a.min == b.min && a.max == b.max);
        }

        public static bool operator !=(GridBounds a, GridBounds b) {
            return (a.min != b.min || a.max != b.max);
        }

    }
}