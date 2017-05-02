using System.Collections.Generic;
using UnityEngine;
using Voxelgon.Geometry;
using Voxelgon.Ship.Editor;
using Voxelgon.Graphics;

namespace Voxelgon.Ship {

    public class Node : ISelectable {

        // FIELDS

        private GridVector _position;
        private Dictionary<Node, Edge> _edges;

        private GameObject _collider;


        // PROPERTIES

        public GridVector GridPosition {
            get { return _position; }
        }

        public Vector3 Position {
            get { return (Vector3)_position; }
        }

        public Bounds Bounds {
            get { return new Bounds(Position, Vector3.one * ShipEditor.NODE_RADIUS); }
        }

        public GameObject Selector {
            get { return _collider; }
        }


        // CONSTRUCTORS

        public Node(GridVector position) {
            _position = position;
        }


        // METHODS

        public Edge GetEdge(Node otherNode) {
            return _edges[otherNode];
        }

        // IBoundable

        public bool Raycast(Ray ray) {
            float distance;
            return Sphere.RaycastSphere(Position, ShipEditor.NODE_RADIUS, ray, out distance);
        }

        public bool Raycast(Ray ray, out float distance) {
            return Sphere.RaycastSphere(Position, ShipEditor.NODE_RADIUS, ray, out distance);
        }

        // ISelectable

        public void MakeSelector(Transform parent) {
            _collider = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _collider.name = "selectedNode";

            var nodeRenderer = _collider.GetComponent<MeshRenderer>();
            nodeRenderer.material.shader = Shader.Find("Unlit/Color");
            nodeRenderer.material.color = ColorPallette.gridSelected;

            _collider.transform.parent = parent;
            _collider.transform.localPosition = Position;
            _collider.transform.localScale = Vector3.one * 0.25f;
        }

        public void DestroySelector() {
            GameObject.Destroy(_collider);
            _collider = null;
        }
    }
}