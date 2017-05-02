using System;
using System.Collections.Generic;
using UnityEngine;
using Voxelgon.Collections;
using Voxelgon.Geometry;
using Voxelgon.Ship.Editor;

namespace Voxelgon.Ship {

    public class Edge : ISelectable {

        // FIELDS

        private Hull _hull;
        private Node _node1;
        private Node _node2;
        private SortedLoopList<Panel> _panels;

        private GameObject _collider;


        // PROPERTIES

        public GridSegment Segment {
            get {
                return new GridSegment(_node1.GridPosition,
                                       _node2.GridPosition);
            }
        }

        public Vector3 Tangent {
            get { return _node2.GridPosition - _node1.GridPosition; }
        }

        public Node Node1 {
            get { return _node1; }
        }

        public Node Node2 {
            get { return _node2; }
        }

        public Hull Hull {
            get { return _hull; }
        }

        public Bounds Bounds {
            get {
                var bounds = Node1.Position.CalcBounds(Node2.Position);
                bounds.Expand(ShipEditor.EDGE_RADIUS);
                return bounds;
            }
        }

        public GameObject Selector {
            get { return _collider; }
        }


        // CONSTRUCTORS

        public Edge(Node node1, Node node2, Panel panel) {
            _node1 = node1;
            _node2 = node2;
            _panels = new SortedLoopList<Panel>(new PanelComparer(this));
            _panels.Add(panel);
        }


        // METHODS

        // IBoundable

        public bool Raycast(Ray ray) {
            float distance;
            return Raycast(ray, out distance);
        }

        public bool Raycast(Ray ray, out float distance) {
            return Cylinder.RaycastCylinder(Node1.Position, Node2.Position,
                                            ShipEditor.EDGE_RADIUS, ray, out distance);
        }

        // ISelectable

        public void MakeSelector(Transform parent) {
            //TODO
            throw new NotImplementedException();
        }

        public void DestroySelector() {
            //TODO
            throw new NotImplementedException();
        }


        // CLASSES

        private struct PanelComparer : IComparer<Panel> {

            // FIELDS

            private Edge _edge;


            // CONSTRUCTORS

            public PanelComparer(Edge edge) {
                _edge = edge;
            }


            // METHODS

            public int Compare(Panel panel1, Panel panel2) {
                var tangent1 = panel1.GetEdgeNormal(_edge);
                var tangent2 = panel2.GetEdgeNormal(_edge);
                var cross = Vector3.Cross(tangent1, tangent2);
                var dot = Vector3.Dot(cross, _edge.Tangent);
                return (dot > 0) ? 1 : -1;
            }
        }
    }
}