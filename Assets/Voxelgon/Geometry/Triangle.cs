using System.Collections.Generic;
using UnityEngine;


namespace Voxelgon.Geometry {

    public class Triangle : Polygon {

        // CONSTRUCTORS

        //create a triangle from the three given points and a color
        public Triangle(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3, Color32? color = null) :
            base(new [] {vertex1, vertex2, vertex3}, color: color) {
        }

        // PROPERTIES

        //is the polygon convex?
        public override bool IsConvex { 
            //triangles are always convex
            get { return true; } 
        }

        //is the polygon valid?
        // must have >= 3 vertices
        public override bool IsValid { 
            get {
                bool valid = true;
                valid &= (!_vertices[0].Equals(_vertices[1]));
                valid &= (!_vertices[1].Equals(_vertices[2]));
                valid &= (!_vertices[2].Equals(_vertices[0]));
                return valid;
            }
        }


        // METHODS

        //returns and array of triangles that make up the polygon
        public override List<Triangle> ToTriangles() {
            return new List<Triangle> {this};
        }
    }
}