using UnityEngine;
using System.Collections.Generic;
using Voxelgon.Util.Geometry;


namespace Voxelgon.Util.Geometry {

    public class Polygon {

        // FIELDS

        private readonly Vector3[] _vertices;
        private readonly Vector3 _normal;
        private readonly Vector3 _center;

        // CONSTRUCTORS

        public Polygon(Vector3[] vertices) {
            _vertices = (Vector3[])vertices.Clone();

            if (VertexCount < 3) {
                throw new InvalidPolygonException("Less than 3 vertices");
            }

            //find center
            _center = GeometryVG.VectorAvg(_vertices);

            //find normal
            _normal = CalcNormal(_vertices);
        }

        public Polygon(ICollection<Vector3> vertices) {
            _vertices = new Vector3[vertices.Count];
            vertices.CopyTo(_vertices, 0);

            if (VertexCount < 3) {
                throw new InvalidPolygonException("Less than 3 vertices");
            }

            //find center
            _center = GeometryVG.VectorAvg(_vertices);

            //find normal
            _normal = CalcNormal(_vertices);
        }

        public Polygon(Vector3 center, float radius, int sideCount, Vector3 normal, Vector3 tangent = default(Vector3)) {
            _vertices = new Vector3[sideCount];
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

        private Polygon(Vector3[] vertices, Vector3 normal, Vector3 center) {
            _vertices = vertices;
            _normal = normal;
            _center = center;
        }


        // PROPERTIES


        //is the polygon convex?
        public bool IsConvex {
            get {
                for (int i = 0; i < VertexCount; i++) {
                    int j = (i + 1) % VertexCount;
                    int k = (i + 2) % VertexCount;

                    if (GeometryVG.TriangleWindingOrder(
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
        public float Area {
            get { return Mathf.Abs(GeometryVG.Shoelace(FlatVertices()) / 2); }
        }

        //the number of vertices in the polygon
        public int VertexCount {
            get { return _vertices.Length; }
        }

        //the polygon's vertices as an array
        public Vector3[] Vertices {
            get { return (Vector3[])_vertices.Clone(); }
        }

        //the polygon's normal vector
        public Vector3 Normal {
            get { return _normal; }
        }

        //the polygon's vertex normals as a new list
        public Vector3[] VertexNormals {
            get {
                var vertexNormals = new Vector3[VertexCount];
                for (var i = 0; i < VertexCount; i++) {
                    vertexNormals[i] = GetVertexNormal(i);
                }

                return vertexNormals;
            }
        }

        //the polygon's geometric center
        public Vector3 Center {
            get { return _center; }
        }

        public int[] TriangleIndices {
            get {
                var indices = new List<int>(VertexCount * 3);
                var vertices2D = GeometryVG.FlattenPoints(_center, _vertices, _normal);
                GeometryVG.TriangulateSegment(vertices2D, indices, 0, 1);

                return indices.ToArray();
            }
        }

        // METHODS

        //returns the winding order relative to the normal
        // 1 = clockwise
        //-1 = counter-clockwise
        // 0 = all points are colinear, or polygon is invalid
        public int WindingOrder(Vector3 normal) {
            var dot = Vector3.Dot(_normal, normal);

            if (dot > 0.0001f) return 1;
            if (dot < -0.0001f) return -1;
            return 0;
        }

        //if the polygon is counter-clockwise, reverse it so it is clockwise
        public Polygon EnsureClockwise(Vector3 normal) {
            if (WindingOrder(normal) != 1) return Reverse();
            return this;
        }

        //returns whether or not `point` is on or inside the polygon
        public bool Contains(Vector3 point) {
            var indices = TriangleIndices;
            for (var i = 0; i < indices.Length; i += 3) {
                if (GeometryVG.TriangleContains(
                        _vertices[indices[i]],
                        _vertices[indices[i + 1]],
                        _vertices[indices[i + 2]],
                        point,
                        _normal)) {
                    return true;
                }
            }

            return false;
        }

        //returns a polygon truncated starting at Vector3 `point` by vector3 `offset`
        public Polygon Truncate(Vector3 point, Vector3 offset) {
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

            return new Polygon(newVertices, _normal, GeometryVG.VectorAvg(newVertices));
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
        //vector from a vertex to the following vertex
        public Vector3 GetEdge(int index) {
            Vector3 vertex1 = _vertices[index];
            Vector3 vertex2 = _vertices[(index + 1) % VertexCount];

            return (vertex2 - vertex1);
        }

        //returns the edge normal at index `index`
        //cross product of plane normal and edge
        public Vector3 GetEdgeNormal(int index) {
            return Vector3.Cross(GetEdge(index), _normal).normalized;
        }

        //reverses the polygon's winding order
        public Polygon Reverse() {
            var size = VertexCount;
            var newVertices = new Vector3[size];
            for (var i = 0; i < size; i++) {
                newVertices[i] = _vertices[size - i - 1];
            }

            return new Polygon(newVertices, -1 * _normal, _center);
        }


        public Vector2[] FlatVertices(Vector3 normal) {
            return GeometryVG.FlattenPoints(_center, _vertices, normal);
        }

        public Vector2[] FlatVertices() {
            return FlatVertices(_normal);
        }


        //returns a clone of this Polygon scales around its center
        public Polygon Scale(float scaleFactor, Vector3 center) {
            var newVertices = new Vector3[VertexCount];

            for (int i = 0; i < newVertices.Length; i++) {
                newVertices[i] = (_vertices[i] - center) * scaleFactor;
            }

            return new Polygon(newVertices, _normal, _center);
        }

        //returns a clone of this Polygon scales around its center
        public Polygon Scale(float scaleFactor) {
            return Scale(scaleFactor, _center);
        }

        // returns a clone of this Polygon offset by a given vector
        public Polygon Translate(Vector3 translationVector) {
            var newVertices = new Vector3[VertexCount];
            var newCenter = _center + translationVector;

            for (int i = 0; i < newVertices.Length; i++) {
                newVertices[i] = _vertices[i] + translationVector;
            }

            return new Polygon(newVertices, _normal, newCenter);
        }

        // returns a clone of this Polygon with each vertex operated on by the 4x4 matrix
        public Polygon Transform(Matrix4x4 matrix) {
            var size = VertexCount;
            var newVertices = new Vector3[size];
            var newNormal = matrix.MultiplyVector(_normal);
            var newCenter = matrix.MultiplyPoint3x4(_center);

            for (var i = 0; i < size; i++) {
                newVertices[i] = matrix.MultiplyPoint3x4(_vertices[i]);
            }

            return new Polygon(newVertices, newNormal, newCenter);
        }

        public Polygon Offset(float amount) {
            var newVertices = (Vector3[])_vertices.Clone();
            var rotation = Quaternion.FromToRotation(_normal, Vector3.forward);

            GeometryVG.TransformPoints(newVertices, Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one));

            Matrix2x2 rotation2D = new Matrix2x2(-Vector2.up, Vector2.right);
            Vector2 lastNormal = rotation2D * (newVertices[0] - newVertices[VertexCount - 1]).normalized * amount;
            Vector2 normalA;
            Vector2 normalB = lastNormal;

            for (var i = 0; i < VertexCount - 1; i++) {
                normalA = normalB;
                normalB = rotation2D * (newVertices[(i + 1) % VertexCount] - newVertices[i]).normalized * amount;
                newVertices[i] += (Vector3)GeometryVG.Miter(normalA, normalB); ;
            }
            newVertices[VertexCount - 1] += (Vector3)GeometryVG.Miter(normalB, lastNormal);

            rotation = Quaternion.Inverse(rotation);
            GeometryVG.TransformPoints(newVertices, Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one));

            return new Polygon(newVertices, _normal, GeometryVG.VectorAvg(newVertices));
        }

        public Polygon Offset(float[] amounts) {
            var newVertices = (Vector3[])_vertices.Clone();
            var rotation = Quaternion.FromToRotation(_normal, Vector3.forward);

            GeometryVG.TransformPoints(newVertices, Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one));

            Matrix2x2 rotation2D = new Matrix2x2(-Vector2.up, Vector2.right);
            Vector2 lastNormal = rotation2D * (newVertices[0] - newVertices[VertexCount - 1]).normalized * amounts[VertexCount - 1];
            Vector2 normalA;
            Vector2 normalB = lastNormal;

            for (var i = 0; i < VertexCount - 1; i++) {
                normalA = normalB;
                normalB = rotation2D * (newVertices[(i + 1) % VertexCount] - newVertices[i]).normalized * amounts[i];
                newVertices[i] += (Vector3)GeometryVG.Miter(normalA, normalB); ;
            }
            newVertices[VertexCount - 1] += (Vector3)GeometryVG.Miter(normalB, lastNormal);

            rotation = Quaternion.Inverse(rotation);
            GeometryVG.TransformPoints(newVertices, Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one));

            return new Polygon(newVertices, _normal, GeometryVG.VectorAvg(newVertices));
        }

        public void CopyVertices(List<Vector3> dest) {
            dest.AddRange(_vertices);
        }

        public void CopyTris(List<int> dest, int offset) {
            var vertices2D = GeometryVG.FlattenPoints(_center, _vertices, _normal);
            GeometryVG.TriangulateSegment(vertices2D, dest, 0, 1, offset);
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

        private static Vector3 CalcNormal(Vector3[] vertices) {
            var tmpNormal = GeometryVG.PointsNormal(vertices).normalized;
            if (tmpNormal.sqrMagnitude < 0.01f) {
                throw new InvalidPolygonException("Points are Colinear");
            }

            //check if coplanar 
            Plane p = new Plane(tmpNormal, vertices[0]);
            for (var i = 3; i < vertices.Length; i++) {
                if (p.GetDistanceToPoint(vertices[i]) > 0.01f) {
                    throw new InvalidPolygonException("Not Planar");
                }
            }
            //check for real normal
            var shoelace = GeometryVG.Shoelace(GeometryVG.FlattenPoints(vertices, tmpNormal));
            if (shoelace > 0.001f) {
                return -tmpNormal;
            } else if (shoelace < -0.001f) {
                return tmpNormal;
            } else {
                throw new InvalidPolygonException("Points are Colinear");
            }
        }
    }
}
