using UnityEngine;
using Voxelgon.Util;

namespace Voxelgon.Geometry {

    public struct Sphere : IBoundable {

        // FIELDS

        public Vector3 center;
        public float radius;


        // PROPERTIES

        public Bounds Bounds {
            get {
                return new Bounds(center, Vector3.one * radius);
            }
        }


        // CONSTRUCTORS

        public Sphere(Vector3 center, float radius) {
            this.center = center;
            this.radius = radius;
        }


        // METHODS
        
        public static bool RaycastSphere(Vector3 center, float radius, Ray ray, out float distance) {
            Vector3 delta = ray.origin - center;
            var b = Vector3.Dot(delta, ray.direction);
            var c = delta.sqrMagnitude - (radius * radius);
            // Exit if ray's origin outside s (c > 0) and r pointing away from s (b > 0)
            if (c > 0.0f && b > 0.0f) {
                distance = 0;
                return false;
            }

            var discr = (b * b) - c;
            // A negative discriminant corresponds to ray missing sphere
            if (discr < 0) {
                distance = 0;
                return false;
            }

            // Ray now found to intersect sphere, compute smallest distance value of intersection
            distance = -b - Mathf.Sqrt(discr);

            // If distance is negative, ray started inside sphere so clamp to zero
            if (distance < 0) distance = 0;
            return true;
        }

        public bool Raycast(Ray ray) {
            float distance;
            return RaycastSphere(center, radius, ray, out distance);
        }

        public bool Raycast(Ray ray, out float distance) {
            return RaycastSphere(center, radius, ray, out distance);
        }
    }
}