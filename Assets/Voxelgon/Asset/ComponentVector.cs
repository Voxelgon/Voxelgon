using UnityEngine;

namespace Voxelgon.Asset {
    public class ComponentVector {

        // CONSTRUCTORS

        public ComponentVector() {
            X = 0;
            Y = 0;
            Z = 0;
        }

        public ComponentVector(float x, float y, float z) {
            X = x;
            Y = y;
            Z = z;
        }


        // PROPERTIES

        public float X {get; set;}
        public float Y {get; set;}
        public float Z {get; set;}


        // METHODS

        public override string ToString() {
            return X + ", " + Y + ", " + Z;
        }


        // OPERATORS

        public static implicit operator Vector3(ComponentVector v) {
            return new Vector3(v.X, v.Y, v.Z);
        }

        public static implicit operator ComponentVector(Vector3 v) {
            return new ComponentVector(v.x, v.y, v.z);
        }
    }
}