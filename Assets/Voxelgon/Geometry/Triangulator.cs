using System.Collections.Generic;
using UnityEngine;
using Voxelgon.Collections;

namespace Voxelgon.Geometry {
    public class Triangulator {
        #region Fields

        private readonly IList<Vector2> _verts;
        private readonly IList<int> _indices;
        private readonly VertexData[] _links;

        #endregion

        #region Constructors

        public Triangulator(IList<Vector2> verts) : this(verts, new SequencialList(verts.Count)) {
        }

        public Triangulator(IList<Vector2> verts, IList<int> indices) {
            _verts = verts;
            _indices = indices;
            _links = new VertexData[_indices.Count];
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Triangulates the polygon data using ear clipping
        /// </summary>
        /// <returns> Enumerable of vertex indices </returns>
        public IEnumerable<int> Triangulate() {
            var vertexCount = _indices.Count;
            var reflexCount = 0;

            // write index data to the indices array and their neighbor links
            for (var i = 0; i < vertexCount; i++) {
                _links[i].Next = (ushort) ((i + 1) % vertexCount);
                _links[i].Prev = (ushort) ((i - 1 + vertexCount) % vertexCount);
            }

            // decide if each vertex is convex or reflex
            for (var i = 0; i < vertexCount; i++) {
                if (IsConvex(i)) {
                    _links[i].Reflex = false;
                } else {
                    _links[i].Reflex = true;
                    reflexCount++;
                }
            }

            var vert = 0;

            var safety = 10 * vertexCount;
            // loop over polygon pruning off ears
            while (reflexCount > 0 && vertexCount > 2 && safety >= 0) {
                var next = _links[vert].Next;
                var prev = _links[vert].Prev;


                // check if we are currently at an ear of the polygon
                if (IsEar(vert, reflexCount)) {
                    // close the links in the list
                    _links[next].Prev = prev;
                    _links[prev].Next = next;

                    // return indices
                    yield return _indices[vert];
                    yield return _indices[next];
                    yield return _indices[prev];
                    vertexCount--;

                    // neighbors may have stopped being reflex,
                    // so update them and move the current index

                    if (_links[prev].Reflex && IsConvex(prev)) {
                        _links[prev].Reflex = false;
                        reflexCount--;
                    }

                    if (_links[next].Reflex && IsConvex(next)) {
                        _links[next].Reflex = false;
                        reflexCount--;
                    }

                    vert = prev;
                } else {
                    // move on to the next vertex
                    vert = next;
                }
                safety--;
            }

            if (safety <= 0) Debug.LogError("triangulation timed out");

            // The polygon is now convex, so we can triangulate it
            // using a triangle strip over the remaining vertices
            if (vertexCount > 2 && safety > 0) {
                bool winding = true;
                var left = vert;
                var right = _links[vert].Prev;

                while (_links[left].Next != right) {
                    if (winding) {
                        yield return _indices[right];
                        yield return _indices[left];
                        left = _links[left].Next;
                        yield return _indices[left];
                    } else {
                        yield return _indices[right];
                        yield return _indices[left];
                        right = _links[right].Prev;
                        yield return _indices[right];
                    }
                    winding = !winding;
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// determines if the given vertex is an ear or not
        /// </summary>
        /// <param name="vert"> vertex to check </param>
        /// <param name="reflexCount"> number of reflex vertices in the polygon </param>
        /// <returns> true if the vertex is an ear, false otherwise </returns>
        private bool IsEar(int vert, int reflexCount) {
            var next = _links[vert].Next;
            var prev = _links[vert].Prev;

            // ears must be convex vertices, and
            // any concave polygon has at least one ear adjacent to a reflex vertex
            if (_links[vert].Reflex || (!_links[next].Reflex && !_links[prev].Reflex)) return false;

            // if theres just one reflex vertex it must be a neighbor,
            // and a triangle cant contain itself
            if (reflexCount == 1) return true;

            var last = prev;
            var first = next;

            // -->   -->   --> last  vert first -->
            //                   V    V   V
            // Verts: xx••x•x••x•x•xxxO•••xxx•••••xx
            // x = reflex • = convex

            // we dont have to check any chain of reflex vertices that include a neighbor
            do {
                last = _links[last].Prev;
            } while (last != first && !_links[last].Reflex && _links[_links[last].Next].Reflex);

            do {
                first = _links[first].Next;
            } while (first != last && !_links[first].Reflex && _links[_links[first].Prev].Reflex);

            // now check each reflex vert between first and last for inclusion in the ear

            var v0 = _indices[vert];
            var v1 = _indices[_links[vert].Next];
            var v2 = _indices[_links[vert].Prev];

            while (last != first) {
                if (_links[first].Reflex) {
                    var contains = GeometryVG.TriangleContains2D(
                        _verts[v0],
                        _verts[v1],
                        _verts[v2],
                        _verts[_indices[first]]);
                    if (contains) return false;
                }

                first = _links[vert].Next;
            }

            return true;
        }

        /// <summary>
        /// determines if the given vertex is convex or reflex
        /// </summary>
        /// <param name="vert"> vertex to check </param>
        /// <returns> true if the vertex is convex, false if the vertex is reflex </returns>
        private bool IsConvex(int vert) {
            var v0 = _indices[vert];
            var v1 = _indices[_links[vert].Next];
            var v2 = _indices[_links[vert].Prev];
            return (GeometryVG.TriangleWindingOrder2D(_verts[v0], _verts[v1], _verts[v2]) != -1);
        }

        #endregion

        #region Structs

        /// <summary>
        /// Internal data for polygon triangulation
        /// </summary>
        private struct VertexData {
            public ushort Next; // next index in indices array
            public ushort Prev; // prev index
            public bool Reflex; // is this vertex reflex or convex?
        }

        #endregion
    }
}