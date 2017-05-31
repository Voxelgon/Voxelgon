using System;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using SCG = System.Collections.Generic;
using C5;
using Voxelgon.Geometry;
using Voxelgon.Util;

namespace Voxelgon.Geometry2D {
    public class BooleanOps {
        private const float epsilon = 0.0001f;

        private readonly HashSet<Vertex> _outContours;

        // vertices to process
        private readonly TreeSet<Vertex> _vertexQueue;

        // edges currently intersected by the sweepline
        private readonly TreeSet<Edge> _edges;

        // dictionary to find edges by their left vertex
        //private readonly HashDictionary<Vertex, ArrayList<Edge>> _edgeDict;
        private readonly HashDictionary<Vertex, Edge> _edgeDict;

        private readonly IPolygon2D _subject;
        private bool _firstRun;
        private Vertex _currentVertex;

        public enum Operation {
            Or,
            And,
            Not,
            Xor
        }

        public BooleanOps(IPolygon2D subject) {
            _subject = subject;
            _vertexQueue = new TreeSet<Vertex>();
            _outContours = new HashSet<Vertex>();
            _edges = new TreeSet<Edge>();
            _edgeDict = new HashDictionary<Vertex, Edge>();
            _firstRun = true;
        }


        public void Intersect(IPolygon2D clip) {
            Operate(Operation.And, clip);
        }

        public void Union(IPolygon2D clip) {
            Operate(Operation.Or, clip);
        }

        public void Subtract(IPolygon2D clip) {
            Operate(Operation.Not, clip);
        }

        public void Xor(IPolygon2D clip) {
            Operate(Operation.Xor, clip);
        }

        public void Operate(Operation op, IPolygon2D clip) {
            var rightMaxima = new HashDictionary<Vertex, RightElbowData>();
            var safetyCounter = (clip.VertexCount + _subject.VertexCount) * 10;


            if (_firstRun) {
                UnwrapContour(_subject);
            }
            else {
                foreach (var c in _outContours) {
                    var current = c;
                    do {
                        _vertexQueue.Add(current);
                        current = current.Next;
                    } while (current != c);
                }
            }


            UnwrapContour(clip, op == Operation.Not || op == Operation.Xor);

            _outContours.Clear();
            _edges.Clear();
            _edgeDict.Clear();

            while (!_vertexQueue.IsEmpty && safetyCounter > 0) {
                safetyCounter--;
                _currentVertex = _vertexQueue.DeleteMin();

                var prevTop = (_currentVertex.CompareTo(_currentVertex.Prev) != -1);
                var nextTop = (_currentVertex.CompareTo(_currentVertex.Next) != 1);

                if (nextTop == prevTop) {
                    RemoveEdge(_currentVertex, nextTop);
                    AddEdge(_currentVertex, nextTop);
                }
                else if (nextTop) {
                    AddEdge(_currentVertex, true);
                    AddEdge(_currentVertex, false);
                }
                else {
                    var edge1 = RemoveEdge(_currentVertex, true);
                    var edge2 = RemoveEdge(_currentVertex, false);
                    var data = new RightElbowData();

                    if (edge1.CompareTo(edge2) == 1) {
                        SwapEdges(ref edge1, ref edge2);
                    }

                    data.Clockwise = edge1.Top;
                    if (op == Operation.Xor) {
                        data.WindingNumber = _edges.CountTo(edge1) + 1;
                    }
                    else {
                        var collection = _edges.RangeTo(edge1);
                        data.WindingNumber = collection.Sum(o => o.Top ? 1 : -1) + (data.Clockwise ? 1 : -1);
                    }

                    rightMaxima.Add(_currentVertex, data);

                    Edge nextEdge;
                    Edge prevEdge;
                    if (_edges.TrySuccessor(edge2, out nextEdge) && _edges.TryPredecessor(edge1, out prevEdge)) {
                        Intersect(prevEdge, nextEdge);
                    }
                }
            }

            while (rightMaxima.Count != 0 && safetyCounter > 0) {
                safetyCounter--;
                var pair = rightMaxima.Choose();
                var vertex = pair.Key;
                var data = pair.Value;

                rightMaxima.Remove(vertex);

                if (data.InResult(op) && vertex.Next != vertex.Prev) {
                    _outContours.Add(vertex);

                    var current = vertex;

                    do {
                        safetyCounter--;
                        rightMaxima.Remove(current);


                        current = current.Next;
                    } while (current != vertex && safetyCounter > 0);

                    if ((op == Operation.Xor) && data.XorInvert()) {
                        do {
                            var temp = current.Next;
                            current.Next = current.Prev;
                            current.Prev = temp;

                            current = current.Prev;
                        } while (current != vertex && safetyCounter > 0);
                    }
                }
            }

            if (safetyCounter == 0) throw new TimeoutException();

            _firstRun = false;
        }

        public IPolygon2D Read() {
            return new MultiPolygon2D(_outContours.Select(o => new SimplePolygon2D(EnumerateContour(o))));
        }


        private static void SwapEdges(ref Edge edge1, ref Edge edge2) {
            var temp = edge1;
            edge1 = edge2;
            edge2 = temp;
        }


        private void UnwrapContour(IPolygon2D polygon, bool reverse = false) {
            foreach (var c in polygon.Contours.Select(o => o.Vertices)) {
                var first = new Vertex(c[0]);
                var prev = first;
                _vertexQueue.Add(first);
                for (var i = 1; i < c.Count; i++) {
                    var position = reverse ? c[c.Count - i] : c[i];
                    // remove degenerate edges as we go
                    if (position != prev.Position) {
                        var vert = new Vertex(position);

                        prev.Next = vert;
                        vert.Prev = prev;

                        _vertexQueue.Add(vert);

                        prev = vert;
                    }
                }
                first.Prev = prev;
                prev.Next = first;
            }
        }

        private SCG.IEnumerable<Vector2> EnumerateContour(Vertex start) {
            var current = start;

            do {
                yield return current.Position;
                current = current.Next;
            } while (current != start);
        }

        private void AddEdge(Vertex left, bool top) {
            var edge = new Edge(left, top);
            _edges.Add(edge);

            if (_edgeDict.Contains(left)) {
                edge.Other = _edgeDict[left];
                edge.Other.Other = edge;
            }
            else {
                _edgeDict.Add(left, edge);
            }

            Edge nextEdge;
            Edge prevEdge;
            if (_edges.TrySuccessor(edge, out nextEdge)) Intersect(edge, nextEdge);
            if (_edges.TryPredecessor(edge, out prevEdge)) Intersect(prevEdge, edge);
        }

        private Edge RemoveEdge(Vertex right, bool top) {
            var left = top ? right.Prev : right.Next;

            var edge = _edgeDict[left];

            if (edge.Other != null) {
                if (edge.Top == top) {
                    //_edgeDict[left] = edge.Other;
                }
                else {
                    edge = edge.Other;
                }

                //edge.Other = edge.Other.Other = null;
            }
            else {
                //_edgeDict.Remove(left);
            }
            _edges.Remove(edge);


            return edge;
        }

        private void Intersect(Edge edge1, Edge edge2) {
            if (edge1.Left == edge2.Left || edge1.Right == edge2.Right) return;
            if (edge1.MinY - edge2.MaxY > epsilon) return;
            if (edge2.MinY - edge2.MaxY > epsilon) return;

            // save these variables so we dont have to type them all out each time
            var left1 = edge1.Left.Position;
            var left2 = edge2.Left.Position;
            var right1 = edge1.Right.Position;
            var right2 = edge2.Right.Position;

            if (GeoUtil2D.SqrDistance(left1, right2) < epsilon) return;
            if (GeoUtil2D.SqrDistance(left2, right1) < epsilon) return;

            //_edges.Remove(edge1);
            //_edges.Remove(edge2);

            bool intersect = false;
            int intersectCount = -1;
            Vector2 intersectPoint;
            Vertex vert1 = null, vert2 = null;
            Edge splitEdge = null;

            if (GeoUtil2D.SqrDistance(left1, left2) < epsilon) {
                intersect = true;
                vert1 = edge1.Left;
                vert2 = edge2.Left;
                intersect = (vert1.InArc(vert2.Prev) != vert1.InArc(vert2.Next));
            }
            else if (GeoUtil2D.SqrDistance(right1, right2) < epsilon) {
                vert1 = edge1.Right;
                vert2 = edge2.Right;
                intersect = (vert1.InArc(vert2.Prev) != vert1.InArc(vert2.Next));
            }

            else if (Segment2D.OnSegment(left2, right2, left1)) {
                if (edge2.VertexArea(edge1.Left.Prev) * edge2.VertexArea(edge1.Left.Next) < -epsilon) {
                    intersect = true;
                    vert1 = edge1.Left;
                    vert2 = edge2.Split(vert1.Position);
                }
            }
            else if (Segment2D.OnSegment(left2, right2, right1)) {
                if (edge2.VertexArea(edge1.Right.Prev) * edge2.VertexArea(edge1.Right.Next) < -epsilon) {
                    intersect = true;
                    vert1 = edge1.Right;
                    vert2 = edge2.Split(vert1.Position);
                }
            }
            else if (Segment2D.OnSegment(left1, right1, left2)) {
                if (edge1.VertexArea(edge2.Left.Prev) * edge1.VertexArea(edge2.Left.Next) < -epsilon) {
                    intersect = true;
                    vert2 = edge2.Left;
                    vert1 = edge1.Split(vert2.Position);
                }
            }
            else if (Segment2D.OnSegment(left1, right1, right2)) {
                if (edge1.VertexArea(edge2.Right.Prev) * edge1.VertexArea(edge2.Right.Next) < -epsilon) {
                    intersect = true;
                    vert2 = edge2.Right;
                    vert1 = edge1.Split(vert2.Position);
                }
            }
            else if (Segment2D.IntersectSegments(left1, right1, left2, right2, out intersectPoint)) {
                intersect = true;
                intersectCount = 3;
                vert1 = edge1.Split(intersectPoint);
                vert2 = edge1.Split(intersectPoint);
            }

            if (intersect) {
                _vertexQueue.Remove(vert1);
                _vertexQueue.Remove(vert2);
                
                var next1 = vert2.Next;
                var next2 = vert1.Next;

                vert1.Next = next1;
                vert2.Next = next2;

                _vertexQueue.Add(vert1);
                _vertexQueue.Add(vert2);
            }

            //if (edge1.Right.CompareTo(_currentVertex) > 0) 
            //_edges.Add(edge1);
            //if (edge2.Right.CompareTo(_currentVertex) > 0) 
            //_edges.Add(edge2);
        }

        private bool EdgeSame(Edge edge1, Edge edge2) {
            if (edge1.Top != edge2.Top) {
                if (edge1.Top) {
                    SwapEdges(ref edge1, ref edge2);
                }
                var vert1 = edge1.Left;
                var vert2 = edge2.Right;
                _vertexQueue.Remove(edge1.Right);
                _vertexQueue.Remove(edge2.Left);
                _edges.Remove(edge1);
                _edges.Remove(edge2);
                vert2.Prev = edge1.Right.Prev;
                vert1.Prev = edge2.Left.Prev;
                vert1.Prev.Next = vert1;
                vert2.Prev.Next = vert2;
                return true;
            }
            return false;
        }

        private bool EdgeOverlapPartial(Edge edge1, Edge edge2, Vertex vert1, Vertex vert2) {
            if (edge1.Left.CompareTo(edge2.Left) == 1) {
                MiscUtil.Swap(ref edge1, ref edge2);
                MiscUtil.Swap(ref vert1, ref vert2);
            }

            //vert1 belongs to edge1, vert2 belongs to edge2
            if (edge1.Top != edge2.Top) {
                _edges.Remove(edge1);
                _edges.Remove(edge2);
                if (edge2.Top) {
                    vert1.Next = edge2.Right;
                    vert1.Next.Prev = vert1;
                }
                else {
                    vert1.Prev = edge2.Right;
                    vert1.Prev.Next = vert1;
                }
                edge1.Right = vert2;
                return true;
            }
            else {
                var vert3 = edge2.Right;
                var vert4 = edge1.Right;
                edge1.Right = vert3;
                edge2.Right = vert4;
                return true;
            }
            return false;
        }

        private class Vertex : IComparable<Vertex> {
            private Vertex _next;
            private Vertex _prev;

            public Vertex Next {
                get { return _next; }
                set {
                    if (value != this) {
                        _next = value;
                        _next._prev = this;
                    }
                }
            }

            public Vertex Prev {
                get { return _prev; }
                set {
                    if (value != this) {
                        _prev = value;
                        _prev._next = this;
                    }
                }
            }

            public readonly Vector2 Position;

            public Vertex(Vector2 position) {
                Position = position;
            }

            public int CompareTo(Vertex other) {
                if (this == other) return 0;
                if (Position != other.Position) {
                    if (Math.Abs(Position.x - other.Position.x) > epsilon) {
                        // x coords are different, sort by x
                        // x coords are different, sort by x
                        // lower x values come first
                        return Position.x < other.Position.x ? -1 : 1;
                    }
                    else {
                        // x coords equal but y coords are different, sort by y
                        // order doesnt strictly matter here as long as its consistant
                        return Position.y < other.Position.y ? -1 : 1;
                    }
                }
                else {
                    if (Prev == null || other.Prev == null) return 1;
                    return (Prev.CompareTo(other.Prev));
                }
            }

            public override string ToString() {
                return Position.ToString();
            }

            public bool InArc(Vertex test) {
                var area1 = GeoUtil2D.Triangle2Area(Prev.Position, Position, Next.Position);
                var area2 = GeoUtil2D.Triangle2Area(Prev.Position, Position, test.Position);
                var area3 = GeoUtil2D.Triangle2Area(Position, Next.Position, test.Position);
                return (Mathf.Abs(area1) < epsilon) ? area3 > 0 : area1 * area2 > 0 && area1 * area3 > 0;
            }

            public static bool Clockwise(Vertex v0, Vertex v1, Vertex v2) {
                return GeoUtil2D.Clockwise(v0.Position, v1.Position, v2.Position);
            }
        }

        private class Edge : IComparable<Edge> {
            public readonly bool Top;
            public Vertex Left { get; private set; }

            public Vertex Right {
                get { return Top ? Left.Next : Left.Prev; }
                set {
                    if (Top) {
                        Left.Next = value;
                    }
                    else {
                        Left.Prev = value;
                    }
                }
            }

            public Edge Other { get; set; }

            public float MinY {
                get { return Mathf.Min(Left.Position.y, Right.Position.y); }
            }

            public float MaxY {
                get { return Mathf.Max(Left.Position.y, Right.Position.y); }
            }

            public Vector2 LeftPos {
                get { return Left.Position; }
            }

            public Vector2 RightPos {
                get { return Right.Position; }
            }

            public Edge(Vertex leftVert, bool top) {
                Left = leftVert;
                Top = top;
            }

            public Vertex Split(Vector2 splitPoint) {
                var middle = new Vertex(splitPoint);
                if (Top) {
                    middle.Next = Right;
                    middle.Prev = Left;
                    // this must be in the correct order 
                    // to prevent crashes and infinite recursion :O
                }
                else {
                    middle.Prev = Right;
                    middle.Next = Left;
                }
                return middle;
            }

            public override string ToString() {
                return Left.Position + " " + Right.Position;
            }

            public float VertexArea(Vertex vert) {
                return GeoUtil2D.TriangleArea(LeftPos, RightPos, vert.Position);
            }

            public int CompareTo(Edge other) {
                if (other == this) return 0; //uh...
                if (other.Left == Left && other.Right == Right) return 0;
                if (MinY - other.MaxY > epsilon) return -1;
                if (other.MinY - MaxY > epsilon) return 1;

                var rightArea = VertexArea(other.Right);
                var leftArea = VertexArea(other.Left);

                if (Mathf.Abs(rightArea) < epsilon &&
                    Mathf.Abs(leftArea) < epsilon) {
                    // edges are colinear so uh
                    // sort by left vert I guess?
                    var left = Left.CompareTo(other.Left);
                    return (left == 0) ? Right.CompareTo(other.Right) : left;
                }
                else {
                    if (Mathf.Abs(leftArea) < epsilon) {
                        // same left node, sort by right node
                        return rightArea > 0 ? -1 : 1;
                    }
                    else if (Mathf.Abs(rightArea) < epsilon) {
                        return leftArea > 0 ? -1 : 1;
                    }
                    else if (Mathf.Approximately(LeftPos.x, other.LeftPos.x)) {
                        // different left node, sort by left node
                        return LeftPos.y > other.LeftPos.y ? -1 : 1;
                    }
                    else if (Left.CompareTo(other.Left) == -1) {
                        return other.VertexArea(Left) > 0 ? -1 : 1;
                    }
                    else {
                        return leftArea > 0 ? -1 : 1;
                    }
                }
            }
        }

        private struct RightElbowData {
            public int WindingNumber;
            public bool Clockwise;

            public bool InResult(Operation op) {
                switch (op) {
                    case Operation.Not:
                    case Operation.Or:
                    default:
                        return WindingNumber == (Clockwise ? 1 : 0);
                    case Operation.And:
                        return WindingNumber == (Clockwise ? 2 : 1);
                    case Operation.Xor:
                        return true;
                }
            }

            public bool XorInvert() {
                return (Clockwise == (WindingNumber % 2 == 0));
            }
        }
    }
}