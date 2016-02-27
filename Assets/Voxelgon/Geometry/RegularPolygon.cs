using UnityEngine;


namespace Voxelgon.Geometry {

    public class RegularPolygon : Polygon {

        // FIELDS

        internal readonly float _radius;

        internal readonly Vector3 _center;

        internal readonly Vector3 _normal;
        

        // CONSTRUCTORS

        //create a regular polygon with the given center, radius, sidecount, normal and optional tangent and color
        public RegularPolygon(Vector3 center, float radius, int sideCount, Vector3 normal, Vector3 tangent = default(Vector3), Color32? color = null)
            : base(new Vector3[sideCount], color: color) {

            _radius = radius;
            _center = center;
            _normal = normal;

            if (normal.Equals(Vector3.zero)) {
                normal = Vector3.forward;
            }

            var rotation = Quaternion.AngleAxis(360.0f / sideCount, normal);

            if (tangent.Equals(Vector3.zero)) {
                tangent = Vector3.Cross(Vector3.up, normal);
            }

            if (tangent.Equals(Vector3.zero)) {
                tangent = Vector3.Cross(Vector3.forward, normal);
            }

            for (int i = 0; i < sideCount; i++) {
                _vertices[i] = center + (tangent * radius);
                _normals[i] = normal;
                tangent = rotation * tangent;
            }
        }


        // PROPERTIES

        //the radius of the regular polygon
        public float Radius { 
            get { return _radius; }
        }

        //the normal of the clockwise polygon
        //if the polygon is invalid, return Vector3.zero
        public Vector3 SurfaceNormal {
            get { return _normal; }
        }

    }
}