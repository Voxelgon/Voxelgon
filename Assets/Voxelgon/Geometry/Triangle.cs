using System;
using System.Collections.Generic;
using UnityEngine;


namespace Voxelgon.Geometry {

    public class Triangle : IPolygon {

        //FIELDS

        private readonly Vector3[] _vertices = new Vector3[3];

        //CONSTRUCTORS

        //create a simple default triangle
        public Triangle() {
            _vertices[0] = Vector3.zero;
            _vertices[1] = Vector3.forward;
            _vertices[2] = Vector3.left;
        }

        //create a triangle from the three given points
        public Triangle(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3) {
            _vertices[0] = vertex1;
            _vertices[1] = vertex2;
            _vertices[2] = vertex3;
        }

        //PROPERTIES

        //IPolygon
        //access each vertex individually by its index
        public Vector3 this[int index] {
            get { return _vertices[index]; }
            set { _vertices[index] = value; }
        }

        //IPolygon
        //the normal of the clockwise polygon
        public Vector3 Normal {
            get {
                Vector3 delta1 = _vertices[1] - _vertices[0];
                Vector3 delta2 = _vertices[2] - _vertices[0];

                return Vector3.Cross(delta1, delta2).normalized;
            }
        }

        //IPolygon
        //is the polygon convex?
        public bool IsConvex { 
            //triangles are always convex
            get { return true; } 
        }

        //IPolygon
        //is the polygon valid?
        // must have >= 3 vertices
        public bool IsValid { 
            get {
                bool valid = true;
                valid &= (!_vertices[0].Equals(_vertices[1]));
                valid &= (!_vertices[1].Equals(_vertices[2]));
                valid &= (!_vertices[2].Equals(_vertices[0]));
                return valid;
            }
        }

        //IPolygon
        //the area of the polygon
        public float Area {
            get {
                Vector3 delta1 = _vertices[1] - _vertices[0];
                Vector3 delta2 = _vertices[2] - _vertices[0];

                return 0.5f * Vector3.Cross(delta1, delta2).magnitude;
            }
        }

        //IPolygon
        //the number of vertices in the polygon
        public int VertexCount {
            get { return 3; }
        }

        //METHODS

        //IPolygon
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

        //IPolygon
        //returns whether or not `point` is on or inside the polygon
        public bool Contains(Vector3 point) {
            var triangle1 = new Triangle(_vertices[0], _vertices[1], point);
            var triangle2 = new Triangle(_vertices[1], _vertices[2], point);
            var triangle3 = new Triangle(_vertices[2], _vertices[0], point);

            return (triangle1.WindingOrder(Normal) != -1
            && triangle2.WindingOrder(Normal) != -1
            && triangle3.WindingOrder(Normal) != -1);
        }

        //IPolygon
        //reverses the polygon's winding order
        public void Reverse() {
            Array.Reverse(_vertices);
        }

        //IPolygon
        //if the polygon is counter-clockwise, reverse it so it is clockwise
        public void EnsureClockwise(Vector3 normal) {
            if (WindingOrder(normal) == -1) {
                Reverse();
            }
        }

        //IPolygon
        //returns and array of triangles that make up the polygon
        public List<Triangle> ToTriangles() {
            return new List<Triangle> {this};
        }
    }
}