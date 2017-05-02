using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using Voxelgon.Geometry;

namespace Voxelgon.Util {

    // An interface for any object that can be represented by an AABB
    public interface IBoundable {
        Bounds Bounds { get; }
        bool Raycast(Ray ray);
        bool Raycast(Ray ray, out float distance);
    }

    // A Bounded Volume Heirarchy (BVH) restricted to a 3D grid
    // Useful for quickly raytracing or intersecting a collection of objects that are on a grid

    // Loosely derived from here: https://github.com/jeske/SimpleScene/tree/master/SimpleScene/Util/ssBVH
    public class BVH<T> where T : IBoundable {

        // FIELDS

        private BVHNode _root; // root node of the tree

        private Dictionary<T, BVHNode> _leafMap; // map of where each object is, used for deletion

        private int _nodeCounter = 0; // counter used to give node's a unique ID (for debugging)
        private int _maxDepth = 0; // maximum depth of the tree


        // CONSTRUCTORS

        // new empty BVH
        public BVH() {
            _leafMap = new Dictionary<T, BVHNode>();
            _root = new BVHNode(this);
        }

        // new populated BVH
        public BVH(List<T> contents) {
            _leafMap = new Dictionary<T, BVHNode>();
            _root = new BVHNode(this, null, contents, 0);
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

        public bool Contains(T obj) {
            return _leafMap.ContainsKey(obj);
        }

        // returns a list of all items that match `itemQuery`, using `boundsQuery` to narrow its results
        public List<T> Traverse(Func<Bounds, Boolean> boundsQuery, Func<T, Boolean> itemQuery) {
            var hitList = new List<T>();

            _root.Traverse(hitList, boundsQuery, itemQuery);

            return hitList;
        }

        // returns a list of all items of type U that match `itemQuery`, using `boundsQuery` to narrow its results
        public List<U> Traverse<U>(Func<Bounds, Boolean> boundsQuery, Func<U, Boolean> itemQuery) where U : T {
            var hitList = new List<U>();

            _root.Traverse<U>(hitList, boundsQuery, itemQuery);

            return hitList;
        }

        // returns a list of all items whos bounds match `boundsQuery`
        public List<T> Traverse(Func<Bounds, Boolean> boundsQuery) {
            return Traverse(boundsQuery, i => boundsQuery(i.Bounds));
        }

        // returns a list of all items of type U whos bounds match `boundsQuery`
        public List<U> Traverse<U>(Func<Bounds, Boolean> boundsQuery) where U : T{
            return Traverse<U>(boundsQuery, i => boundsQuery(i.Bounds));
        }

        // returns a list of all items that are intersected by `ray`
        public List<T> Traverse(Ray ray) {
            return Traverse(b => b.IntersectRay(ray), i => i.Raycast(ray));
        }

        // returns a list of all items that are intersected by `ray`
        public List<U> Traverse<U>(Ray ray) where U : T{
            return Traverse<U>(b => b.IntersectRay(ray), i => i.Raycast(ray));
        }

        // draw the BVH's bounds recursively
        public void DrawDebug(float duration) {
            _root.DrawDebug(duration);
        }

        // returns a string representation of this BVH
        public override string ToString() {
            return "GridBVH<" + typeof(T) + ">";
        }

        // PRIVATE METHODS

        private static bool BoundsApprox(Bounds bounds1, Bounds bounds2) {
            return (bounds1.center.Approximately(bounds2.center)
                 && bounds1.extents.Approximately(bounds2.extents));
        }

        // CLASSES

        private class BVHNode {

            // FIELDS
            private const int MAX_LEAF_SIZE = 4; // maximum number of objects in a leaf before we try to split
            private Color[] DEBUG_COLORS = { // colors used for debugging based on depth
                Color.red,
                Color.blue,
                Color.green,
                Color.yellow,
                Color.cyan};
            private Color DEBUG_OBJECT_COLOR = Color.magenta;


            // Parent and Children nodes
            private BVHNode _parent;
            private BVHNode _left;
            private BVHNode _right;

            private Bounds _bounds; // Bounding box for this node
            private List<T> _contents;  // Items in this node, null if it is not a leaf node

            private readonly BVH<T> _bvh; // BVH this node belongs to

            private int _depth;
            [SerializeField]
            private readonly int _id;


            // CONSTRUCTORS

            // new root node
            public BVHNode(BVH<T> bvh) {
                _contents = new List<T>();
                _parent = null;
                _left = null;
                _right = null;
                _bvh = bvh;
                _id = bvh._nodeCounter++;
            }

            // new interior node
            public BVHNode(BVH<T> bvh, BVHNode parent, BVHNode left, BVHNode right, int depth) {
                _contents = null;
                _parent = parent;
                _left = left;
                _right = right;
                _bvh = bvh;
                _id = bvh._nodeCounter++;
                _bounds = GeometryVG.CalcBounds(left._bounds, right._bounds);

                SetDepth(depth);
            }

            // new leaf
            public BVHNode(BVH<T> bvh, BVHNode parent, List<T> contents, int depth) {
                if (contents.Count == 0) throw new ArgumentOutOfRangeException("contents", "contents list is empty");

                _contents = contents;
                _parent = parent;
                _left = null;
                _right = null;
                _bvh = bvh;
                _id = bvh._nodeCounter++;
                _bounds = GeometryVG.CalcBounds(contents.Select(b => b.Bounds));

                _contents.ForEach(o => _bvh._leafMap.Add(o, this));

                SetDepth(depth);

                if (_contents.Count > MAX_LEAF_SIZE) {
                    Split();
                }
            }

            // new leaf
            public BVHNode(BVH<T> bvh, BVHNode parent, List<T> contents, Bounds bounds, int depth) {
                if (contents.Count == 0) throw new ArgumentOutOfRangeException("contents", "contents list is empty");

                _contents = contents;
                _parent = parent;
                _left = null;
                _right = null;
                _bvh = bvh;
                _id = bvh._nodeCounter++;
                _bounds = bounds;

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

            // traverses the tree looking for items that match `itemQuery`, 
            // using `boundsQuery` to narrow its results
            public void Traverse(List<T> hitList, Func<Bounds, Boolean> boundsQuery, Func<T, Boolean> itemQuery) {
                if (IsLeaf) {
                    hitList.AddRange( _contents.Where(itemQuery));
                } else {
                    // propogate to any child nodes that also match
                    if (boundsQuery(_left._bounds)) _left.Traverse(hitList, boundsQuery, itemQuery);
                    if (boundsQuery(_right._bounds)) _right.Traverse(hitList, boundsQuery, itemQuery);
                }
            }

            // traverses the tree looking for items of type U that match `itemQuery`, 
            // using `boundsQuery` to narrow its results
            public void Traverse<U>(List<U> hitList, Func<Bounds, Boolean> boundsQuery, Func<U, Boolean> itemQuery) where U : T{
                if (IsLeaf) {
                    hitList.AddRange( _contents.OfType<U>()
                                               .Where(itemQuery));
                } else {
                    // propogate to any child nodes that also match
                    if (boundsQuery(_left._bounds)) _left.Traverse<U>(hitList, boundsQuery, itemQuery);
                    if (boundsQuery(_right._bounds)) _right.Traverse<U>(hitList, boundsQuery, itemQuery);
                }
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

                    var leftSAH = leftBounds.SurfaceArea();
                    var rightSAH = rightBounds.SurfaceArea();

                    var sendLeftSAH = rightSAH + GeometryVG.CalcBounds(leftBounds, objBounds).SurfaceArea();   // (L+N,R)
                    var sendRightSAH = leftSAH + GeometryVG.CalcBounds(rightBounds, objBounds).SurfaceArea();  // (L,R+N)
                    var mergeSAH = objBounds.SurfaceArea()
                                     + GeometryVG.CalcBounds(leftBounds, rightBounds).SurfaceArea();           // (L+R,N)

                    // we are adding the new object to this node or a child, so expand bounds to fit the object
                    _bounds = GeometryVG.CalcBounds(_bounds, objBounds);

                    if (mergeSAH < System.Math.Min(sendLeftSAH, sendRightSAH)) {
                        // move children to new node under this one, then add a new leaf under this one
                        /*      n     *
                         *     / \    *
                         *    n   l   *
                         *   / \      *
                         *  l   l     */

                        _left = new BVHNode(_bvh, this, _left, _right, _depth + 1);
                        _right = new BVHNode(_bvh, this, new List<T> { newObject }, _depth + 1);

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
                    _bounds = GeometryVG.CalcBounds(_bounds, newObject.Bounds);
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
                        _bounds = new Bounds();
                    } else {
                        _parent.RemoveLeaf(this);
                    }
                }

                return true;
            }

            // draw the node's bounds in world and propogate downwards
            public void DrawDebug(float duration) {
                _bounds.DrawDebug(DEBUG_COLORS[_depth % DEBUG_COLORS.Length], duration);
                if (IsLeaf) {
                    _contents.ForEach(o => o.Bounds.DrawDebug(DEBUG_OBJECT_COLOR, duration));
                } else {
                    _left.DrawDebug(duration);
                    _right.DrawDebug(duration);
                }
            }

            // PRIVATE METHODS

            // split this node into two halves
            private bool Split() {
                if (!IsLeaf)
                    throw new Exception("Tried to split an internal node!");

                int axis = 0;
                // Choose the longest axis to split on and sort the list
                if (_bounds.size.x >= _bounds.size.y && _bounds.size.x >= _bounds.size.z) {
                    // x biggest
                    axis = 0;
                } else if (_bounds.size.y >= _bounds.size.z) {
                    // y biggest
                    axis = 1;
                } else {
                    // z biggest
                    axis = 2;
                }

                _contents.Sort((a, b) => {
                    // distance between bounds on our selected axis
                    float delta = (b.Bounds.min[axis] + b.Bounds.max[axis])
                                - (a.Bounds.min[axis] + a.Bounds.max[axis]);
                    if (Mathf.Approximately(delta, 0)) {
                        // if the centers are the same, sort by Volume
                        return (b.Bounds.Volume() > a.Bounds.Volume()) ? -1 : 1;
                    }
                    return (delta < 0) ? -1 : 1;
                });

                // split the list of items down the middle
                int center = _contents.Count / 2;
                var leftItems = _contents.GetRange(0, center);
                var rightItems = _contents.GetRange(center, _contents.Count - center);

                var leftBounds = GeometryVG.CalcBounds(leftItems.Select(b => b.Bounds));
                var rightBounds = GeometryVG.CalcBounds(rightItems.Select(b => b.Bounds));

                // if we successfully made the bounds smaller, split
                if (!BoundsApprox(leftBounds, rightBounds)) {
                    _contents.ForEach(o => _bvh._leafMap.Remove(o));
                    _contents = null;

                    _left = new BVHNode(_bvh, this, leftItems, leftBounds, _depth + 1);
                    _right = new BVHNode(_bvh, this, rightItems, rightBounds, _depth + 1);

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
                        if (_left != null) _left.SetDepth(depth + 1);
                        if (_right != null) _right.SetDepth(depth + 1);
                    }
                }
            }

            // recalculate this node's bounds and propogate upwards
            private void RecalculateBounds() {
                Bounds newBounds;
                if (IsLeaf) { // combination of all contents
                    newBounds = GeometryVG.CalcBounds(_contents.Select(b => b.Bounds));
                } else {      // combination of children nodes
                    newBounds = GeometryVG.CalcBounds(_left._bounds, _right._bounds);
                }

                // if the new bounds are different, propogate upwards
                if (_bounds != newBounds) {
                    _bounds = newBounds;
                    _parent.RecalculateBounds();
                }
            }

            // removes a leaf
            private void RemoveLeaf(BVHNode leaf) {
                if (IsLeaf) throw new InvalidOperationException("Attempt to remove a child of a leaf!");

                leaf._parent = null;

                BVHNode keepNode;
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