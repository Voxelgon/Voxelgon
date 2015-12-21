using UnityEngine;
using System.Collections.Generic;


namespace Voxelgon.Geometry {

    public class Polygon : IPolygon {

        //FIELDS

        private readonly List<Vector3> _vertices = new List<Vector3>();

        //PROPERTIES

        //IPolygon
        //access each vertex individually by its index
        public Vector3 this[int index] { 
            get { return _vertices[index]; }
            set { _vertices[index] = value; }
        }

        //IPolygon
        //the normal of the clockwise polygon
        // if the polygon is invalid, return Vector3.zero
        public Vector3 Normal { 
            get {
                if (!IsValid) {
                    return Vector3.zero;
                }

                Vector3 baseNormal = Geometry.TriangleNormal(
                                         _vertices[0],
                                         _vertices[1],
                                         _vertices[2]);

                float angleSum = 0;

                for (int i = 0; i < VertexCount; i++) {
                    int j = (i + 1) % VertexCount;
                    int k = (i + 2) % VertexCount;

                    angleSum += Geometry.TriangleAngle(
                        _vertices[i],
                        _vertices[j],
                        _vertices[k],
                        baseNormal);
                }

                return baseNormal * ((angleSum >= 0) ? 1 : -1);
            }
        }

        //IPolygon
        //is the polygon convex?
        public bool IsConvex { 
            get {
                if (!IsValid)
                    return false;

                for (int i = 0; i < VertexCount; i++) {
                    int j = (i + 1) % VertexCount;
                    int k = (i + 2) % VertexCount;

                    if (Geometry.TriangleWindingOrder(
                            _vertices[i],
                            _vertices[j],
                            _vertices[k],
                            Normal) != 1) {
                        return false;
                    }
                }
                return true;
            }
        }

        //IPolygon
        //is the polygon valid?
        // must have >= 3 vertices
        public bool IsValid { 
            get {
                bool valid = true;
                valid &= (VertexCount >= 3);
                return valid;
            }
        }

        //IPolygon
        //the area of the polygon
        public float Area { 
            get {
                if (!IsValid) {
                    return 0;
                }

                float area = 0;
                foreach (Triangle t in ToTriangles()) {
                    area += t.Area;
                }

                return area;
            }
        }

        //IPolygon
        //the number of vertices in the polygon
        public int VertexCount {
            get { return _vertices.Count; }
        }

        //METHODS

        //IPolygon
        //returns the winding order relative to the normal
        // 1 = clockwise
        //-1 = counter-clockwise
        // 0 = all points are colinear, or polygon is invalid
        public int WindingOrder(Vector3 normal) {
            if (!IsValid) {
                return 0;
            }

            return (Vector3.Dot(normal, Normal) >= 0) ? 1 : -1;
        }

        //IPolygon
        //returns whether or not `point` is on or inside the polygon
        public bool Contains(Vector3 point) {
            if (!IsValid) {
                return false;
            }

            bool contains = false;
            foreach (Triangle t in ToTriangles()) {
                contains |= t.Contains(point);
            }
            return contains;
        }

        //IPolygon
        //reverses the polygon's winding order
        public void Reverse() {
            _vertices.Reverse();
        }

        //IPolygon
        //if the polygon is counter-clockwise, reverse it so it is clockwise
        public void EnsureClockwise(Vector3 normal) {
            if (WindingOrder(normal) == -1) {
                Reverse();
            }
        }

        //IPolygon
        //returns an array of triangles that make up the polygon
        public List<Triangle> ToTriangles() {
            if (!IsValid) {
                throw new System.InvalidOperationException("Polygon must have at least 3 vertices");
            }

            var triangles = new List<Triangle>();
            PolygonSegment(triangles, 0, 1, Normal);

            return triangles;
        }

        //PRIVATE METHODS

        //adds triangles to List `triangles`, calls itself recursively to handle concave polygonsa
        private int PolygonSegment(List<Triangle> triangles, int index1, int index2, Vector3 normal) {
            int index3 = index2 + 1;

            while (index3 < _vertices.Count && index3 >= 0) {
                bool validTri = true;

                if (Geometry.TriangleWindingOrder(_vertices[index1], _vertices[index2], _vertices[index3], normal) == 1) {

                    for (int i = index3 + 1; i < _vertices.Count; i++) {
                        validTri &= !Geometry.TriangleContains(_vertices[index1], _vertices[index2], _vertices[index3], _vertices[i], normal);
                    }
                }
                else {
                    validTri = false;
                }

                if (validTri) {
                    triangles.Add(new Triangle(_vertices[index1], _vertices[index2], _vertices[index3]));

                    if (index1 != 0 && Geometry.TriangleWindingOrder(_vertices[0], _vertices[index1], _vertices[index3], normal) == 1) {
                        return index3;
                    }

                    index2 = index3;
                    index3++;
                }
                else {
                    index3 = PolygonSegment(triangles, index2, index3, normal);
                }
            }
            return -1;
        }
    }
}