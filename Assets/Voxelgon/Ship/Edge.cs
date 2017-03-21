using System;
using System.Collections.Generic;
using UnityEngine;
using Voxelgon.Collections;
using Voxelgon.Util.Geometry;
using Voxelgon.Ship.Editor;

namespace Voxelgon.Ship {

    public class Edge : ISelectable {

        // FIELDS

        private Hull _hull;
        private Node _node1;
        private Node _node2;
        private SortedLoopList<Wall> _walls;

        private GameObject _collider;


        // PROPERTIES

        public GridSegment Segment {
            get {
                return new GridSegment(_node1.GridPosition,
                                       _node2.GridPosition);
            }
        }

        public Vector3 Tangent {
            get { return Segment.Tangent; }
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

        public GameObject Collider {
            get { return _collider; }
        }


        // CONSTRUCTORS

        public Edge(Node node1, Node node2, Wall wall) {
            _node1 = node1;
            _node2 = node2;
            _walls = new SortedLoopList<Wall>(new WallComparer(this));
            _walls.Add(wall);
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

        public void MakeCollider(Transform parent) {
            //TODO
            throw new NotImplementedException();
        }

        public void DestroyCollider() {
            //TODO
            throw new NotImplementedException();
        }


        // CLASSES

        private struct WallComparer : IComparer<Wall> {

            // FIELDS

            private Edge _edge;


            // CONSTRUCTORS

            public WallComparer(Edge edge) {
                _edge = edge;
            }


            // METHODS

            public int Compare(Wall wall1, Wall wall2) {
                var tangent1 = wall1.GetEdgeNormal(_edge);
                var tangent2 = wall2.GetEdgeNormal(_edge);
                var cross = Vector3.Cross(tangent1, tangent2);
                var dot = Vector3.Dot(cross, _edge.Segment.Tangent);
                return (dot > 0) ? 1 : -1;
            }
        }
    }
}