using UnityEngine;
using System.Collections.Generic;
using Voxelgon.Util.Geometry;

namespace Voxelgon.Util.MeshGeneration {

    public class UVSphere : MeshFragment{

        // CONSTRUCTORS

        public UVSphere(float radius, int resolution, Color32 color32, Vector3 center, Vector3 axis) {
            axis.Normalize();
            var builder = new MeshBuilder();
            var profile = new Polygon(center, radius, resolution, axis);
            var stepcount = resolution / 4;
            var profcount = (stepcount * 2) + 1;
            var points = new Vector3[profcount];
            var tangents = new Vector3[profcount];
            var scales = new float[profcount];

            points[stepcount] = center;
            scales[stepcount] = 1.0f;
            for (var i = 1; i <= stepcount; i++) {
                float angle = ((float)i / stepcount) * Mathf.PI / 2;
                var cos = Mathf.Cos(angle);
                var sin = Mathf.Sin(angle);

                scales[stepcount + i] = cos;
                scales[stepcount - i] = cos;
                tangents[stepcount + i] = axis;
                tangents[stepcount - i] = axis;
                points[stepcount + i] = center + (sin * axis * radius);
                points[stepcount - i] = center - (sin * axis * radius);
            }

            var path = new Path(points, tangents, scales);

            builder.Sweep(profile, path, color32, false, true, true);
        }
    }
}