using System;
using System.Collections.Generic;

namespace Voxelgon.Util.Grid {


    // Loosely derived from here: https://github.com/jeske/SimpleScene/tree/master/SimpleScene/Util/ssBVH
    public class GridBVH<T> where T : IGridObject {

        // FIELDS

        private const int MAX_LEAF_SIZE = 8; // maximum number of objects in a leaf before we try to split

        private GridBVHNode _root; // root node of the tree

        private Dictionary<T, GridBVHNode> _leafMap; // map of where each object is, used for deletion

        private int _nodeCounter = 0; // counter used to give node's a unique ID (for debugging)
        private int _maxDepth = 0; // maximum depth of the tree


        // CONSTRUCTORS

        // new empty BVH
        public GridBVH() {
            _root = new GridBVHNode(this);
        }

        // new populated BVH
        public GridBVH(List<T> contents) {
            _root = new GridBVHNode(this, null, contents, 0);
        }


        // METHODS

        // adds an object to the BVH
        public void Add(T newObject) {
            _root.Add(newObject);
        }

        // removes an object from the BVH
        public bool Remove(T remObject) {
            return _leafMap[remObject].Remove(remObject);
        }

        // returns a string representation of this BVH
        public override string ToString() {
            return "GridBVH<" + typeof(T) + ">";
        }


        // CLASSES

        private class GridBVHNode {

            // FIELDS

            // Parent and Children nodes
            private GridBVHNode _parent;
            private GridBVHNode _left;
            private GridBVHNode _right;

            private GridBounds _bounds; // Bounding box for this node
            private List<T> _contents;  // Items in this node, null if it is not a leaf node

            private readonly GridBVH<T> _bvh; // BVH this node belongs to

            private int _depth;
            private readonly int _id;


            // CONSTRUCTORS

            // new root node
            public GridBVHNode(GridBVH<T> bvh) {
                _contents = new List<T>();
                _parent = null;
                _left = null;
                _right = null;
                _bvh = bvh;
                _id = bvh._nodeCounter++;
            }

            // new interior node
            public GridBVHNode(GridBVH<T> bvh, GridBVHNode parent, GridBVHNode left, GridBVHNode right, int depth) {
                _contents = null;
                _parent = parent;
                _left = left;
                _right = right;
                _bvh = bvh;
                _id = bvh._nodeCounter++;
                _bounds = GridBounds.Combine(left._bounds, right._bounds);

                SetDepth(depth);
            }

            // new leaf
            public GridBVHNode(GridBVH<T> bvh, GridBVHNode parent, List<T> contents, int depth) {
                if (contents.Count == 0) throw new ArgumentOutOfRangeException("contents", "contents list is empty");

                _contents = contents;
                _parent = parent;
                _left = null;
                _right = null;
                _bvh = bvh;
                _id = bvh._nodeCounter++;
                _bounds = GridBounds.Combine(contents);

                _contents.ForEach(o => _bvh._leafMap.Add(o, this));

                SetDepth(depth);

                if (_contents.Count > MAX_LEAF_SIZE) {
                    Split();
                }
            }


            // PROPERTIES 

            // is this node the root node?
            public bool IsRoot {
                get { return _parent == null; }
            }

            // is this node a leaf?
            public bool IsLeaf {
                get { return _contents != null; }
            }

            // unique ID of this node (for debugging)
            public int ID {
                get { return _id; }
            }


            // METHODS

            // returns a string representation of this node
            public override string ToString() {
                return "GridBVHNode<" + typeof(T) + ">:" + _id;
            }

            // adds a new object 
            public void Add(T newObject) {
                // 1. first we traverse the tree looking for the best Node
                if (!IsLeaf) {
                    // find the best way to add this object.. 3 options..
                    // 1. send to left node  (L+N,R)
                    // 2. send to right node (L,R+N)
                    // 3. merge and pushdown left-and-right node (L+R,N)

                    var leftBounds = _left._bounds;
                    var rightBounds = _right._bounds;
                    var objBounds = newObject.Bounds;

                    int leftSAH = leftBounds.SurfaceArea;
                    int rightSAH = rightBounds.SurfaceArea;

                    int sendLeftSAH = rightSAH + GridBounds.Combine(leftBounds, objBounds).SurfaceArea;             // (L+N,R)
                    int sendRightSAH = leftSAH + GridBounds.Combine(rightBounds, objBounds).SurfaceArea;            // (L,R+N)
                    int mergeSAH = objBounds.SurfaceArea + GridBounds.Combine(leftBounds, rightBounds).SurfaceArea; // (L+R,N)

                    // we are adding the new object to this node or a child, so expand bounds to fit the object
                    _bounds = GridBounds.Combine(_bounds, objBounds);

                    if (mergeSAH < Math.Min(sendLeftSAH, sendRightSAH)) {
                        // move children to new node under this one, then add a new leaf under this one
                        /*      n     *
                         *     / \    *
                         *    n   l   *
                         *   / \      *
                         *  l   l     */

                        _left = new GridBVHNode(_bvh, this, _left, _right, _depth + 1);
                        _right = new GridBVHNode(_bvh, this, new List<T> { newObject }, _depth + 1);

                        _contents = null;
                        return;
                    } else {
                        if (sendLeftSAH < sendRightSAH) {
                            _left.Add(newObject);
                            return;
                        } else {
                            _right.Add(newObject);
                            return;
                        }
                    }
                }

                // 2. then we add the object and map it to our leaf
                _contents.Add(newObject);
                _bvh._leafMap.Add(newObject, this);

                if (_contents.Count == 1) {
                    _bounds = newObject.Bounds;
                } else {
                    _bounds = GridBounds.Combine(_bounds, newObject.Bounds);
                    if (_contents.Count > MAX_LEAF_SIZE) Split();
                }
            }

            // removes an object 
            public bool Remove(T remObject) {
                if (!IsLeaf) throw new InvalidOperationException("Attempt to remove object from non-leaf!");
                if (!_contents.Remove(remObject)) return false;

                if (_contents.Count > 0) {
                    RecalculateBounds();
                } else {
                    if (IsRoot) {
                        _bounds = new GridBounds();
                    } else {
                        _parent.RemoveLeaf(this);
                    }
                }

                return true;
            }

            // PRIVATE METHODS

            // split this node into two halves
            private bool Split() {
                if (!IsLeaf) throw new Exception("Tried to split an internal node!");

                // Choose the longest axis to split on and sort the list
                if (_bounds.xSize >= _bounds.ySize && _bounds.xSize >= _bounds.zSize) {
                    // x biggest
                    _contents.Sort((a, b) => {
                        // sort by distance between centers on z axis
                        int delta = (b.Bounds.min.x + b.Bounds.max.x) - (a.Bounds.min.x + a.Bounds.max.x);
                        if (delta == 0) return b.Bounds.Volume - a.Bounds.Volume; // if the centers are the same, sort by Volume
                        return delta;
                    });
                } else if (_bounds.ySize >= _bounds.zSize) {
                    // y biggest
                    _contents.Sort((a, b) => {
                        // sort by distance between centers on z axis
                        int delta = (b.Bounds.min.y + b.Bounds.max.y) - (a.Bounds.min.y + a.Bounds.max.y);
                        if (delta == 0) return b.Bounds.Volume - a.Bounds.Volume; // if the centers are the same, sort by Volume
                        return delta;
                    });
                } else {
                    // z biggest
                    _contents.Sort((a, b) => {
                        // sort by distance between centers on z axis
                        int delta = (b.Bounds.min.z + b.Bounds.max.z) - (a.Bounds.min.z + a.Bounds.max.z);
                        if (delta == 0) return b.Bounds.Volume - a.Bounds.Volume; // if the centers are the same, sort by Volume
                        return delta;
                    });
                }

                // split the list of items down the middle
                int center = _contents.Count / 2;
                List<T> leftItems = _contents.GetRange(0, center);
                List<T> rightItems = _contents.GetRange(center, _contents.Count - 1);

                var leftNode = new GridBVHNode(_bvh, this, leftItems, _depth + 1);
                var rightNode = new GridBVHNode(_bvh, this, rightItems, _depth + 1);

                // if we successfully made the bounds smaller, split
                if (leftNode._bounds != rightNode._bounds) {
                    _contents.ForEach(o => _bvh._leafMap.Remove(o));
                    _contents = null;
                    _left = leftNode;
                    _right = rightNode;
                    return true;
                }
                return false;
            }

            // set the depth of this node and propogate downwards
            private void SetDepth(int depth) {
                _depth = depth;

                // propogate downwards until we hit a leaf
                if (IsLeaf) {
                    if (depth > _bvh._maxDepth) {
                        _bvh._maxDepth = depth;
                    } else {
                        _left.SetDepth(depth + 1);
                        _right.SetDepth(depth + 1);
                    }
                }
            }

            // recalculate this node's bounds and propogate upwards
            private void RecalculateBounds() {
                GridBounds newBounds;
                if (IsLeaf) { // combination of all contents
                    newBounds = GridBounds.Combine(_contents);
                } else {      // combination of children nodes
                    newBounds = GridBounds.Combine(_left._bounds, _right._bounds);
                }

                // if the new bounds are different, propogate upwards
                if (_bounds != newBounds) {
                    _bounds = newBounds;
                    _parent.RecalculateBounds();
                }
            }

            // removes a leaf
            private void RemoveLeaf(GridBVHNode leaf) {
                if (IsLeaf) throw new InvalidOperationException("Attempt to remove a child of a leaf!");

                leaf._parent = null;

                GridBVHNode keepNode;
                if (_left == leaf) {
                    keepNode = _right;
                } else if (_right == leaf) {
                    keepNode = _left;
                } else {
                    throw new ArgumentOutOfRangeException("leaf is not present in this node", "leaf");
                }

                _bounds = keepNode._bounds;

                if (keepNode.IsLeaf) {
                    _contents = keepNode._contents;
                    _contents.ForEach(o => _bvh._leafMap[o] = this);
                    _left = null;
                    _right = null;
                } else {
                    _left = keepNode._left;
                    _right = keepNode._right;
                    _left.SetDepth(_depth + 1);
                    _right.SetDepth(_depth + 1);
                }
            }
        }
    }
}