using System;
using UnityEngine;
using System.Collections.Generic;
using Voxelgon.Geometry;
using System.Linq;

namespace Voxelgon.Geometry2D {
    public class SimplePolygon2D : IPolygon2D {
        #region Fields

        private readonly List<Vector2> _vertices;

        #endregion

        #region Constructors

        public SimplePolygon2D(IEnumerable<Vector2> vertices) {
            _vertices = vertices.ToList();

            if (VertexCount < 3) {
                throw new InvalidPolygonException("Less than 3 vertices");
            }
        }

        public SimplePolygon2D(Vector2 center, float radius, int sideCount, Vector2 tangent = default(Vector2)) {
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

        private SimplePolygon2D(List<Vector2> vertices) {
            _vertices = vertices;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns true if the polygon is convex
        /// </summary>
        public bool IsConvex {
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

        /// <summary>
        /// The area of the polygon
        /// </summary>
        public float Area {
            get { return GeometryVG.Shoelace(_vertices) * 0.5f; }
        }

        /// <summary>
        /// The geometric center of the polygon
        /// </summary>
        public Vector2 Center {
            get { return GeometryVG.VectorAvg(_vertices); }
        }

        /// <summary>
        /// The number of vertices in the polygon
        /// </summary>
        public int VertexCount {
            get { return _vertices.Count; }
        }

        /// <summary>
        /// The vertices that make up the polygon
        /// </summary>
        public IList<Vector2> Vertices {
            get { return _vertices.AsReadOnly(); }
        }

        /// <summary>
        /// The indices for the triangulated form of the polygon
        /// </summary>
        public IEnumerable<int> Indices {
            get { return Triangulation.Triangulate(_vertices); }
        }

        /// <summary>
        /// The normal vectors for each vertex
        /// </summary>
        public IEnumerable<Vector2> VertexNormals {
            get {
                for (var i = 0; i < VertexCount; i++) {
                    yield return GetVertexNormal(i);
                }
            }
        }

        /// <summary>
        /// The edge vectors for each vertex,
        /// the vector from the vertex to the next
        /// </summary>
        public IEnumerable<Vector2> Edges {
            get {
                for (var i = 0; i < VertexCount; i++) {
                    yield return GetEdge(i);
                }
            }
        }

        /// <summary>
        /// The edge normal vectors for each vertex,
        /// normalized and orthogonal to the edge vector
        /// </summary>
        public IEnumerable<Vector2> EdgeNormals {
            get {
                for (var i = 0; i < VertexCount; i++) {
                    yield return GetEdgeNormal(i);
                }
            }
        }

        /// <summary>
        /// The winding order of the polygon
        /// </summary>
        /// <remarks>
        /// 1 = clockwise
        ///-1 = counter-clockwise
        /// </remarks>
        public int WindingOrder {
            get { return GeometryVG.Shoelace(_vertices) > 0 ? 1 : -1; }
        }

        /// <summary>
        /// If the winding order is clockwise
        /// </summary>
        public bool Clockwise {
            get { return WindingOrder == 1; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the position for a vertex
        /// </summary>
        /// <param name="index">vertex index</param>
        /// <returns>the vertex's position vector</returns>
        public Vector2 GetVertex(int index) {
            return _vertices[index];
        }

        /// <summary>
        /// Gets the normal for a vertex
        /// </summary>
        /// <param name="index">vertex index</param>
        /// <returns>the vertex's normal vector</returns>
        public Vector2 GetVertexNormal(int index) {
            var vertex1 = _vertices[(index - 1 + VertexCount) % VertexCount];
            var vertex2 = _vertices[index];
            var vertex3 = _vertices[(index + 1) % VertexCount];

            var edgeNormal1 = (vertex2 - vertex1).Ortho();
            var edgeNormal2 = (vertex3 - vertex2).Ortho();

            return (edgeNormal1 + edgeNormal2).normalized;
        }

        /// <summary>
        /// Gets the edge vector for a vertex
        /// </summary>
        /// <param name="index">vertex index</param>
        /// <returns>the vertex's edge vector</returns>
        public Vector2 GetEdge(int index) {
            var vertex1 = _vertices[index];
            var vertex2 = _vertices[(index + 1) % VertexCount];

            return (vertex2 - vertex1);
        }

        /// <summary>
        /// Gets the edge normal vector for a vertex
        /// </summary>
        /// <param name="index">vertex index</param>
        /// <returns>the vertex's edge normal vector</returns>
        public Vector2 GetEdgeNormal(int index) {
            var edge = GetEdge(index);
            return edge.Ortho().normalized;
        }

        #region Transformation Methods

        /// <summary>
        /// Scales a polygon around its center
        /// </summary>
        /// <param name="scaleFactor">amount to scale by</param>
        /// <returns>A clone of this polygon scaled about its center</returns>
        public SimplePolygon2D Scale(float scaleFactor) {
            return Scale(scaleFactor, Center);
        }

        /// <summary>
        /// Scales a polygon around a point
        /// </summary>
        /// <param name="scaleFactor">amount to scale by</param>
        /// <param name="scaleCenter">point to scale around</param>
        /// <returns>A clone of this polygon scaled about <c>scaleCenter</c></returns>
        public SimplePolygon2D Scale(float scaleFactor, Vector2 scaleCenter) {
            return new SimplePolygon2D(_vertices.Select(o => scaleCenter + ((o - scaleCenter) * scaleFactor)));
        }

        /// <summary>
        /// Scales a polygon around its center
        /// </summary>
        /// <param name="scaleFactor">amount to scale by</param>
        /// <returns>A clone of this polygon scaled about its center</returns>
        public SimplePolygon2D Scale(Vector2 scaleFactor) {
            return Scale(scaleFactor, Center);
        }

        /// <summary>
        /// Scales a polygon around a point
        /// </summary>
        /// <param name="scaleFactor">amount to scale by</param>
        /// <param name="scaleCenter">point to scale around</param>
        /// <returns>A clone of this polygon scaled about <c>scaleCenter</c></returns>
        public SimplePolygon2D Scale(Vector2 scaleFactor, Vector2 scaleCenter) {
            return new SimplePolygon2D(_vertices.Select(o => {
                o -= scaleCenter;
                o.x *= scaleFactor.x;
                o.y *= scaleFactor.y;
                o += scaleCenter;
                return o;
            }));
        }

        /// <summary>
        /// Translates a polygon
        /// </summary>
        /// <param name="translateVector">vector to translate by</param>
        /// <returns>A clone of this polygon translated by <c>translateVector</c></returns>
        public SimplePolygon2D Translate(Vector2 translateVector) {
            return new SimplePolygon2D(_vertices.Select(o => o + translateVector));
        }

        /// <summary>
        /// Rotates a polygon around its center
        /// </summary>
        /// <param name="angle">angle in radians, counterclockwise</param>
        /// <returns>A clone of this polygon rotated around its center</returns>
        public SimplePolygon2D Rotate(float angle) {
            return Rotate(angle, Center);
        }

        /// <summary>
        /// Rotates a polygon around a point
        /// </summary>
        /// <param name="angle">angle in radians, counterclockwise</param>
        /// <param name="rotateCenter">point to rotate around</param>
        /// <returns>a clone of this polygon rotated around <c>rotateCenter</c></returns>
        public SimplePolygon2D Rotate(float angle, Vector2 rotateCenter) {
            var matrix = Matrix2x2.Rotation(angle);
            return new SimplePolygon2D(_vertices.Select(o => rotateCenter + matrix * (o - rotateCenter)));
        }

        /// <summary>
        /// Applies a transformation matrix on a polygon
        /// </summary>
        /// <param name="transformMatrix">transformation matrix</param>
        /// <returns>a clone of this polygon multiplied by the transformation matrix</returns>
        public SimplePolygon2D Transform(Matrix2x2 transformMatrix) {
            return new SimplePolygon2D(_vertices.Select(o => transformMatrix * o));
        }

        /// <summary>
        /// Applies a transformation matrix and translates a polygon
        /// </summary>
        /// <param name="transformMatrix">transformation matrix</param>
        /// <param name="translateVector">translation vector</param>
        /// <returns>a clone of this polygon multiplied by the transformation matrix and then translated</returns>
        public SimplePolygon2D Transform(Matrix2x2 transformMatrix, Vector2 translateVector) {
            return new SimplePolygon2D(_vertices.Select(o => translateVector + transformMatrix * o));
        }

        #endregion

        /// <summary>
        /// Ensures the polygon is clockwise
        /// </summary>
        /// <returns>This polygon, or a clone that has been reversed if it is not clockwise</returns>
        public IPolygon2D EnsureClockwise() {
            return WindingOrder != 1 ? Reverse() : this;
        }

        /// <summary>
        /// Reverses the polygon's winding order
        /// </summary>
        /// <returns>A clone of the polygon with the opposite winding order</returns>
        public IPolygon2D Reverse() {
            var size = VertexCount;
            var verts = new List<Vector2>(size);
            verts.Reverse();

            return new SimplePolygon2D(verts);
        }

        /// <summary>
        /// Truncates the polygon against a line
        /// </summary>
        /// <param name="point">position for the line</param>
        /// <param name="dir">direction for the line</param>
        /// <returns>the portion of the polygon on the right side of the line</returns>
        public IPolygon2D Truncate(Vector2 point, Vector2 dir) {
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

            return new SimplePolygon2D(verts);
        }

        public SimplePolygon2D Offset(float thickness) {
            var lastNormal = GetEdgeNormal(VertexCount - 1) * thickness;
            var i = 0;

            return new SimplePolygon2D(_vertices.Select(o => {
                var normal = GetEdgeNormal(i) * thickness;
                var value = o + GeometryVG.Miter(lastNormal, normal);
                lastNormal = normal;
                i++;
                return value;
            }));
        }

        public IPolygon2D Offset(IList<float> thicknesses) {
            var lastNormal = GetEdgeNormal(VertexCount - 1) * thicknesses.Last();
            var i = 0;

            return new SimplePolygon2D(_vertices.Select(o => {
                var normal = GetEdgeNormal(i) * thicknesses[i];
                var value = o + GeometryVG.Miter(lastNormal, normal);
                lastNormal = normal;
                i++;
                return value;
            }));
        }

        public void CopyVertices(List<Vector2> dest) {
            dest.AddRange(_vertices);
        }

        public void CopyTris(List<int> dest, int offset) {
            GeometryVG.TriangulateSegment(_vertices, dest, 0, 1, offset);
        }

        #region Debug Methods

        /// <summary>
        /// Draw the Polygon in worldspace
        /// </summary>
        public void Draw() {
            Draw(Color.blue);
        }

        /// <summary>
        /// Draw the Polygon in worldspace
        /// </summary>
        /// <param name="color">color to draw in</param>
        public void Draw(Color32 color) {
            for (int i = 0; i < VertexCount; i++) {
                Debug.DrawLine(_vertices[i], _vertices[(i + 1) % VertexCount], color);
            }
        }

        #endregion

        /// <summary>
        /// Checks if the given point is inside the polygon
        /// </summary>
        /// <param name="point">point to check</param>
        /// <returns>if the point is inside the polygon</returns>
        public bool Contains(Vector2 point) {
            var lastVertex = _vertices[VertexCount - 1];
            var ray = new Ray2D(point, Vector2.right);
            var counter = 0;

            for (var i = 0; i < VertexCount; i++) {
                var vertex = _vertices[i];
                if (Mathf.Approximately(vertex.y, point.y)) {
                    vertex.y += 0.01f;
                }
                if (Segment2D.RaycastSegment(ray, vertex, lastVertex)) counter++;
                lastVertex = vertex;
            }

            return (counter % 2) == 1;
        }

        /// <summary>
        ///  Checks if the polygons are equal
        /// </summary>
        /// <param name="other">the polygon to compare against</param>
        /// <returns>if the polygons are equal</returns>
        public bool Equals(IPolygon2D other) {
            if (!(other is SimplePolygon2D)) return false;
            if (other.VertexCount != VertexCount) return false;

            var otherVertices = other.Vertices;

            for (var i = 0; i < VertexCount; i++) {
                if (otherVertices[i] != _vertices[i]) return false;
            }
            return true;
        }

        #region Interface Overrides

        IEnumerable<SimplePolygon2D> IPolygon2D.Contours {
            get { yield return this; }
        }

        /// <summary>
        /// Scales a polygon around its center
        /// </summary>
        /// <param name="scaleFactor">amount to scale by</param>
        /// <returns>A clone of this polygon scaled about its center</returns>
        IPolygon2D IPolygon2D.Scale(float scaleFactor) {
            return Scale(scaleFactor);
        }

        /// <summary>
        /// Scales a polygon around a point
        /// </summary>
        /// <param name="scaleFactor">amount to scale by</param>
        /// <param name="scaleCenter">point to scale around</param>
        /// <returns>A clone of this polygon scaled about <c>scaleCenter</c></returns>
        IPolygon2D IPolygon2D.Scale(float scaleFactor, Vector2 scaleCenter) {
            return Scale(scaleFactor, scaleCenter);
        }

        /// <summary>
        /// Scales a polygon around its center
        /// </summary>
        /// <param name="scaleFactor">amount to scale by</param>
        /// <returns>A clone of this polygon scaled about its center</returns>
        IPolygon2D IPolygon2D.Scale(Vector2 scaleFactor) {
            return Scale(scaleFactor);
        }

        /// <summary>
        /// Scales a polygon around a point
        /// </summary>
        /// <param name="scaleFactor">amount to scale by</param>
        /// <param name="scaleCenter">point to scale around</param>
        /// <returns>A clone of this polygon scaled about <c>scaleCenter</c></returns>
        IPolygon2D IPolygon2D.Scale(Vector2 scaleFactor, Vector2 scaleCenter) {
            return Scale(scaleFactor, scaleCenter);
        }

        /// <summary>
        /// Translates a polygon
        /// </summary>
        /// <param name="translateVector">vector to translate by</param>
        /// <returns>A clone of this polygon translated by <c>translateVector</c></returns>
        IPolygon2D IPolygon2D.Translate(Vector2 translateVector) {
            return Translate(translateVector);
        }

        /// <summary>
        /// Rotates a polygon around its center
        /// </summary>
        /// <param name="angle">angle in radians, counterclockwise</param>
        /// <returns>A clone of this polygon rotated around its center</returns>
        IPolygon2D IPolygon2D.Rotate(float angle) {
            return Rotate(angle);
        }

        /// <summary>
        /// Rotates a polygon around a point
        /// </summary>
        /// <param name="angle">angle in radians, counterclockwise</param>
        /// <param name="rotateCenter">point to rotate around</param>
        /// <returns>a clone of this polygon rotated around <c>rotateCenter</c></returns>
        IPolygon2D IPolygon2D.Rotate(float angle, Vector2 rotateCenter) {
            return Rotate(angle, rotateCenter);
        }

        /// <summary>
        /// Applies a transformation matrix on a polygon
        /// </summary>
        /// <param name="transformMatrix">transformation matrix</param>
        /// <returns>a clone of this polygon multiplied by the transformation matrix</returns>
        IPolygon2D IPolygon2D.Transform(Matrix2x2 transformMatrix) {
            return Transform(transformMatrix);
        }

        /// <summary>
        /// Applies a transformation matrix and translates a polygon
        /// </summary>
        /// <param name="transformMatrix">transformation matrix</param>
        /// <param name="translateVector">translation vector</param>
        /// <returns>a clone of this polygon multiplied by the transformation matrix and then translated</returns>
        IPolygon2D IPolygon2D.Transform(Matrix2x2 transformMatrix, Vector2 translateVector) {
            return Transform(transformMatrix, translateVector);
        }

        IPolygon2D IPolygon2D.Offset(float thickness) {
            return Offset(thickness);
        }

        #endregion

        #endregion
    }
}