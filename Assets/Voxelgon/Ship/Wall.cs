using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using Voxelgon.Geometry;
using Voxelgon.Util.Grid;
using Voxelgon.Ship.Editor;


namespace Voxelgon.Ship {

    public class Wall {

        //Fields

        public readonly ShipEditor _editor;

        private List<GridVector> _nodes = new List<GridVector>();
        private List<GridSegment> _edges = new List<GridSegment>();

        private MeshFragment _mesh = null;
        private MeshBuilder _meshBuilder = new MeshBuilder();

        private Plane _wallPlane;
        private bool _isPolygon = false;

        private bool _verticesChanged;

        private float _thickness = 0.2f;
        //total thickness of the wall

        //Constructors

        public Wall() {
            _wallPlane = new Plane();
        }

        public Wall(ShipEditor editor) {
            _wallPlane = new Plane();
            _editor = editor;
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
                BuildMesh();
                if (_verticesChanged) {
                    //         BuildSimpleMesh();
                }
                return _mesh;
            }
        }

        public bool VerticesChanged {
            get { return _verticesChanged; }
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

        public List<GridVector> Nodes {
            get { return new List<GridVector>(_nodes); }
        }

        public List<GridSegment> Edges {
            get { return new List<GridSegment>(_edges); }
        }

        //Methods

        public GridVector GetNode(int index) {
            return _nodes[index];
        }

        public GridSegment GetEdge(int index) {
            return _edges[index];
        }

        public bool SetNodes(List<GridVector> nodes, ShipEditor.BuildMode mode) {
            Clear();
            foreach (var n in nodes) {
                if (!AddNode(n)) {
                    Clear();
                    return false;
                }
            }
            return true;
        }

        public bool ValidNode(GridVector node) {
            if (ContainsNode(node)) {
                return false;
            }
            if (IsPolygon) {
                return Mathf.Abs(((Plane)_wallPlane).GetDistanceToPoint((Vector3)node)) < 0.001;
            }
            return true;
        }

        public bool ContainsNode(GridVector vertex) {
            return _nodes.Contains(vertex);
        }


        //Private Methods

        private bool AddNode(GridVector node) {
            if (IsPolygon) {
                if (ValidNode(node)) {
                    _nodes.Add(node);
                    return true;
                } else {
                    return false;
                }
            } else {
                _nodes.Add(node);
                CalcPlane();
                return true;
            }
        }

        private void Clear() {
            _nodes.Clear();
            _edges.Clear();
            _isPolygon = false;
        }

        private void CalcPlane() {
            if (VertexCount > 2 && !_isPolygon) {
                for (int i = 0; i < _nodes.Count - 2; i++) {
                    var normal = Geometry.Geometry.TriangleNormal((Vector3)_nodes[0], (Vector3)_nodes[1], (Vector3)_nodes[2]);
                    if (normal.sqrMagnitude > 0.001f) {
                        _wallPlane = new Plane(normal, (Vector3)_nodes[0]);
                        _isPolygon = true;
                        break;
                    }
                }
            }
        }

        private void CalcEdges() {
            if (IsPolygon) {
                GridVector node1;
                GridVector node2 = _nodes.Last();
                _nodes.ForEach(o => {
                    node1 = node2;
                    node2 = o;
                    _edges.Add(new GridSegment(node1, node2));
                });
            }
        }

        private void BuildMesh() {
            _meshBuilder.Clear();

            if (IsPolygon) {
                var profile = new Polygon(_nodes);
                _meshBuilder.AddPolygon(profile, Color.green);
            }
            _mesh = _meshBuilder.ToFragment();
        }
    }
}
