using UnityEngine;

namespace Voxelgon.Geometry2D {
    public struct Bounds2D {
        #region Fields

        private Vector2 _min;
        private Vector2 _max;

        #endregion

        #region Constructors

        public Bounds2D(Vector2 p1, Vector2 p2) {
            _min.x = Mathf.Min(p1.x, p2.x);
            _min.y = Mathf.Min(p1.y, p2.y);

            _max.x = Mathf.Max(p1.x, p2.x);
            _max.y = Mathf.Max(p1.y, p2.y);
        }

        #endregion

        #region Properties

        public Vector2 Min {
            get { return _min; }
            set { _min = value; }
        }

        public Vector2 Max {
            get { return _max; }
            set { _max = value; }
        }

        #endregion

        #region Methods

        public bool Contains(Vector2 point) {
            return point.x > _min.x && point.x < _max.x &&
                   point.y > _min.y && point.y < _max.y;
        }

        #endregion

    }
}