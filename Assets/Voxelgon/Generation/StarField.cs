using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxelgon.Util.MeshGeneration;
using Voxelgon.Geometry;

namespace Voxelgon.Generation {

    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]

    public class StarField : MonoBehaviour {

        // FIELDS

        public float radius;
        public float minSize;
        public float maxSize;
        public AnimationCurve sizeCurve;
        public Gradient colorCurve;
        [RangeAttribute(1, 21844)]
        public int starCount;

        [SpaceAttribute]

        public float intensityMultiplier;
        [RangeAttribute(0, 1)]
        public float intensityCutoff;


        // Use this for initialization
        void Start() {
            GenerateStars(RandomStars());
        }

        // Update is called once per frame
        void Update() {

        }

        void GenerateStars(Star[] stars) {
            var triangle = new Polygon(Vector3.up * radius, 1, 3, -Vector3.up);
            var triVerts = triangle.Vertices;

            var filter = GetComponent<MeshFilter>();
            var builder = new MeshBuilder(starCount * 3);

            foreach(var s in stars) {
                var scale = new Vector3(s.size, 1, s.size);
                var rotation = Quaternion.FromToRotation(Vector3.up, s.position);
                var intensity = Mathf.Clamp01(intensityMultiplier * s.intensity * (radius / s.position.sqrMagnitude));
                if (intensity < intensityCutoff) break;
                var v0 = triVerts[0];
                var v1 = triVerts[1];
                var v2 = triVerts[2];

                v0.Scale(scale);
                v1.Scale(scale);
                v2.Scale(scale);

                v0 = rotation * v0;
                v1 = rotation * v1;
                v2 = rotation * v2;

                var color = s.color;
                color.a = intensity;
                //color.a *= radius / s.position.sqrMagnitude;
                //color.a *= 1 - Mathf.Clamp01((Mathf.Abs(v0.y) / radius) - 0.5f);

                builder.AddTriangle(v0, v1, v2, color);
            }

            filter.mesh = builder.FirstMesh;
        }

        Star[] RandomStars() {
            var stars = new Star[starCount]; 

            for(var i = 0; i < starCount; i++) {
                stars[i].size = Mathf.Lerp(minSize, maxSize, sizeCurve.Evaluate(UnityEngine.Random.value));
                var position = UnityEngine.Random.insideUnitCircle.xz() * radius;
                var y = 2 * UnityEngine.Random.value - 1;
                y = y * y * y * radius;
                y += (radius / 10) * (UnityEngine.Random.value - 0.5f);
                y = Mathf.Clamp(y, -radius, radius);
                position.y = y;
                stars[i].position = position;
                stars[i].color = colorCurve.Evaluate(UnityEngine.Random.value);
                stars[i].intensity = 1;//UnityEngine.Random.value;
            }

            return stars;

        }

        private struct Star{
            public float size;
            public float intensity;
            public Vector3 position;
            public Color color;
        }
    }
}
