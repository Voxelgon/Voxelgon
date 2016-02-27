using UnityEngine;
using System;
using System.Collections.Generic;


namespace Voxelgon.Geometry {

    public class Polygon {

        // FIELDS

        internal readonly Vector3[] _vertices;
        internal readonly Vector3[] _normals;
        internal readonly Color32[] _colors;

        // CONSTRUCTORS

        public Polygon(Vector3[] vertices, Vector3[] normals = null, Color32[] colors = null, Color32? color = null) {
            _vertices = (Vector3[]) vertices.Clone();
            _normals = new Vector3[_vertices.Length];
            _colors = new Color32[_vertices.Length];

            //fill in unassigned normals with the SurfaceNormal
            Vector3 normal = SurfaceNormal;
            int normalCount = 0;
            if (normals != null) {
                normalCount = normals.Length;
                normals.CopyTo(_normals, 0);
            }

            for (int i = normalCount; i < _vertices.Length; i++) {
                _normals[i] = normal;
            }

            //fill in unassigned vertex colors with `color`
            int colorCount = 0;
            if (colors != null) { 
                colorCount = colors.Length;
                colors.CopyTo(_colors, 0);
            }

            for (int i = colorCount; i < _vertices.Length; i++) {
                _colors[i] = color ?? Color.gray;
            }
        }

        public Polygon(Vector3 center, float radius, int sideCount, Vector3 normal, Vector3 tangent = default(Vector3)) {
            if (normal.Equals(Vector3.zero)) {
                normal = Vector3.forward;
            }

            _vertices = new Vector3[sideCount];
            var rotation = Quaternion.AngleAxis(360.0f / sideCount, normal);

            if (tangent.Equals(Vector3.zero)) {
                tangent = Vector3.Cross(Vector3.up, normal);
            }

            if (tangent.Equals(Vector3.zero)) {
                tangent = Vector3.Cross(Vector3.forward, normal);
            }

            for (int i = 0; i < sideCount; i++) {
                _vertices[i] = center + (tangent * radius);
                tangent = rotation * tangent;
            }
        }

        // PROPERTIES

        //the normal of the clockwise polygon
        //if the polygon is invalid, return Vector3.zero
        public Vector3 SurfaceNormal {
            get {
                if (!IsValid) {
                    return Vector3.zero;
                }

                Vector3 baseNormal = Geometry.TriangleNormal(
                                         _vertices[0],
                                         _vertices[1],
                                         _vertices[2]);
                if (VertexCount == 3) {
                    return baseNormal;
                }

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
                            SurfaceNormal) != 1) {
                        return false;
                    }
                }
                return true;
            }
        }

        //is the polygon valid?
        // must have >= 3 vertices
        public bool IsValid {
            get {
                bool valid = true;
                valid &= (VertexCount >= 3);
                return valid;
            }
        }

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

        //the number of vertices in the polygon
        public int VertexCount {
            get { return _vertices.Length; }
        }


        // METHODS

        //returns the winding order relative to the normal
        // 1 = clockwise
        //-1 = counter-clockwise
        // 0 = all points are colinear, or polygon is invalid
        public int WindingOrder(Vector3 normal) {
            if (!IsValid) {
                return 0;
            }

            return (Vector3.Dot(normal, SurfaceNormal) >= 0) ? 1 : -1;
        }

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

        //reverses the polygon's winding order
        public Polygon Reverse() {
            var vertices = (Vector3[]) _vertices.Clone();
            var normals  = (Vector3[]) _normals.Clone();
            var colors   = (Color32[]) _colors.Clone();
            Array.Reverse(vertices);
            Array.Reverse(normals);
            Array.Reverse(colors);
            return new Polygon(vertices, normals, colors);
        }

        //if the polygon is counter-clockwise, reverse it so it is clockwise
        public Polygon EnsureClockwise(Vector3 normal) {
            if (WindingOrder(normal) == -1) {
                return Reverse();
            }
            return Clone();
        }

        //returns an array of triangles that make up the polygon
        public List<Triangle> ToTriangles() {
            var triangles = new List<Triangle>();

            if (IsValid) {
                PolygonSegment(triangles, 0, 1, SurfaceNormal);
            }

            return triangles;
        }

        //returns a polygon truncated starting at Vector3 `point` by vector3 `offset`
        public Polygon Truncate(Vector3 point, Vector3 offset) {
            var plane = new Plane(offset.normalized, offset + point);
            var verts = new List<Vector3>();
            var norms = new List<Vector3>();
            var trim = new List<int>();
            int start = 0;


            for (int i = 0; i < VertexCount; i++) {
                if ((!plane.GetSide(_vertices[i]))) {
                    trim.Add(i);
                }
            }

            for (int i = 0; i < trim.Count; i++) {
                int lastTrim = (i - 1 + trim.Count) % trim.Count;
                int nextTrim = (i + 1) % trim.Count;
                int lastVert = (trim[i] - 1 + VertexCount) % VertexCount;
                int nextVert = (trim[i] + 1) % VertexCount;

                if (trim[lastTrim] != lastVert|| trim.Count == 1) {
                    for (int j = start; j < trim[i]; j++) {
                        verts.Add(_vertices[j]);
                        norms.Add(_normals[j]);
                    }

                    var normal = (_vertices[lastVert] - _vertices[trim[i]]).normalized;
                    var ray = new Ray(_vertices[trim[i]], normal);
                    float length;
                    plane.Raycast(ray, out length);
                    verts.Add(_vertices[trim[i]] + normal * length);
                }
                if (trim[nextTrim] != nextVert || trim.Count == 1) {
                    start = trim[i] + 1;

                    var normal = (_vertices[nextVert] - _vertices[trim[i]]).normalized;
                    var ray = new Ray(_vertices[trim[i]], normal);
                    float length;
                    plane.Raycast(ray, out length);
                    verts.Add(_vertices[trim[i]] + normal * length);
                }
            }

            if (trim.Count == 0) {
                verts = new List<Vector3>(_vertices);
            }
            return new Polygon(verts.ToArray());
        }

        //returns the vertex at index `index`
        public Vector3 GetVertex(int index) {
            return _vertices[index];
        }

        //returns the vertex normal at index `index`
        //same as mesh normal, usually close to parallel with plane normal
        public Vector3 GetNormal(int index) {
            return _normals[index];
        }

        //returns the vector pointing "out" of the vertex at index `index`
        //normalized average of two adjacent edge normals
        public Vector3 GetVertexNormal(int index) {
            Vector3 edge1 = GetEdgeNormal(index);
            Vector3 edge2 = GetEdgeNormal((index - 1 + VertexCount) % VertexCount);

            return (edge1 + edge2) / 2;
        }

        //returns the edge vector at index `index`
        //normalized vector from a vertex to the following vertex
        public Vector3 GetEdge(int index) {
            Vector3 vertex1 = _vertices[index];
            Vector3 vertex2 = _vertices[(index + 1) % VertexCount];

            return (vertex2 - vertex1).normalized;
        }

        //returns the edge normal at index `index`
        //cross product of plane normal and edge
        public Vector3 GetEdgeNormal(int index) {
            return Vector3.Cross(SurfaceNormal, GetEdge(index));
        }

        //returns the color at index `index`
        public Color32 GetColor(int index) {
            return _colors[index];
        }

        //returns a clone of this IPolygon
        public Polygon Clone() {
            return new Polygon(_vertices, _normals);
        }

        //draw the polygon in the world for 1 frame
        public void Draw() {
            for (int i = 0; i < VertexCount; i++) {
                int next = (i + 1) % VertexCount;
                Debug.DrawLine(_vertices[i], _vertices[next]);
            }
        }

        //are the polygons equal?
        public bool Equals(Polygon p) {
            if (VertexCount != p.VertexCount) { return false; }
            for (int i = 0; i < VertexCount; i++) {
                if (!GetVertex(i).Equals(p.GetVertex(i))
                 || !GetNormal(i).Equals(p.GetNormal(i)) 
                 || !GetColor(i).Equals(p.GetColor(i))) { 
                    return false; 
                }
            }

            return true;
        }


        // PRIVATE METHODS

        //adds triangles to List `triangles`, calls itself recursively to handle concave polygons
        private int PolygonSegment(List<Triangle> triangles, int index1, int index2, Vector3 normal) {
            int index3 = index2 + 1;

            while (index3 < VertexCount && index3 >= 0) {
                bool validTri = true;

                if (Geometry.TriangleWindingOrder(_vertices[index1], _vertices[index2], _vertices[index3], normal) == 1) {

                    for (int i = index3 + 1; i < VertexCount; i++) {
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
