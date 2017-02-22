using UnityEngine;
using System;
using System.Collections.Generic;


namespace Voxelgon.Geometry {

    public class Polygon {

        // FIELDS

        protected readonly Vector3[] _vertices;
        protected readonly Vector3 _normal;
        protected readonly Vector3 _center;
        protected readonly Color32 _color;

        // CONSTRUCTORS

        public Polygon(Vector3[] vertices, Color32 color) {
            _vertices = (Vector3[])vertices.Clone();
            _color = color;

            if (_vertices.Length < 3) {
                throw new InvalidPolygonException("Less than 3 vertices");
            }

            //find center
            _center = Geometry.VectorAvg(_vertices);

            //find temporary normal
            var tmpNormal = Geometry.PointsNormal(_vertices).normalized;
            if (tmpNormal.sqrMagnitude < 0.01f) {
                throw new InvalidPolygonException("Points are Colinear");
            }

            //check if coplanar 
            Plane p = new Plane(tmpNormal, _vertices[0]);
            for (var i = 3; i < _vertices.Length; i++) {
                if (p.GetDistanceToPoint(_vertices[i]) > 0.01f) {
                    throw new InvalidPolygonException("Not Planar");
                }
            }
            //check for real normal
            var shoelace = Geometry.Shoelace(Geometry.FlattenPoints(_vertices, tmpNormal));
            if (shoelace > 0.001f) {
                _normal = -tmpNormal;
            } else if (shoelace < -0.001f) {
                _normal = tmpNormal;
            } else {
                throw new InvalidPolygonException("Points are Colinear");
            }
        }

        public Polygon(Vector3 center, float radius, int sideCount, Vector3 normal, Color32 color, Vector3 tangent = default(Vector3)) {
            _vertices = new Vector3[sideCount];
            _color = color;
            _center = center;
            _normal = normal;

            if (normal.Equals(Vector3.zero)) {
                normal = Vector3.forward;
            }

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

        protected Polygon(Vector3[] vertices, Vector3 normal, Vector3 center, Color32 color) {
            _vertices = vertices;
            _normal = normal;
            _center = center;
            _color = color;
        }


        // PROPERTIES


        //is the polygon convex?
        public virtual bool IsConvex {
            get {
                for (int i = 0; i < VertexCount; i++) {
                    int j = (i + 1) % VertexCount;
                    int k = (i + 2) % VertexCount;

                    if (Geometry.TriangleWindingOrder(
                            _vertices[i],
                            _vertices[j],
                            _vertices[k],
                            _normal) != 1) {
                        return false;
                    }
                }

                return true;
            }
        }

        //the area of the polygon
        public virtual float Area {
            get { return Mathf.Abs(Geometry.Shoelace(FlatVertices()) / 2); }
        }

        //the number of vertices in the polygon
        public virtual int VertexCount {
            get { return _vertices.Length; }
        }

        //the polygon's vertices as an array
        public virtual Vector3[] Vertices {
            get { return (Vector3[])_vertices.Clone(); }
        }

        //the polygon's normal vector
        public virtual Vector3 Normal {
            get { return _normal; }
        }

        //the polygon's colors as a new list
        public virtual Color32 Color {
            get { return _color; }
        }

        //the polygon's vertex normals as a new list
        public virtual Vector3[] VertexNormals {
            get {
                var vertexNormals = new Vector3[VertexCount];
                for (var i = 0; i < VertexCount; i++) {
                    vertexNormals[i] = GetVertexNormal(i);
                }

                return vertexNormals;
            }
        }

        //the polygon's geometric center
        public virtual Vector3 Center {
            get { return _center; }
        }

        public virtual int[] TriangleIndices {
            get {
                var indices = new List<int>();
                var vertices2D = Geometry.FlattenPoints(_center, _vertices, _normal);
                Geometry.PolygonSegment(vertices2D, indices, 0, 1);

                return indices.ToArray();
            }
        }

        // METHODS

        //returns the winding order relative to the normal
        // 1 = clockwise
        //-1 = counter-clockwise
        // 0 = all points are colinear, or polygon is invalid
        public virtual int WindingOrder(Vector3 normal) {
            var dot = Vector3.Dot(_normal, normal);

            if (dot > 0.0001f) return 1;
            if (dot < -0.0001f) return -1;
            return 0;
        }

        //if the polygon is counter-clockwise, reverse it so it is clockwise
        public virtual Polygon EnsureClockwise(Vector3 normal) {
            if (WindingOrder(normal) != 1) return Reverse();
            return Clone();
        }

        //returns whether or not `point` is on or inside the polygon
        public virtual bool Contains(Vector3 point) {
            foreach (Triangle t in ToTriangles()) {
                if (t.Contains(point))
                    return true;
            }

            return false;
        }

        //returns an array of triangles that make up the polygon
        public virtual Triangle[] ToTriangles() {
            var triangleIndices = TriangleIndices;
            if (triangleIndices.Length % 3 != 0) {
                throw new InvalidPolygonException("Unknown Error");
            }
            var triCount = triangleIndices.Length / 3;
            var triangles = new Triangle[triCount];

            var j = 0;
            for (int i = 0; i < triCount; i++) {
                triangles[i] = new Triangle(
                            _vertices[j],
                            _vertices[j + 1],
                            _vertices[j + 2],
                            _color);
                j += 3;
            }

            return triangles;
        }

        //returns a polygon truncated starting at Vector3 `point` by vector3 `offset`
        public virtual Polygon Truncate(Vector3 point, Vector3 offset) {
            var plane = new Plane(offset.normalized, offset + point);
            var verts = new List<Vector3>();
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

            var newVertices = verts.ToArray();

            return new Polygon(newVertices, _normal, Geometry.VectorAvg(newVertices), _color);
        }

        //returns the vertex at index `index`
        public Vector3 GetVertex(int index) {
            return _vertices[index];
        }

        //returns the vector pointing "out" of the vertex at index `index`
        //normalized average of two adjacent edge normals
        public Vector3 GetVertexNormal(int index) {
            /*Vector3 edge1 = GetEdgeNormal(index);
            Vector3 edge2 = GetEdgeNormal((index - 1 + VertexCount) % VertexCount);

            return (edge1 + edge2) / 2;
            */
            Vector3 vertex1 = _vertices[(index - 1 + VertexCount) % VertexCount];
            Vector3 vertex2 = _vertices[index];
            Vector3 vertex3 = _vertices[(index + 1) % VertexCount];

            Vector3 edgeNormal1 = Vector3.Cross(vertex2 - vertex1, _normal);
            Vector3 edgeNormal2 = Vector3.Cross(vertex3 - vertex2, _normal);
            edgeNormal2 *= Mathf.Sqrt(edgeNormal1.sqrMagnitude / edgeNormal2.sqrMagnitude);

            return (edgeNormal1 + edgeNormal2).normalized;
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
            return Vector3.Cross(GetEdge(index), _normal).normalized;
        }

        //returns a clone of this Polygon
        public Polygon Clone() {
            return new Polygon(_vertices, _normal, _center, _color);
        }

        //reverses the polygon's winding order
        public virtual Polygon Reverse() {
            var newVertices = (Vector3[])_vertices.Clone();
            Array.Reverse(newVertices);

            return new Polygon(newVertices, -1 * _normal, _center, _color);
        }


        public Vector2[] FlatVertices(Vector3 normal) {
            return Geometry.FlattenPoints(_center, _vertices, normal);
        }

        public Vector2[] FlatVertices() {
            return FlatVertices(_normal);
        }


        //returns a clone of this Polygon scales around its center
        public Polygon Scale(float scaleFactor, Vector3 center) {
            var newVertices = new Vector3[_vertices.Length];

            for (int i = 0; i < newVertices.Length; i++) {
                newVertices[i] = (_vertices[i] - center) * scaleFactor;
            }

            return new Polygon(newVertices, _normal, _center, _color);
        }

        //returns a clone of this Polygon scales around its center
        public Polygon Scale(float scaleFactor) {
            return Scale(scaleFactor, _center);
        }

        // returns a clone of this Polygon offset by a given vector
        public Polygon Translate(Vector3 translationVector) {
            var newVertices = new Vector3[_vertices.Length];
            var newCenter = _center + translationVector;

            for (int i = 0; i < newVertices.Length; i++) {
                newVertices[i] = _vertices[i] + translationVector;
            }

            return new Polygon(newVertices, _normal, newCenter, _color);
        }

        // returns a clone of this Polygon with each vertex operated on by the 4x4 matrix
        public Polygon Transform(Matrix4x4 matrix) {
            var newVertices = new Vector3[VertexCount];
            var newNormal = matrix.MultiplyVector(_normal);
            var newCenter = matrix.MultiplyPoint3x4(_center);

            for (var i = 0; i < VertexCount; i++) {
                newVertices[i] = matrix.MultiplyPoint3x4(_vertices[i]);
            }

            return new Polygon(newVertices, newNormal, newCenter, _color);
        }


        //draw the polygon in the world for 1 frame
        public void Draw() {
            for (int i = 0; i < VertexCount; i++) {
                Debug.DrawLine(_vertices[i], _vertices[(i + 1) % VertexCount]);
            }
        }

        //are the polygons equal?
        public bool Equals(Polygon p) {
            if (VertexCount != p.VertexCount
            || Normal != p.Normal) { return false; }

            for (int i = 0; i < VertexCount; i++) {
                if (!GetVertex(i).Equals(p.GetVertex(i))) {
                    return false;
                }
            }

            return true;
        }
    }
}
