using UnityEngine;
using System;
using System.Collections.Generic;


namespace Voxelgon.Geometry {

    public class Path {

        // FIELDS

        protected readonly Vector3[] _vertices;
        protected readonly Vector3[] _tangents;
        protected readonly float[] _scales;


        // CONSTRUCTORS

        public Path(Vector3[] vertices, Vector3[] tangents, float[] scales) {
            _vertices = (Vector3[])vertices.Clone();
            _tangents = (Vector3[])tangents.Clone();
            _scales = (float[])scales.Clone();
        }

        private Path(int size) {
            _vertices = new Vector3[size];
            _tangents = new Vector3[size];
            _scales = new float[size];
        }


        // PROPERTIES


        //the number of vertices in the path
        public int VertexCount {
            get { return _vertices.Length; }
        }

        //the path's vertices as a new list
        public Vector3[] Vertices {
            get { return (Vector3[])_vertices.Clone(); }
        }

        //the path's tangents as a new list
        public Vector3[] Tangents{
            get { return (Vector3[])_tangents.Clone(); }
        }

        //the path's scale values as a new list
        public float[] Scales{
            get { return (float[])_scales.Clone(); }
        }

        // METHODS

        //reverses the path
        public virtual Path Reverse() {
            var size = VertexCount;

            var reversed = new Path(size);

            for (var i = 0; i < size; i++) {
                var j = size - i - 1;
                reversed._vertices[i] = _vertices[j];
                reversed._tangents[i] = _tangents[j];
                reversed._scales[i] = _scales[j];
            }

            return reversed;
        }

        //returns the vertex at index `index`
        public Vector3 GetVertex(int index) {
            return _vertices[index];
        }

        public Vector3 GetTangent(int index) {
            return _tangents[index];
        }

        //returns the normal of the plane the point and its neighbors lie on
        public Vector3 GetBinormal(int index) {
            if (index <= 0 || index >= VertexCount - 1) {
                return Vector3.zero;
            }

            var edge1 = _vertices[index + 1] - _vertices[index];
            var edge2 = _vertices[index - 1] - _vertices[index];

            return Vector3.Cross(edge1, edge2).normalized;
        }

        //returns the normal of a vector, pointing away from its neighbors
        public Vector3 GetNormal(int index) {
            if (index <= 0 || index >= VertexCount - 1) {
                return Vector3.zero;
            }

            var edge1 = _vertices[index + 1] - _vertices[index];
            var edge2 = _vertices[index - 1] - _vertices[index];
            var binormal = Vector3.Cross(edge1, edge2);

            return Vector3.Cross(GetTangent(index), binormal).normalized;
        }

        //returns the edge vector at index `index`
        //normalized vector from a vertex to the following vertex
        public Vector3 GetEdge(int index) {
            if (index < 0 || index >= VertexCount - 1) {
                return Vector3.zero;
            }

            var vertex1 = _vertices[index];
            var vertex2 = _vertices[index + 1];

            return (vertex2 - vertex1).normalized;
        }

        //returns the vertex at index `index`
        public float GetScale(int index) {
            return _scales[index];
        }


        //returns a clone of this Path
        public Path Clone() {
            return new Path(_vertices, _tangents, _scales);
        }

        // returns a clone of this Path offset by a given vector
        public Path Translate(Vector3 translationVector) {
            var newVertices = new Vector3[_vertices.Length];
            for (int i = 0; i < newVertices.Length; i++) {
                newVertices[i] = _vertices[i] + translationVector;
            }

            return new Path(newVertices, _tangents, _scales);
        }

        //draw the path in the world for a frame
        public void Draw() {
            for (int i = 0; i < VertexCount - 1; i++) {
                Debug.DrawLine(_vertices[i], _vertices[i + 1]);
            }
        }

        //are the paths equal?
        public bool Equals(Path p) {
            if (VertexCount != p.VertexCount) { return false; }
            
            for (int i = 0; i < VertexCount; i++) {
                if (!GetVertex(i).Equals(p.GetVertex(i))
                || !GetTangent(i).Equals(p.GetTangent(i))
                || !Mathf.Approximately(GetScale(i), p.GetScale(i))) {
                    return false;
                }
            }

            return true;
        }
    }
}