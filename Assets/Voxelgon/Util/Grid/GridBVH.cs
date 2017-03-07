using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using Voxelgon.Geometry;

namespace Voxelgon.Util.Grid {


    // Loosely derived from here: https://github.com/jeske/SimpleScene/tree/master/SimpleScene/Util/ssBVH
    public class GridBVH<T> where T : IGridObject {

        // FIELDS

        private const int MAX_LEAF_SIZE = 8;

        private GridBVHNode _root; // root node of the tree

        private Dictionary<T, GridBVHNode> _leafMap; // map of where each object is, used for deletion

        private int _nodeCounter = 0; // counter used to give node's a unique ID (for debugging)
        private int _maxDepth = 0; // maximum depth of the tree


        // CONSTRUCTORS

        public GridBVH() {
            _root = new GridBVHNode(this);
        }


        // METHODS


        public override string ToString() {
            return "GridBVH<" + typeof(T) + ">";
        }


        // CLASSES

        private class GridBVHNode {

            // FIELDS

            // Bounding box for this node
            public GridBounds bounds;

            // Parent and Children nodes
            public GridBVHNode parent;
            public GridBVHNode left;
            public GridBVHNode right;

            // Items in this node, null if it is not a leaf node
            public List<T> items;

            private readonly GridBVH<T> _bvh;

            public int depth;
            public int id;


            // CONSTRUCTORS

            public GridBVHNode(GridBVH<T> bvh) {
                items = new List<T>();
                parent = null;
                right = null;
                right = null;
                _bvh = bvh;
                id = bvh._nodeCounter++;
            }

            private GridBVHNode(GridBVH<T> bvh, GridBVHNode parent, List<T> items, int depth) {
                this.parent = parent;
                this.items = items;
                this.depth = depth;
                _bvh = bvh;
                id = bvh._nodeCounter++;

                SetDepth(depth);
            }


            // PROPERTIES 

            public bool IsRoot {
                get { return parent == null; }
            }

            public bool IsLeaf {
                get { return items != null; }
            }

            // METHODS

            public override string ToString() {
                return "GridBVHNode<" + typeof(T) + ">:" + id;
            }

            // adds a new object 
            private void AddObject(T newObject) {
                // 1. first we traverse the tree looking for the best Node
                if (!IsLeaf) {
                    // find the best way to add this object.. 3 options..
                    // 1. send to left node  (L+N,R)
                    // 2. send to right node (L,R+N)
                    // 3. merge and pushdown left-and-right node (L+R,N)

                    var leftBounds = left.bounds;
                    var rightBounds = right.bounds;
                    var objBounds = newObject.Bounds;

                    int leftSAH = leftBounds.SurfaceArea;
                    int rightSAH = rightBounds.SurfaceArea;

                    int sendLeftSAH = rightSAH + GridBounds.Combine(leftBounds, objBounds).SurfaceArea;       // (L+N,R)
                    int sendRightSAH = leftSAH + GridBounds.Combine(rightBounds, objBounds).SurfaceArea;      // (L,R+N)
                    int mergeSAH = objBounds.SurfaceArea + GridBounds.Combine(leftBounds, rightBounds).SurfaceArea; // (L+R,N)

                    // we are adding the new object to this node or a child, so expand bounds to fit the object
                    bounds = GridBounds.Combine(bounds, objBounds);

                    // Doing a merge-and-pushdown can be expensive, so we only do it if it's notably better
                    const int MERGE_PRICE = 3;

                    if (MERGE_PRICE * mergeSAH < Math.Min(sendLeftSAH, sendRightSAH)) {
                        AddObjectAndPushDown(newObject);
                        return;
                    } else {
                        if (sendLeftSAH < sendRightSAH) {
                            left.AddObject(newObject);
                            return;
                        } else {
                            right.AddObject(newObject);
                            return;
                        }
                    }
                }

                // 2. then we add the object and map it to our leaf
                items.Add(newObject);
                _bvh._leafMap.Add(newObject, this);
                if (items.Count > MAX_LEAF_SIZE) {
                    AttemptSplit();
                }
            }

            private void AddObjectAndPushDown(T newObject) {

            }


            private void SetDepth(int depth) {
                this.depth = depth;

                if (IsLeaf) {
                    if (depth > _bvh._maxDepth) {
                        _bvh._maxDepth = depth;
                    } else {
                        left.SetDepth(depth + 1);
                        right.SetDepth(depth + 1);
                    }
                }
            }

            private bool AttemptSplit() {
                if (!IsLeaf) throw new Exception("Tried to split an internal node!");

                var xSize = bounds.xSize;
                var ySize = bounds.ySize;
                var zSize = bounds.zSize;

                List<T> leftItems = new List<T>();
                List<T> rightItems = new List<T>();

                Vector3 center = Geometry.Geometry.VectorAvg(
                    (List<Vector3>)items.Select(e => e.Bounds.Center)
                );

                if (xSize >= ySize && xSize >= zSize) {
                    // x biggest
                    items.ForEach(e => {
                        if (e.Bounds.Center.x < center.x) leftItems.Add(e);
                        else rightItems.Add(e);
                    });
                } else if (ySize >= zSize) {
                    // y biggest
                    items.ForEach(e => {
                        if (e.Bounds.Center.y < center.y) leftItems.Add(e);
                        else rightItems.Add(e);
                    });
                } else {
                    // z biggest
                    items.ForEach(e => {
                        if (e.Bounds.Center.y < center.y) leftItems.Add(e);
                        else rightItems.Add(e);
                    });
                }

                if (leftItems.Count == 0 || rightItems.Count == 0) return false; // nothing to split!

                items = null;
                left = new GridBVHNode(_bvh, this, leftItems, depth + 1);
                right = new GridBVHNode(_bvh, this, rightItems, depth + 1);

                return true;
            }
        }
    }
}