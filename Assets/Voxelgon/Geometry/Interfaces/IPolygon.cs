using System.Collections.Generic;
using UnityEngine;


namespace Voxelgon.Geometry {

    public interface IPolygon {

        // PROPERTIES

        //access each vertex individually by its index
        Vector3 this[int index] { get; }

        //the normal of the clockwise polygon
        // if the polygon is invalid, return Vector3.zero
        Vector3 Normal { get; }

        //is the polygon convex?
        bool IsConvex { get; }

        //is the polygon valid?
        // must have >= 3 vertices
        bool IsValid { get; }

        //the area of the polygon
        float Area { get; }

        //the number of vertices in the polygon
        int VertexCount { get; }

        // METHODS

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

        //returns an array of triangles that make up the polygon
        List<Triangle> ToTriangles();

        //returns a polygon truncated starting at Vector3 `point` by vector3 `offset`
        Polygon Truncate(Vector3 point, Vector3 offset);
    }
}