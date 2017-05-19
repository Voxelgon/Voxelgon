using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using SCG = System.Collections.Generic;
using C5;
using Voxelgon.Geometry;

namespace Voxelgon.Geometry2D {
    public class BooleanOps {
        private readonly HashSet<Vertex> _outContours;

        // vertices to process
        private readonly IntervalHeap<Vertex> _vertexQueue;

        // edges currently intersected by the sweepline
        private readonly TreeSet<Edge> _edges;
        // dictionary to find edges by their left vertex
        private readonly HashDictionary<Vertex, ArrayList<Edge>> _edgeDict;

        private readonly IPolygon2D _subject;
        private bool _firstRun;

        public enum Operation {
            Or,
            And,
            Not,
            Xor
        }

        public BooleanOps(IPolygon2D subject) {
            _subject = subject;
            _vertexQueue = new IntervalHeap<Vertex>();
            _outContours = new HashSet<Vertex>();
            _edges = new TreeSet<Edge>();
            _edgeDict = new HashDictionary<Vertex, ArrayList<Edge>>();
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
            var rightElbows = new HashSet<Vertex>();
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

            while (!_vertexQueue.IsEmpty && safetyCounter > 0) {
                safetyCounter--;
                var vert = _vertexQueue.DeleteMin();

                var prevTop = (vert.CompareTo(vert.Prev) != -1);
                var nextTop = (vert.CompareTo(vert.Next) != 1);
                var elbow = (nextTop != prevTop);

                if (!elbow) {
                    RemoveEdge(vert, nextTop);
                    AddEdge(vert, nextTop, _vertexQueue);
                }
                else {
                    if (nextTop) {
                        AddEdge(vert, true, _vertexQueue);
                        AddEdge(vert, false, _vertexQueue);
                    }
                    else {

                        var edge1 = RemoveEdge(vert, true);
                        var edge2 = RemoveEdge(vert, false);
                        
                        SortEdges(ref edge1, ref edge2);
                        
                        bool clockwise = edge1.Top;
                        var upperEdges = _edges.RangeTo(edge1);
                        var windingNumber = upperEdges.Sum(o => o.Top ? 1 : -1) + (clockwise ? 1 : -1);


                        Debug.Log(windingNumber + " " + vert);

                        switch (op) {
                            default:
                            case Operation.Not:
                            case Operation.Or:
                                if ((clockwise && windingNumber == 1) ||
                                    (!clockwise && windingNumber == 0)) rightElbows.Add(vert);
                                break;
                            case Operation.And:
                                if ((clockwise && windingNumber == 2) ||
                                    (!clockwise && windingNumber == 1)) rightElbows.Add(vert);
                                break;
                            case Operation.Xor:
                                rightElbows.Add(vert);
                                break;
                        }

                        if (edge1 == null || edge2 == null) throw new InvalidOperationException("Unknown error");

                        Edge nextEdge;
                        Edge prevEdge;
                        if (_edges.TrySuccessor(edge2, out nextEdge) && _edges.TryPredecessor(edge1, out prevEdge)) {
                            Intersect(prevEdge, nextEdge);
                        }
                    }
                }
            }

            while (rightElbows.Count != 0 && safetyCounter > 0) {
                safetyCounter--;
                var c = rightElbows.First();
                var current = c;
                //Debug.Log(current);

                do {
                    safetyCounter--;
                    rightElbows.Remove(current);
                    current = current.Next;
                } while (current != c && safetyCounter > 0);

                _outContours.Add(c);
            }

            if (op == Operation.Xor) {
                foreach (var c in _outContours) {
                    var winding = GeoUtil.WindingOrder(c.Prev.Position, c.Position, c.Next.Position);
                    if (winding == -1) {
                        var current = c;
                        do {
                            safetyCounter--;
                            var lastNext = current.Next;
                            current.Next = current.Prev;
                            current.Prev = lastNext;
                            current = lastNext;
                        } while (current != c && safetyCounter > 0);
                    }
                }
            }

            if (safetyCounter == 0) throw new TimeoutException();

            _firstRun = false;
        }

        public IPolygon2D Read() {
            return new MultiPolygon2D(_outContours.Select(o => new SimplePolygon2D(EnumerateContour(o))));
        }

        private bool SortEdges(ref Edge edge1, ref Edge edge2) {
            if (edge1.CompareTo(edge2) == 1) {
                var temp = edge1;
                edge1 = edge2;
                edge2 = temp;
                return false;
            }
            return true;
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

                        vert.Prev = prev;
                        prev.Next = vert;

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
        
        private void AddEdge(Vertex left, bool top, IntervalHeap<Vertex> heap) {
            var edge = new Edge(left, top);
            _edges.Add(edge);

            if (!_edgeDict.Contains(left)) {
                _edgeDict.Add(left, new ArrayList<Edge>());
            }
            _edgeDict[left].Add(edge);

            Edge nextEdge;
            Edge prevEdge;
            if (_edges.TrySuccessor(edge, out nextEdge)) Intersect(edge, nextEdge);
            if (_edges.TryPredecessor(edge, out prevEdge)) Intersect(prevEdge, edge);
        }

        private Edge RemoveEdge(Vertex right, bool top) {
            var left = top ? right.Prev : right.Next;
            var edge = GetEdge(left, right, top);
                
            if (edge != null) {
                _edges.Remove(edge);
                _edgeDict[left].Remove(edge);
            }
            
            return edge;
        }

        private  Edge GetEdge(Vertex left, Vertex right, bool top) {
            if (!_edgeDict.Contains(left)) return null;
            if (_edgeDict[left].Count == 0) {
                _edgeDict.Remove(left);
                return null;
            }
            return _edgeDict[left].First(o => o.RightVert == right);
        }
        
        private void Intersect(Edge node1, Edge node2) {
            if (node1.LeftVert == node2.LeftVert ||
                node1.RightVert == node2.RightVert) return;

            Vector2 vertPos;

            // we never change anything about the left vert of the nodes, so use that as an anchor

            if (Vector3.SqrMagnitude(node1.LeftVert.Position - node2.LeftVert.Position) < 0.0001) {
                var vert1 = node1.LeftVert;
                var vert2 = node2.LeftVert;

                var c1 = Vertex.Clockwise(vert1.Prev, vert1, vert2.Next);
                var c2 = Vertex.Clockwise(vert1.Next, vert1, vert2.Next);
                var c3 = Vertex.Clockwise(vert1.Prev, vert1, vert2.Prev);
                var c4 = Vertex.Clockwise(vert1.Next, vert1, vert2.Prev);

                if ((c1 == c2) != (c3 == c4)) {
                    Debug.Log("Left end intersection at " + vert1);
                    // the triangles formed by these two ears overlap
                    // swap the next verts for them

                    var tempNext = vert1.Next;
                    vert1.Next = vert2.Next;
                    vert2.Next = tempNext;

                    vert1.Next.Prev = vert1;
                    vert2.Next.Prev = vert2;
                    _edges.Remove(node1);
                    _edges.Remove(node2);
                    _edges.Add(node1);
                    _edges.Add(node2);
                }
            }
            else if (Vector3.SqrMagnitude(node1.RightVert.Position - node2.RightVert.Position) < 0.0001) {
                var vert1 = node1.RightVert;
                var vert2 = node2.RightVert;

                var c1 = Vertex.Clockwise(vert1.Prev, vert1, vert2.Next);
                var c2 = Vertex.Clockwise(vert1.Next, vert1, vert2.Next);
                var c3 = Vertex.Clockwise(vert1.Prev, vert1, vert2.Prev);
                var c4 = Vertex.Clockwise(vert1.Next, vert1, vert2.Prev);

                if ((c1 == c2) != (c3 == c4)) {
                    Debug.Log("Right end intersection at " + vert1);
                    // the triangles formed by these two ears overlap
                    // swap the next verts for them

                    var tempNext = vert1.Next;
                    vert1.Next = vert2.Next;
                    vert2.Next = tempNext;

                    vert1.Next.Prev = vert1;
                    vert2.Next.Prev = vert2;
                }
            }
            else if (Segment2D.IntersectSegments(
                node1.LeftVert.Position, node1.RightVert.Position,
                node2.LeftVert.Position, node2.RightVert.Position, out vertPos)) {
                // intersection

                var vert1 = new Vertex(vertPos, true);
                var vert2 = new Vertex(vertPos, true);

                Debug.Log("Regular intersection at " + vert1);

                vert1.Prev = node1.Top ? node1.LeftVert : node1.RightVert;
                vert2.Prev = node2.Top ? node2.LeftVert : node2.RightVert;

                vert1.Next = vert2.Prev.Next;
                vert2.Next = vert1.Prev.Next;

                vert1.Prev.Next = vert1.Next.Prev = vert1;
                vert2.Prev.Next = vert2.Next.Prev = vert2;

                _vertexQueue.Add(vert1);
                _vertexQueue.Add(vert2);
            }
        }
        
        

        private class Vertex : IComparable<Vertex> {
            public Vertex Next;
            public Vertex Prev;

            public readonly Vector2 Position;

            private readonly bool _intersection;

            public Vertex(Vector2 position, bool intersection = false) {
                _intersection = intersection;
                Position = position;
            }

            public int CompareTo(Vertex other) {
                if (this == other) return 0;


                if (Position != other.Position) {
                    if (Math.Abs(Position.x - other.Position.x) > 0.001f) {
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
                    if (Next == null || Prev == null || other.Next == null || other.Prev == null) return 0;

                    var offset1 = (Position + Next.Position + Prev.Position) / 3;
                    var offset2 = (other.Position + other.Next.Position + other.Prev.Position) / 3;

                    if (Math.Abs(offset1.x - offset2.x) > 0.001f) {
                        // x coords are different, sort by x
                        // lower x values come first
                        return offset1.x < offset2.x ? -1 : 1;
                    }
                    else {
                        // x coords equal but y coords are different, sort by y
                        // order doesnt strictly matter here as long as its consistant
                        return offset1.y < offset2.y ? -1 : 1;
                    }
                }
            }

            public override string ToString() {
                return Position.ToString();
            }

            public static bool Clockwise(Vertex v0, Vertex v1, Vertex v2) {
                return GeoUtil.Clockwise(v0.Position, v1.Position, v2.Position);
            }
        }
        /*

        private class SweepLine {
            private readonly TreeSet<Edge> _edges;
            private readonly HashDictionary<Vertex, ArrayList<Edge>> _edgeDict;

            public SweepLine() {
                _edges = new TreeSet<Edge>();
                _edgeDict = new HashDictionary<Vertex, ArrayList<Edge>>();
            }

            public void AddEdge(Vertex left, bool top, IntervalHeap<Vertex> heap) {
                var edge = new Edge(left, top);
                _edges.Add(edge);

                if (!_edgeDict.Contains(left)) {
                    _edgeDict.Add(left, new ArrayList<Edge>());
                }
                _edgeDict[left].Add(edge);

                IntersectNeighbors(edge, heap);
            }

            public void RemoveEdge(Vertex right, bool top) {
                var left = top ? right.Prev : right.Next;
                var edge = GetEdge(left, right, top);
                
                if (edge != null) {
                    _edges.Remove(edge);
                    _edgeDict[left].Remove(edge);
                }
            }

            public Edge GetEdge(Vertex left, Vertex right, bool top) {
                if (!_edgeDict.Contains(left)) return null;
                if (_edgeDict[left].Count == 0) {
                    _edgeDict.Remove(left);
                    return null;
                }
                return _edgeDict[left].First(o => o.RightVert == right);
            }

            public void IntersectNeighbors(Edge edge, IntervalHeap<Vertex> heap) {
                Edge next;
                if (_edges.TrySuccessor(edge, out next)) Intersect(edge, next, heap);
                Edge prev;
                if (_edges.TryPredecessor(edge, out prev)) Intersect(prev, edge, heap);
            }

            public int WindingNumber(Vertex vert) {
                if (_edgeDict[vert.Prev].Count < 1) return 0;
                if (_edgeDict[vert.Next].Count < 1) return 0;

                var edge1 = _edgeDict[vert.Prev].First(o => o.RightVert == vert);
                var edge2 = _edgeDict[vert.Next].First(o => o.RightVert == vert);
                var upper = (edge1.CompareTo(edge2) == -1) ? edge1 : edge2;

                var collection = _edges.RangeTo(upper);
                return collection.Sum(o => o.Top ? 1 : -1) + (upper.Top ? 1 : -1);
            }


            #region Private Methods

            private void Intersect(Edge node1, Edge node2, IntervalHeap<Vertex> heap) {
                if (node1.LeftVert == node2.LeftVert ||
                    node1.RightVert == node2.RightVert) return;

                Vector2 vertPos;

                // we never change anything about the left vert of the nodes, so use that as an anchor

                if (Vector3.SqrMagnitude(node1.LeftVert.Position - node2.LeftVert.Position) < 0.0001) {
                    var vert1 = node1.LeftVert;
                    var vert2 = node2.LeftVert;

                    var c1 = Vertex.Clockwise(vert1.Prev, vert1, vert2.Next);
                    var c2 = Vertex.Clockwise(vert1.Next, vert1, vert2.Next);
                    var c3 = Vertex.Clockwise(vert1.Prev, vert1, vert2.Prev);
                    var c4 = Vertex.Clockwise(vert1.Next, vert1, vert2.Prev);

                    if ((c1 == c2) != (c3 == c4)) {
                        Debug.Log("Left end intersection at " + vert1);
                        // the triangles formed by these two ears overlap
                        // swap the next verts for them

                        var tempNext = vert1.Next;
                        vert1.Next = vert2.Next;
                        vert2.Next = tempNext;

                        vert1.Next.Prev = vert1;
                        vert2.Next.Prev = vert2;
                        _edges.Remove(node1);
                        _edges.Remove(node2);
                        _edges.Add(node1);
                        _edges.Add(node2);
                    }
                }
                else if (Vector3.SqrMagnitude(node1.RightVert.Position - node2.RightVert.Position) < 0.0001) {
                    var vert1 = node1.RightVert;
                    var vert2 = node2.RightVert;

                    var c1 = Vertex.Clockwise(vert1.Prev, vert1, vert2.Next);
                    var c2 = Vertex.Clockwise(vert1.Next, vert1, vert2.Next);
                    var c3 = Vertex.Clockwise(vert1.Prev, vert1, vert2.Prev);
                    var c4 = Vertex.Clockwise(vert1.Next, vert1, vert2.Prev);

                    if ((c1 == c2) != (c3 == c4)) {
                        Debug.Log("Right end intersection at " + vert1);
                        // the triangles formed by these two ears overlap
                        // swap the next verts for them

                        var tempNext = vert1.Next;
                        vert1.Next = vert2.Next;
                        vert2.Next = tempNext;

                        vert1.Next.Prev = vert1;
                        vert2.Next.Prev = vert2;
                    }
                }
                else if (Segment2D.IntersectSegments(
                    node1.LeftVert.Position, node1.RightVert.Position,
                    node2.LeftVert.Position, node2.RightVert.Position, out vertPos)) {
                    // intersection

                    var vert1 = new Vertex(vertPos, true);
                    var vert2 = new Vertex(vertPos, true);

                    Debug.Log("Regular intersection at " + vert1);

                    vert1.Prev = node1.Top ? node1.LeftVert : node1.RightVert;
                    vert2.Prev = node2.Top ? node2.LeftVert : node2.RightVert;

                    vert1.Next = vert2.Prev.Next;
                    vert2.Next = vert1.Prev.Next;

                    vert1.Prev.Next = vert1.Next.Prev = vert1;
                    vert2.Prev.Next = vert2.Next.Prev = vert2;

                    heap.Add(vert1);
                    heap.Add(vert2);
                }
            }

            #endregion
        }*/

        private class Edge : IComparable<Edge> {
            public readonly bool Top;

            public Vertex LeftVert { get; private set; }

            public Vertex RightVert {
                get { return Top ? LeftVert.Next : LeftVert.Prev; }
            }

            private float MinY {
                get { return Mathf.Min(LeftVert.Position.y, RightVert.Position.y); }
            }

            private float MaxY {
                get { return Mathf.Max(LeftVert.Position.y, RightVert.Position.y); }
            }

            public Edge(Vertex leftVert, bool top) {
                LeftVert = leftVert;
                Top = top;
            }

            public override string ToString() {
                return LeftVert.Position + " " + RightVert.Position;
            }

            public int CompareTo(Edge other) {
                if (other == this) return 0; //uh...

                if (MinY - other.MaxY > 0.001f) return -1;
                if (other.MinY - MaxY > 0.001f) return 1;

                var rightArea = GeoUtil.TriangleArea(LeftVert.Position, RightVert.Position,
                    other.RightVert.Position);
                var leftArea = GeoUtil.TriangleArea(LeftVert.Position, RightVert.Position,
                    other.LeftVert.Position);

                if (Mathf.Abs(rightArea) < 0.001f &&
                    Mathf.Abs(leftArea) < 0.001f) {
                    // edges are colinear so uh
                    // sort by left vert I guess?
                    var left = LeftVert.CompareTo(other.LeftVert);
                    return (left == 0) ? RightVert.CompareTo(other.RightVert) : left;
                }
                else {
                    if (LeftVert.Position == other.LeftVert.Position) {
                        // same left node, sort by right node
                        return rightArea > 0 ? -1 : 1;
                    }
                    else if (Mathf.Approximately(LeftVert.Position.x, other.LeftVert.Position.x)) {
                        // different left node, sort by left node
                        return LeftVert.Position.y > other.LeftVert.Position.y ? -1 : 1;
                    }
                    else if (LeftVert.CompareTo(other.LeftVert) == -1) {
                        var otherArea =
                            GeoUtil.TriangleArea(other.LeftVert.Position, other.RightVert.Position,
                                LeftVert.Position);
                        return otherArea < 0 ? -1 : 1;
                    }
                    else {
                        return leftArea > 0 ? -1 : 1;
                    }
                }
            }
        }
    }
}