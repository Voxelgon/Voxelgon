using System.Collections.Generic;
using UnityEngine;
using Voxelgon.Collections;
using Voxelgon.Geometry;

namespace Voxelgon.Geometry2D {
    /// <summary>
    /// Class for triangulating planar polygons
    /// </summary>
    public class Triangulation {
        #region Public Methods

        /// <summary>
        /// Triangulates the polygon data using ear clipping
        /// for a sequencial polygon
        /// </summary>
        /// <param name="vertices">vertex list</param>
        /// <returns> Enumerable of vertex indices </returns>
        public static IEnumerable<int> Triangulate(IList<Vector2> vertices) {
            return Triangulate(vertices, new SequencialList(vertices.Count));
        }

        /// <summary>
        /// Triangulates the polygon data using ear clipping
        /// </summary>
        /// <param name="vertices">vertex list</param>
        /// <param name="indices">index list</param>
        /// <returns> Enumerable of vertex indices </returns>
        public static IEnumerable<int> Triangulate(IList<Vector2> vertices, IList<int> indices) {
            var vertexCount = indices.Count;
            var links = new VertexData[vertexCount];
            var reflexCount = 0;

            // write index data to the indices array and their neighbor links
            for (var i = 0; i < vertexCount; i++) {
                links[i].Next = (ushort) ((i + 1) % vertexCount);
                links[i].Prev = (ushort) ((i - 1 + vertexCount) % vertexCount);
            }

            // decide if each vertex is convex or reflex
            for (var i = 0; i < vertexCount; i++) {
                if (IsConvex(i, vertices, indices, links)) {
                    links[i].Reflex = false;
                } else {
                    links[i].Reflex = true;
                    reflexCount++;
                }
            }

            var vert = 0;

            var safety = 10 * vertexCount;

            // loop over polygon pruning off ears
            while (reflexCount > 0 && vertexCount > 2 && safety >= 0) {
                var next = links[vert].Next;
                var prev = links[vert].Prev;

                var i0 = indices[prev];
                var i1 = indices[vert];
                var i2 = indices[next];

                var isEar = false;

                // check if the current vertex is a good ear
                if (!links[vert].Reflex && (links[next].Reflex || links[prev].Reflex)) {
                    isEar = true;
                    if (reflexCount == 1) break;

                    // -->   -->   --> last  vert first -->
                    //                   V    V   V
                    // Verts: xx••x•x••x•x•xxxO•••xxx•••••xx
                    // x = reflex • = convex

                    var last = prev;
                    var first = next;

                    // we dont have to check any chain of reflex vertices that include a neighbor
                    do {
                        last = links[last].Prev;
                    } while (last != first && !links[last].Reflex && links[links[last].Next].Reflex);

                    do {
                        first = links[first].Next;
                    } while (first != last && !links[first].Reflex && links[links[first].Prev].Reflex);

                    // now check each reflex vert between first and last for inclusion in the ear

                    for (; last != first; first = links[first].Next) {
                        if (!links[first].Reflex) continue;

                        // dont test the point if it is at the same location as one in the ear
                        // otherwise we might get false negatives
                        var test = indices[first];
                        if (test == i0 || test == i1 || test == i2) continue;

                        if (GeoUtil2D.TriangleContains(vertices[i0], vertices[i1], vertices[i2], vertices[test])) {
                            isEar = false;
                            break;
                        }
                    }
                }


                // if this is an ear, prune and return it,
                // otherwise move on to the next vertex
                if (isEar) {
                    // close the links in the list
                    links[next].Prev = prev;
                    links[prev].Next = next;


                    // return indices
                    yield return i0;
                    yield return i1;
                    yield return i2;
                    vertexCount--;

                    // neighbors may have stopped being reflex,
                    // so update them and move the current index
                    if (links[prev].Reflex && IsConvex(prev, vertices, indices, links)) {
                        links[prev].Reflex = false;
                        reflexCount--;
                    }

                    if (links[next].Reflex && IsConvex(next, vertices, indices, links)) {
                        links[next].Reflex = false;
                        reflexCount--;
                    }

                    vert = prev;
                } else {
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
                var right = links[vert].Prev;

                while (links[left].Next != right) {
                    if (winding) {
                        yield return indices[right];
                        yield return indices[left];
                        left = links[left].Next;
                        yield return indices[left];
                    } else {
                        yield return indices[right];
                        yield return indices[left];
                        right = links[right].Prev;
                        yield return indices[right];
                    }
                    winding = !winding;
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// determines if the given vertex is convex or reflex
        /// </summary>
        /// <param name="vert"> vertex to check </param>
        /// <param name="vertices">vertex list</param>
        /// <param name="indices">index list</param>
        /// <param name="links">links list</param>
        /// <returns> true if the vertex is convex, false if the vertex is reflex </returns>
        private static bool IsConvex(int vert, IList<Vector2> vertices, IList<int> indices, IList<VertexData> links) {
            var v0 = indices[vert];
            var v1 = indices[links[vert].Next];

            var v2 = indices[links[vert].Prev];
            return (GeoUtil2D.WindingOrder(vertices[v0], vertices[v1], vertices[v2]) != -1);
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