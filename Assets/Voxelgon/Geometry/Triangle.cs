using System.Collections.Generic;
using UnityEngine;


namespace Voxelgon.Geometry {

    public struct Triangle {
        // FIELDS

        private readonly Vector3 _vertex0;
        private readonly Vector3 _vertex1;
        private readonly Vector3 _vertex2;
        private readonly Vector3 _normal;
        private readonly Color32 _color;

        // CONSTRUCTORS

        //create a triangle from the three given points and a color
        public Triangle(Vector3 vertex0, Vector3 vertex1, Vector3 vertex2, Color32 color) {
            _vertex0 = vertex0;
            _vertex1 = vertex1;
            _vertex2 = vertex2;
            _normal = Geometry.TriangleNormal(_vertex0, _vertex1, _vertex2);
            _color = color;
        }

        // PROPERTIES

        public Vector3 Vertex0 {
            get { return _vertex0; }
        }

        public Vector3 Vertex1 {
            get { return _vertex1; }
        }

        public Vector3 Vertex2 {
            get { return _vertex2; }
        }

        public Vector3 Normal {
            get { return _normal; }
        }

        public Color32 Color {
            get { return _color; }
        }

        //the area of the triangle 
        public float Area {
            get { return Geometry.TriangleArea(_vertex0, _vertex1, _vertex2); }
        }


        // METHODS

        public bool Contains(Vector3 point) {
            return Geometry.TriangleContains(_vertex0, _vertex1, _vertex2, point, _normal);
        }
    }
}