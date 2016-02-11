using System.Collections.Generic;
using UnityEngine;


namespace Voxelgon.Geometry {

    public interface IPolygon {

        // PROPERTIES

        //the normal of the clockwise polygon
        // if the polygon is invalid, return Vector3.zero
        Vector3 SurfaceNormal { get; }

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
        Polygon Reverse();

        //if the polygon is counter-clockwise, reverse it so it is clockwise
        Polygon EnsureClockwise(Vector3 normal);

        //returns an array of triangles that make up the polygon
        List<Triangle> ToTriangles();

        //returns a polygon truncated starting at Vector3 `point` by vector3 `offset`
        Polygon Truncate(Vector3 point, Vector3 offset);

        //returns the vertex at index `index`
        Vector3 GetVertex(int index);

        //returns the vertex normal at index `index`
        //same as mesh normal, usually close to parallel with plane normal
        Vector3 GetNormal(int index);

        //returns the vector pointing "out" of the vertex at index `index`
        //normalized average of two adjacent edge normals
        Vector3 GetVertexNormal(int index);

        //returns the edge vector at index `index`
        //normalized vector from a vertex to the following vertex
        Vector3 GetEdge(int index);

        //returns the edge normal at index `index`
        //cross product of plane normal and edge
        Vector3 GetEdgeNormal(int index);

        //returns a clone of this IPolygon
        Polygon Clone();


    }
}