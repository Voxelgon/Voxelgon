using System.Collections.Generic;
using UnityEngine;


namespace Voxelgon.Geometry {

    public class Triangle : Polygon {

        // CONSTRUCTORS

        //create a triangle from the three given points and a color
        public Triangle(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3, Color32 color) :
            this(new[] { vertex1, vertex2, vertex3 }, color) { }

        public Triangle(Vector3[] vertices, Color32 color) :
            base(
                vertices,
                Geometry.TriangleNormal(vertices[0], vertices[1], vertices[2]),
                Geometry.VectorAvg(vertices),
                color) { }

        // PROPERTIES

        //is the triangle convex?
        public override bool IsConvex {
            //triangles are always convex
            get { return true; }
        }

        //the area of the triangle 
        public override float Area {
            get { return Vector3.Cross(_vertices[1] - _vertices[0], _vertices[2] - _vertices[0]).magnitude / 2; }
        }

        //the number of vertices in the polygon
        public override int VertexCount {
            get { return 3; }
        }

        public override int[] TriangleIndices {
            get { return new int[] { 0, 1, 2 }; }
        }



        // METHODS

        //returns and array of triangles that make up the polygon
        public override Triangle[] ToTriangles() {
            return new Triangle[] { new Triangle(_vertices, _color) };
        }
    }
}