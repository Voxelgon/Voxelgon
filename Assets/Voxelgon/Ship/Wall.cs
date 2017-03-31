using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using Voxelgon.Collections;
using Voxelgon.Util;
using Voxelgon.Util.Geometry;

namespace Voxelgon.Ship {

    public class Wall : ISelectable {

        //Fields

        private readonly Hull _hull;

        private LoopList<Node> _nodes = new LoopList<Node>();

        private MeshFragment _mesh = null;
        private MeshBuilder _meshBuilder = new MeshBuilder();

        private Plane _wallPlane;
        private bool _isPolygon = false;

        private bool _verticesChanged;

        private float _thickness = 0.2f;
        //total thickness of the wall

        private GameObject _collider;


        //Constructors

        public Wall(Hull hull) {
            _wallPlane = new Plane();
            _hull = hull;
        }

        //Properties

        public int VertexCount {
            get { return _nodes.Count; }
        }

        public bool IsPolygon {
            get { return _isPolygon && _nodes.Count > 2; }
        }

        public MeshFragment Mesh {
            get {
                if (_verticesChanged) {
                    BuildMesh();
                }
                return _mesh;
            }
        }

        public float Thickness {
            get {
                return _thickness;
            }

            set {
                if (value < 0.0 || value > 2.0) {
                    throw new ArgumentOutOfRangeException("value");
                }
                _thickness = value;
            }
        }

        public Vector3 Normal {
            get {
                if (IsPolygon) {
                    return ((Plane)_wallPlane).normal;
                }
                return Vector3.zero;
            }
        }

        public IEnumerable<Node> Nodes {
            get { return _nodes.AsEnumerable(); }
        }

        public IEnumerable<Edge> Edges {
            get {
                if (_nodes.Count < 3) {
                    throw new InvalidOperationException();
                }

                var node = _nodes.First;
                do {
                    yield return node.GetEdge(node = _nodes.GetNext(node));
                } while (node != _nodes.First);
            }
        }

        public short DeckLevel {
            get {
                var min = _nodes.Min(n => n.GridPosition.y);
                var max = _nodes.Max(n => n.GridPosition.y);
                if (max > min + 1) throw new InvalidOperationException();
                return min;
            }
        }

        public Hull Hull {
            get { return _hull; }
        }

        public Bounds Bounds {
            get { return _mesh.Bounds; }
        }

        public GameObject Selector {
            get { return _collider; }
        }


        //Methods

        public bool ContainsNode(Node vertex) {
            return _nodes.Contains(vertex);
        }

        public Vector3 GetEdgeNormal(Edge edge) {
            var p1 = edge.Node1;
            var p2 = edge.Node2;

            if (_nodes.Contains(p1) && _nodes.Contains(p2)) {
                if (_nodes.GetNext(p1) == p2) {
                    return Vector3.Cross(edge.Tangent, Normal);
                }

                if (_nodes.GetNext(p2) == p1) {
                    return Vector3.Cross(edge.Tangent, -Normal);
                }
            }
            throw new ArgumentOutOfRangeException("edge", "edge not present in this wall");
        }

        // IBoundable

        public bool Raycast(Ray ray) {
            //TODO
            throw new NotImplementedException();
        }

        public bool Raycast(Ray ray, out float distance) {
            //TODO
            throw new NotImplementedException();
        }

        // ISelectable

        public void MakeSelector(Transform parent) {
            //TODO
            throw new NotImplementedException();
        }

        public void DestroySelector() {
            GameObject.Destroy(_collider);
            _collider = null;
        }

        // Private Methods

        private bool AddNode(Node node) {
            throw new NotImplementedException();
        }

        private void Clear() {
            _verticesChanged = true;
            _nodes.Clear();
            _isPolygon = false;
        }

        private void CalcPlane() {
            if (VertexCount > 2 && !_isPolygon) {
                Node v1 = _nodes.First;
                Node v2 = _nodes.GetNext(v1);
                Node v3 = _nodes.GetNext(v2);

                do {
                    var normal = GeometryVG.TriangleNormal(v1.Position, v2.Position, v3.Position);
                    if (normal.sqrMagnitude > 0.001f) {
                        _wallPlane = new Plane(normal, v1.Position);
                        _isPolygon = true;
                        break;
                    }
                    v1 = v2;
                    v2 = v3;
                    v3 = _nodes.GetNext(v3);
                } while (v3 != _nodes.First);
            }
        }

        private void BuildMesh() {
            _verticesChanged = false;
            _meshBuilder.Clear();

            if (IsPolygon) {
                var profile = new Polygon(_nodes.Cast<Vector3>() as ICollection<Vector3>);
                _meshBuilder.AddPolygon(profile, Color.green);
            }
            _mesh = _meshBuilder.ToFragment();
        }
    }
}
