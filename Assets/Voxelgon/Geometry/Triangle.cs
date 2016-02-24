using System;
using System.Collections.Generic;
using UnityEngine;


namespace Voxelgon.Geometry {

    public class Triangle : IPolygon {

        // FIELDS

        private readonly Vector3[] _vertices = new Vector3[3];
        private readonly Vector3[] _normals = new Vector3[3];
        private readonly Color32[] _colors = new Color32[3];

        // CONSTRUCTORS

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

        // PROPERTIES

        //IPolygon
        //the normal of the clockwise polygon
        //if the polygon is invalid, return Vector3.zero
        public Vector3 SurfaceNormal {
            get {
                if (!IsValid) {
                    return Vector3.zero;
                }
                
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

        // OPERATORS

        //Convert to a 3-sided polygon
        public static explicit operator Polygon(Triangle t) {
            return new Polygon(new List<Vector3>(t._vertices));
        }

        // METHODS

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

            return (triangle1.WindingOrder(SurfaceNormal) != -1
            && triangle2.WindingOrder(SurfaceNormal) != -1
            && triangle3.WindingOrder(SurfaceNormal) != -1);
        }

        //IPolygon
        //reverses the polygon's winding order
        public Polygon Reverse() {
            var rev = (Vector3[]) _vertices.Clone();
            Array.Reverse(rev);
            return new Polygon(rev);
        }

        //IPolygon
        //if the polygon is counter-clockwise, reverse it so it is clockwise
        public Polygon EnsureClockwise(Vector3 normal) {
            if (WindingOrder(normal) == -1) {
                return Reverse();
            }
            return Clone();
        }

        //IPolygon
        //returns and array of triangles that make up the polygon
        public List<Triangle> ToTriangles() {
            return new List<Triangle> {this};
        }

        //IPolygon
        //returns a polygon truncated starting at Vector3 `point` by vector3 `offset`
        public Polygon Truncate(Vector3 point, Vector3 offset) {
            var poly = (Polygon) this;
            return poly.Truncate(point, offset);
        }

        //IPolygon
        //returns the vertex at index `index`
        public Vector3 GetVertex(int index) {
            return _vertices[index];
        }

        //IPolygon
        //returns the vertex normal at index `index`
        //same as mesh normal, usually close to parallel with plane normal
        public Vector3 GetNormal(int index) {
            return _normals[index];
        }

        //IPolygon
        //returns the vector pointing "out" of the vertex at index `index`
        //normalized average of two adjacent edge normals
        public Vector3 GetVertexNormal(int index) {
            Vector3 edge1 = GetEdgeNormal(index);
            Vector3 edge2 = GetEdgeNormal((index + 2) % 3);

            return (edge1 + edge2) / 2;
        }

        //IPolygon
        //returns the edge vector at index `index`
        //normalized vector from a vertex to the following vertex
        public Vector3 GetEdge(int index) {
            Vector3 vertex1 = _vertices[index];
            Vector3 vertex2 = _vertices[(index + 1) % 3];

            return (vertex2 - vertex1).normalized;
        }

        //IPolygon
        //returns the edge normal at index `index`
        //cross product of plane normal and edge
        public Vector3 GetEdgeNormal(int index) {
            return Vector3.Cross(SurfaceNormal, GetEdge(index));
        }

        //returns the color at index `index`
        public Color32 GetColor(int index) {
            return _colors[index];
        }

        //IPolygon
        //returns a clone of this IPolygon
        public Polygon Clone() {
            return new Polygon(_vertices);
        }

        //IPolygon
        //are the polygons equal?
        public bool Equals(IPolygon p) {
            if (p.VertexCount != 3) { return false; }
            for (int i = 0; i < 3; i++) {
                if (GetVertex(i) != p.GetVertex(i) || GetNormal(i) != p.GetNormal(i)) { return false; }
            }

            return true;
        }
    }
}