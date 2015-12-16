using System;
using UnityEngine;


namespace Voxelgon.Geometry {

    public class Triangle : IPolygon {

        //FIELDS

        private readonly Vector3[] _vertices = new Vector3[3];

        //CONSTRUCTORS

        public Triangle() {
            _vertices[0] = Vector3.zero;
            _vertices[1] = Vector3.zero;
            _vertices[2] = Vector3.zero;
        }

        public Triangle(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3) {
            _vertices[0] = vertex1;
            _vertices[1] = vertex2;
            _vertices[2] = vertex3;
        }

        //PROPERTIES

        //access each vertex individually by its index
        public Vector3 this[int index] {
            get { return _vertices[index]; }
            set { _vertices[index] = value; }
        }

        //the normal of the clockwise polygon
        public Vector3 Normal {
            get {
                Vector3 delta1 = _vertices[1] - _vertices[0];
                Vector3 delta2 = _vertices[2] - _vertices[0];

                return Vector3.Cross(delta1, delta2).normalized;
            }
        }

        //the area of the polygon
        public float Area {
            get {
                Vector3 delta1 = _vertices[1] - _vertices[0];
                Vector3 delta2 = _vertices[2] - _vertices[0];

                return 0.5f * Vector3.Cross(delta1, delta2).magnitude;
            }
        }

        //the number of vertices in the polygon
        public int VertexCount {
            get { return 3; }
        }

        //METHODS

        //returns the winding order relative to the normal
        // 1 = clockwise
        //-1 = counter-clockwise
        // 0 = all points are colinear
        public int WindingOrder(Vector3 normal) {
            Vector3 delta1 = _vertices[1] - _vertices[0];
            Vector3 delta2 = _vertices[2] - _vertices[0];

            if (Vector3.Angle(delta1, delta2) < 0.01f
                || Vector3.Angle(delta1, delta2) > 179.99f) {
                return 0;
            }

            Vector3 resultNormal = Vector3.Cross(delta1, delta2);

            return (Vector3.Dot(normal, resultNormal) >= 0) ? 1 : -1; 
        }

        //returns whether or not `point` is on or inside the polygon
        public bool Contains(Vector3 point) {
            var triangle1 = new Triangle(_vertices[0], _vertices[1], point);
            var triangle2 = new Triangle(_vertices[1], _vertices[2], point);
            var triangle3 = new Triangle(_vertices[2], _vertices[0], point);

            return (triangle1.WindingOrder(Normal) != -1
            && triangle2.WindingOrder(Normal) != -1
            && triangle3.WindingOrder(Normal) != -1);
        }

        //reverses the polygon's winding order
        public void Reverse() {
            Array.Reverse(_vertices);
        }

        //if the polygon is counter-clockwise, reverse it so it is clockwise
        public void EnsureClockwise(Vector3 normal) {
            if (WindingOrder(normal) == -1) {
                Reverse();
            }
        }
    }
}