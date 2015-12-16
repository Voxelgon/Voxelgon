using UnityEngine;

namespace Voxelgon.Geometry {
    public interface IPolygon {

        //PROPERTIES

        //access each vertex individually by its index
        Vector3 this[int index] { get; set; }

        //the normal of the clockwise polygon
        Vector3 Normal { get; }

        //the area of the polygon
        float Area { get; }

        //the number of vertices in the polygon
        int VertexCount { get; }

        //METHODS

        //returns the winding order relative to the normal
        // 1 = clockwise
        //-1 = counter-clockwise
        // 0 = all points are colinear
        int WindingOrder(Vector3 normal);

        //returns whether or not `point` is on or inside the polygon
        bool Contains(Vector3 point);

        //reverses the polygon's winding order
        void Reverse();

        //if the polygon is counter-clockwise, reverse it so it is clockwise
        void EnsureClockwise(Vector3 normal);
    }
}