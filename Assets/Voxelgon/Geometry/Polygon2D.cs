using UnityEngine;
using System.Collections.Generic;
using Voxelgon.Geometry;
using System.Linq;


namespace Voxelgon.Geometry {

    public class Polygon2D {

        // FIELDS

        private readonly List<Vector2> _vertices;

        // CONSTRUCTORS

        public Polygon2D(Vector2[] vertices) {
            _vertices = new List<Vector2>(vertices);

            if (VertexCount < 3) {
                throw new InvalidPolygonException("Less than 3 vertices");
            }
        }

        public Polygon2D(IEnumerable<Vector2> vertices) {
            _vertices = vertices.ToList();

            if (VertexCount < 3) {
                throw new InvalidPolygonException("Less than 3 vertices");
            }
        }

        public Polygon2D(Vector2 center, float radius, int sideCount, Vector2 tangent = default(Vector2)) {
            _vertices = new List<Vector2>(sideCount);

            var matrix = Matrix2x2.Rotation(2 * Mathf.PI / sideCount);

            tangent.Normalize();
            if (tangent.sqrMagnitude < 0.1f) {
                tangent = Vector2.up;
            }

            for (int i = 0; i < sideCount; i++) {
                _vertices.Add(center + (tangent * radius));
                tangent = matrix * tangent;
            }
        }

        private Polygon2D(List<Vector2> vertices) {
            _vertices = vertices;
        }

        // PROPERTIES


        //is the polygon convex?
        public bool IsConvex {
            //TODO use better algorithm
            get {
                for (int i = 0; i < VertexCount; i++) {
                    int j = (i + 1) % VertexCount;
                    int k = (i + 2) % VertexCount;

                    if (GeometryVG.TriangleWindingOrder2D(
                            _vertices[i],
                            _vertices[j],
                            _vertices[k]) != 1) {
                        return false;
                    }
                }

                return true;
            }
        }

        //the area of the polygon
        public float Area {
            get { return Mathf.Abs(GeometryVG.Shoelace(_vertices) / 2); }
        }

        //the number of vertices in the polygon
        public int VertexCount {
            get { return _vertices.Count; }
        }

        //the polygon's vertices as an array
        public IEnumerable<Vector2> Vertices {
            get { return _vertices.AsEnumerable(); }
        }

        //the polygon's vertex normals as a new list
        public IEnumerable<Vector2> VertexNormals {
            get {
                for (var i = 0; i < VertexCount; i++) {
                    yield return GetVertexNormal(i);
                }
            }
        }

        //the polygon's geometric center
        public Vector2 Center {
            get { return GeometryVG.VectorAvg(_vertices); }
        }

        //winding order
        // 1 = clockwise
        //-1 = counter-clockwise
        public int WindingOrder {
            get { return GeometryVG.Shoelace(_vertices) > 0 ? 1 : -1; }
        }

        public List<int> TriangleIndices {
            get {
                var indices = new List<int>(VertexCount * 3);
                GeometryVG.TriangulateSegment(_vertices, indices, 0, 1);

                return indices;
            }
        }

        // METHODS

        //if the polygon is counter-clockwise, reverse it so it is clockwise
        public Polygon2D EnsureClockwise(Vector3 normal) {
            if (WindingOrder != 1) return Reverse();
            return this;
        }

        //returns whether or not `point` is on or inside the polygon
        public bool Contains(Vector3 point) {
            //TODO: use faster algorithm
            var indices = TriangleIndices;
            for (var i = 0; i < indices.Count; i += 3) {
                if (GeometryVG.TriangleContains2D(
                        _vertices[indices[i]],
                        _vertices[indices[i + 1]],
                        _vertices[indices[i + 2]],
                        point)) {
                    return true;
                }
            }

            return false;
        }

        //returns a polygon truncated by the given line on the right hand side
        public Polygon2D Truncate(Vector2 point, Vector2 dir) {
            var verts = new List<Vector2>();
            var trim = new List<int>();
            int start = 0;


            for (int i = 0; i < VertexCount; i++) {
                if (GeometryVG.TriangleWindingOrder2D(point, point + dir, (_vertices[i])) > 0) {
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

                    var segmentPoint = _vertices[trim[i]];
                    var segmentDir = _vertices[lastVert] - segmentPoint;
                    var intersection = GeometryVG.LineIntersection2D(point, dir, segmentPoint, segmentDir);
                    verts.Add(intersection);
                }
                if (trim[nextTrim] != nextVert || trim.Count == 1) {
                    start = trim[i] + 1;

                    var segmentPoint = _vertices[trim[i]];
                    var segmentDir = _vertices[nextVert] - segmentPoint;
                    var intersection = GeometryVG.LineIntersection2D(point, dir, segmentPoint, segmentDir);
                    verts.Add(intersection);
                }
            }

            if (trim.Count == 0) {
                return this;
            }

            return new Polygon2D(verts);
        }

        //returns the vertex at index `index`
        public Vector2 GetVertex(int index) {
            return _vertices[index];
        }

        //returns the vector pointing "out" of the vertex at index `index`
        //normalized average of two adjacent edge normals
        public Vector2 GetVertexNormal(int index) {

            var vertex1 = _vertices[(index - 1 + VertexCount) % VertexCount];
            var vertex2 = _vertices[index];
            var vertex3 = _vertices[(index + 1) % VertexCount];

            var edgeNormal1 = (vertex2 - vertex1).Ortho();
            var edgeNormal2 = (vertex3 - vertex2).Ortho();

            return (edgeNormal1 + edgeNormal2).normalized;
        }

        //returns the edge vector at index `index`
        //vector from a vertex to the following vertex
        public Vector2 GetEdge(int index) {
            var vertex1 = _vertices[index];
            var vertex2 = _vertices[(index + 1) % VertexCount];

            return (vertex2 - vertex1);
        }

        //returns the edge normal at index `index`
        public Vector2 GetEdgeNormal(int index) {
            var edge = GetEdge(index);
            return new Vector2(edge.y, -edge.x);
        }

        //reverses the polygon's winding order
        public Polygon2D Reverse() {
            var size = VertexCount;
            var verts = new List<Vector2>(size);
            verts.Reverse();

            return new Polygon2D(verts);
        }


        //returns a clone of this Polygon scales around its center
        public Polygon2D Scale(float scaleFactor, Vector2 center) {
            return new Polygon2D(_vertices.Select(o => (o - center) * scaleFactor));
        }

        //returns a clone of this Polygon scales around its center
        public Polygon2D Scale(float scaleFactor) {
            return Scale(scaleFactor, Center);
        }

        // returns a clone of this Polygon offset by a given vector
        public Polygon2D Translate(Vector2 translationVector) {
            return new Polygon2D(_vertices.Select(o => o + translationVector));
        }

        // returns a clone of this Polygon with each vertex operated on by the 4x4 matrix
        public Polygon2D Transform(Matrix4x4 matrix) {
            return new Polygon2D(_vertices.Select(o => (Vector2)matrix.MultiplyPoint3x4(o)));
        }

        public Polygon2D Offset(float amount) {

            var verts = _vertices.ToList();
            Vector2 normalA;
            Vector2 normalB = (verts[0] - verts[VertexCount - 1]).normalized.Ortho() * amount;

            for (var i = 0; i < VertexCount; i++) {
                normalA = normalB;
                normalB = (verts[(i + 1) % VertexCount] - verts[i]).normalized.Ortho() * amount;
                verts[i] += GeometryVG.Miter(normalA, normalB); ;
            }
            //verts[VertexCount - 1] += GeometryVG.Miter(normalB, lastNormal);

            return new Polygon2D(verts);
        }

        public Polygon2D Offset(IEnumerable<float> amounts) {

            var verts = _vertices.ToList();
            Vector2 normalA;
            Vector2 normalB = (verts[0] - verts[VertexCount - 1]).normalized.Ortho() * amounts.Last();

            var enumerator = amounts.GetEnumerator();

            for (var i = 0; i < VertexCount; i++) {
                normalA = normalB;
                normalB = (verts[(i + 1) % VertexCount] - verts[i]).normalized.Ortho() * enumerator.Current;
                verts[i] += GeometryVG.Miter(normalA, normalB); ;
                enumerator.MoveNext();
            }

            return new Polygon2D(verts);
        }

        public void CopyVertices(List<Vector2> dest) {
            dest.AddRange(_vertices);
        }

        public void CopyTris(List<int> dest, int offset) {
            GeometryVG.TriangulateSegment(_vertices, dest, 0, 1, offset);
        }

        //draw the polygon in the world for 1 frame
        public void Draw() {
            for (int i = 0; i < VertexCount; i++) {
                Debug.DrawLine(_vertices[i], _vertices[(i + 1) % VertexCount]);
            }
        }

        //are the polygons equal?
        public bool Equals(Polygon p) {
            if (VertexCount != p.VertexCount) { return false; }

            for (int i = 0; i < VertexCount; i++) {
                if (!GetVertex(i).Equals(p.GetVertex(i))) {
                    return false;
                }
            }

            return true;
        }
    }
}
