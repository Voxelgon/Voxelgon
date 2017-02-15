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
            _vertices = vertices;
            _tangents = tangents;
            _scales = scales;
        }


        // PROPERTIES


        //the number of vertices in the path
        public virtual int VertexCount {
            get { return _vertices.Length; }
        }

        //the path's vertices as a new list
        public virtual List<Vector3> Vertices {
            get { return new List<Vector3>(_vertices); }
        }

        //the path's tangents as a new list
        public virtual List<Vector3> Tangents{
            get { return new List<Vector3>(_tangents); }
        }

        //the path's scale values as a new list
        public virtual List<float> Scales{
            get { return new List<float>(_scales); }
        }

        // METHODS

        //reverses the path
        public virtual Path Reverse() {
            var vertices = (Vector3[])_vertices.Clone();
            var tangents = (Vector3[])_tangents.Clone();
            var scales = (float[])_scales.Clone();
            Array.Reverse(vertices);
            Array.Reverse(tangents);
            Array.Reverse(scales);
            return new Path(vertices, tangents, scales);
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