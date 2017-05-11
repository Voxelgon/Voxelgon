using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Voxelgon.Geometry2D {
    public class MultiPolygon2D : IPolygon2D {
        #region Fields

        private readonly List<SimplePolygon2D> _contours;

        #endregion

        #region Constructor

        public MultiPolygon2D(IEnumerable<SimplePolygon2D> contours) {
            //TODO: Check if contours overlap
            _contours = contours.ToList();
        }

        #endregion

        #region Properties

        public bool IsConvex {
            get { return _contours.All(o => o.Clockwise && o.IsConvex); }
        }

        public float Area {
            get { return _contours.Sum(o => o.Area); }
        }

        public int VertexCount {
            get { return _contours.Sum(o => o.VertexCount); }
        }

        public Vector2 Center {
            get { throw new NotImplementedException(); }
        }

        public IList<Vector2> Vertices {
            get { return _contours.SelectMany(o => o.Vertices).ToList(); }
        }

        public IEnumerable<int> Indices {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<SimplePolygon2D> Contours {
            get { return _contours.AsEnumerable(); }
        }

        #endregion

        #region Public Methods

        #region Transformation Methods

        public IPolygon2D Scale(float scaleFactor) {
            return new MultiPolygon2D(
                _contours.Select(o => o.Scale(scaleFactor)));
        }

        public IPolygon2D Scale(float scaleFactor, Vector2 scaleCenter) {
            return new MultiPolygon2D(
                _contours.Select(o => o.Scale(scaleFactor, scaleCenter)));
        }

        public IPolygon2D Scale(Vector2 scaleFactor) {
            return new MultiPolygon2D(
                _contours.Select(o => o.Scale(scaleFactor)));
        }

        public IPolygon2D Scale(Vector2 scaleFactor, Vector2 scaleCenter) {
            return new MultiPolygon2D(
                _contours.Select(o => o.Scale(scaleFactor, scaleCenter)));
        }

        public IPolygon2D Translate(Vector2 translateVector) {
            return new MultiPolygon2D(
                _contours.Select(o => o.Translate(translateVector)));
        }

        public IPolygon2D Rotate(float angle) {
            return new MultiPolygon2D(
                _contours.Select(o => o.Rotate(angle)));
        }

        public IPolygon2D Rotate(float angle, Vector2 rotateCenter) {
            return new MultiPolygon2D(
                _contours.Select(o => o.Rotate(angle, rotateCenter)));
        }

        public IPolygon2D Transform(Matrix2x2 transformMatrix) {
            return new MultiPolygon2D(
                _contours.Select(o => o.Transform(transformMatrix)));
        }

        public IPolygon2D Transform(Matrix2x2 transformMatrix, Vector2 translateVector) {
            return new MultiPolygon2D(
                _contours.Select(o => o.Transform(transformMatrix, translateVector)));
        }

        #endregion

        public IPolygon2D Offset(float thickness) {
            return new MultiPolygon2D(
                _contours.Select(o => o.Offset(thickness)));
        }

        public bool Contains(Vector2 point) {
            // for the point to be in this multipolygon it must be in
            // 1) ANY clockwise polygon
            // 2) NO counterclockwise polygon
            bool contains = false;
            foreach (var c in _contours) {
                if (c.Clockwise) {
                    contains |= c.Contains(point);
                } else {
                    if (c.Contains(point)) return false;
                }
            }
            return contains;
        }

        #region Debug Methods

        public void Draw() {
            _contours.ForEach(o => o.Draw());
        }

        public void Draw(Color32 color) {
            _contours.ForEach(o => o.Draw(color));
        }


        #endregion

        public bool Equals(IPolygon2D other) {
            var otherMulti = other as MultiPolygon2D;
            return (otherMulti != null) && _contours.SequenceEqual(otherMulti._contours);
        }

        #endregion
    }
}