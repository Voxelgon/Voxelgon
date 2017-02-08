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

        public Polygon(Vector3[] vertices, Vector3[] normals = null, Color32[] colors = null, Color32? color = default(Color32?)) {
            _vertices = (Vector3[])vertices.Clone();
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


        // PROPERTIES

        //the normal of the clockwise polygon
        //if the polygon is invalid, return Vector3.zero
        public virtual Vector3 SurfaceNormal {
            get {
                if (!IsValid)
                    throw new InvalidPolygonException();

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
        public virtual bool IsConvex {
            get {
                if (!IsValid)
                    throw new InvalidPolygonException();

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
        public virtual bool IsValid {
            get {
                bool valid = true;
                valid &= (VertexCount >= 3);
                return valid;
            }
        }

        //the area of the polygon
        public virtual float Area {
            get {
                if (!IsValid)
                    throw new InvalidPolygonException();

                float area = 0;
                foreach (Triangle t in ToTriangles()) {
                    area += t.Area;
                }

                return area;
            }
        }

        //the number of vertices in the polygon
        public virtual int VertexCount {
            get { return _vertices.Length; }
        }

        //the polygon's vertices as a new list
        public virtual List<Vector3> Vertices {
            get { return new List<Vector3>(_vertices); }
        }

        //the polygon's normals as a new list
        public virtual List<Vector3> Normals {
            get { return new List<Vector3>(_normals); }
        }

        //the polygon's colors as a new list
        public virtual List<Color32> Colors {
            get { return new List<Color32>(_colors); }
        }

        //the polygon's geometric center
        public virtual Vector3 Center {
            get {
                var sum = new Vector3();
                foreach (Vector3 v in _vertices) {
                    sum += v;
                }

                return sum / _vertices.Length;
            }
        }

        // METHODS

        //returns the winding order relative to the normal
        // 1 = clockwise
        //-1 = counter-clockwise
        // 0 = all points are colinear, or polygon is invalid
        public virtual int WindingOrder(Vector3 normal) {
            if (!IsValid)
                throw new InvalidPolygonException();

            return (Vector3.Dot(normal, SurfaceNormal) >= 0) ? 1 : -1;
        }

        //returns whether or not `point` is on or inside the polygon
        public virtual bool Contains(Vector3 point) {
            if (!IsValid)
                throw new InvalidPolygonException();

            foreach (Triangle t in ToTriangles()) {
                if (t.Contains(point))
                    return true;
            }
            return false;
        }

        //reverses the polygon's winding order
        public virtual Polygon Reverse() {
            var vertices = (Vector3[])_vertices.Clone();
            var normals = (Vector3[])_normals.Clone();
            var colors = (Color32[])_colors.Clone();
            Array.Reverse(vertices);
            Array.Reverse(normals);
            Array.Reverse(colors);
            return new Polygon(vertices, normals, colors);
        }

        //if the polygon is counter-clockwise, reverse it so it is clockwise
        public virtual Polygon EnsureClockwise(Vector3 normal) {
            if (WindingOrder(normal) == -1) {
                return Reverse();
            }
            return Clone();
        }

        public virtual List<int> ToTriangleIndices() {
            var indices = new List<int>();

            if (IsValid) {
                PolygonSegment(indices, 0, 1, SurfaceNormal);
            }

            return indices;
        }

        //returns an array of triangles that make up the polygon
        public virtual List<Triangle> ToTriangles() {
            var triangles = new List<Triangle>();
            var indices = ToTriangleIndices();

            if (indices.Count % 3 == 0 && IsValid) {
                for (int i = 0; i < indices.Count; i += 3) {
                    triangles.Add(new Triangle(
                            _vertices[i],
                            _vertices[i + 1],
                            _vertices[i + 2],
                            _colors[i],
                            _colors[i + 1],
                            _colors[i + 2]));
                }
            } 

            return triangles;
        }

        //returns a polygon truncated starting at Vector3 `point` by vector3 `offset`
        public virtual Polygon Truncate(Vector3 point, Vector3 offset) {
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

                if (trim[lastTrim] != lastVert || trim.Count == 1) {
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
            return Vector3.Cross(GetEdge(index), SurfaceNormal);
        }

        //returns the color at index `index`
        public Color32 GetColor(int index) {
            return _colors[index];
        }

        //returns a clone of this Polygon
        public Polygon Clone() {
            return new Polygon(_vertices, _normals, _colors);
        }

        //returns a clone of this Polygon scales around its center
        public Polygon Scale(float scaleFactor, Vector3 center) {
            var newVertices = new Vector3[_vertices.Length];
            for (int i = 0; i < newVertices.Length; i++) {
                newVertices[i] = (_vertices[i] - center) * scaleFactor;
            }

            return new Polygon(newVertices, _normals, _colors);
        }

        //returns a clone of this Polygon scales around its center
        public Polygon Scale(float scaleFactor) {
            return Scale(scaleFactor, Center);
        }

        // returns a clone of this Polygon offset by a given vector
        public Polygon Translate(Vector3 translationVector) {
            var newVertices = new Vector3[_vertices.Length];
            for (int i = 0; i < newVertices.Length; i++) {
                newVertices[i] = _vertices[i] + translationVector;
            }

            return new Polygon(newVertices, _normals, _colors);
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

        //adds indices to List `indices`, calls itself recursively to handle concave polygons
        private int PolygonSegment(List<int> indices, int index1, int index2, Vector3 normal) {
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
                    indices.Add(index1);
                    indices.Add(index2);
                    indices.Add(index3);

                    if (index1 != 0 && Geometry.TriangleWindingOrder(_vertices[0], _vertices[index1], _vertices[index3], normal) == 1) {
                        return index3;
                    }

                    index2 = index3;
                    index3++;
                }
                else {
                    index3 = PolygonSegment(indices, index2, index3, normal);
                }
            }
            return -1;
        }
    }
}
