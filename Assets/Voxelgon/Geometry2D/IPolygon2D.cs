using System.Collections.Generic;
using UnityEngine;

namespace Voxelgon.Geometry2D {
    public interface IPolygon2D {
        #region Properties

        /// <summary>
        /// if the polygon is convex
        /// </summary>
        bool IsConvex { get; }

        /// <summary>
        /// The area of the polygon
        /// </summary>
        float Area { get; }

        /// <summary>
        /// The number of vertices in the polygon
        /// </summary>
        int VertexCount { get; }

        /// <summary>
        /// The geometric center of the polygon
        /// </summary>
        Vector2 Center { get; }

        /// <summary>
        /// The vertices that make up the polygon
        /// </summary>
        IList<Vector2> Vertices { get; }

        /// <summary>
        /// The indices for the triangulated form of the polygon
        /// </summary>
        IEnumerable<int> Indices { get; }

        #endregion

        #region Methods

        #region Transformation Methods

        /// <summary>
        /// Scales a polygon around its center
        /// </summary>
        /// <param name="scaleFactor">amount to scale by</param>
        /// <returns>A clone of this polygon scaled about its center</returns>
        IPolygon2D Scale(float scaleFactor);

        /// <summary>
        /// Scales a polygon around a point
        /// </summary>
        /// <param name="scaleFactor">amount to scale by</param>
        /// <param name="scaleCenter">point to scale around</param>
        /// <returns>A clone of this polygon scaled about <c>scaleCenter</c></returns>
        IPolygon2D Scale(float scaleFactor, Vector2 scaleCenter);

        /// <summary>
        /// Scales a polygon around its center
        /// </summary>
        /// <param name="scaleFactor">amount to scale by</param>
        /// <returns>A clone of this polygon scaled about its center</returns>
        IPolygon2D Scale(Vector2 scaleFactor);

        /// <summary>
        /// Scales a polygon around a point
        /// </summary>
        /// <param name="scaleFactor">amount to scale by</param>
        /// <param name="scaleCenter">point to scale around</param>
        /// <returns>A clone of this polygon scaled about <c>scaleCenter</c></returns>
        IPolygon2D Scale(Vector2 scaleFactor, Vector2 scaleCenter);

        /// <summary>
        /// Translates a polygon
        /// </summary>
        /// <param name="translateVector">vector to translate by</param>
        /// <returns>A clone of this polygon translated by <c>translateVector</c></returns>
        IPolygon2D Translate(Vector2 translateVector);

        /// <summary>
        /// Rotates a polygon around its center
        /// </summary>
        /// <param name="angle">angle in radians, counterclockwise</param>
        /// <returns>A clone of this polygon rotated around its center</returns>
        IPolygon2D Rotate(float angle);

        /// <summary>
        /// Rotates a polygon around a point
        /// </summary>
        /// <param name="angle">angle in radians, counterclockwise</param>
        /// <param name="rotateCenter">point to rotate around</param>
        /// <returns>a clone of this polygon rotated around <c>rotateCenter</c></returns>
        IPolygon2D Rotate(float angle, Vector2 rotateCenter);

        /// <summary>
        /// Applies a transformation matrix on a polygon
        /// </summary>
        /// <param name="transformMatrix">transformation matrix</param>
        /// <returns>a clone of this polygon multiplied by the transformation matrix</returns>
        IPolygon2D Transform(Matrix2x2 transformMatrix);

        /// <summary>
        /// Applies a transformation matrix and translates a polygon
        /// </summary>
        /// <param name="transformMatrix">transformation matrix</param>
        /// <param name="translateVector">translation vector</param>
        /// <returns>a clone of this polygon multiplied by the transformation matrix and then translated</returns>
        IPolygon2D Transform(Matrix2x2 transformMatrix, Vector2 translateVector);

        #endregion

        IPolygon2D Offset(float thickness);
        IPolygon2D Offset(IList<float> thicknesses);

        #region Debug Methods

        /// <summary>
        /// Draw the Polygon in worldspace
        /// </summary>
        void Draw();

        /// <summary>
        /// Draw the Polygon in worldspace
        /// </summary>
        /// <param name="color">color to draw in</param>
        void Draw(Color32 color);

        #endregion

        /// <summary>
        /// Checks if the given point is inside the polygon
        /// </summary>
        /// <param name="point">point to check</param>
        /// <returns>if the point is inside the polygon</returns>
        bool Contains(Vector2 point);

        /// <summary>
        ///  Checks if the polygons are equal
        /// </summary>
        /// <param name="other">the polygon to compare against</param>
        /// <returns>if the polygons are equal</returns>
        bool Equals(IPolygon2D other);

        #endregion
    }
}