using UnityEngine;
using System;

namespace Voxelgon.Util.Grid {

    public struct GridBounds {

        // FIELDS

        public readonly GridPoint min;
        public readonly GridPoint max;


        // CONSTRUCTORS

        private GridBounds(GridPoint min, GridPoint max) {
            this.min = min;
            this.max = max;
        }


        // PROPERTIES

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
                return (max.x - min.x) * (max.y - min.y) * (max.z - min.z);
            }
        }

        public int SurfaceArea {
            get {
                int xSize = max.x - min.x;
                int ySize = max.y - min.y;
                int zSize = max.z - min.z;
                return 2 * ((xSize * ySize) + (ySize * zSize) + (zSize * xSize));
            }
        }

        public GridPoint GridCenter {
            get { return new GridPoint((min.x + max.x) / 2, (min.y + max.y) / 2, (min.z + max.z) / 2); }
        }

        public Vector3 Center {
           get { return new Vector3((min.x + max.x) / 2.0f, (min.y + max.y) / 2.0f, (min.z + max.z) / 2.0f); }
        }


        // METHODS

        public static GridBounds FromPoints(GridPoint point1, GridPoint point2) {
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

            return new GridBounds(new GridPoint(minX, minY, minZ), new GridPoint(maxX, maxY, maxZ));
        }

        public static GridBounds Combine(GridBounds bounds1, GridBounds bounds2) {
            return new GridBounds(
                        new GridPoint(
                            Math.Min(bounds1.min.x, bounds2.min.x),
                            Math.Min(bounds1.min.y, bounds2.min.y),
                            Math.Min(bounds1.min.z, bounds2.min.z)),
                        new GridPoint(
                            Math.Max(bounds1.max.x, bounds2.max.x),
                            Math.Max(bounds1.max.y, bounds2.max.y),
                            Math.Max(bounds1.max.z, bounds2.max.z)));
        }

        public bool ContainsPoint(GridPoint p) {
            return (p.x >= min.x && p.x <= max.x
                 && p.y >= min.y && p.y <= max.y
                 && p.z >= min.z && p.z <= max.z);
        }
    }
}