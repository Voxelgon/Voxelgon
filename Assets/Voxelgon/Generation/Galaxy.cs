using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxelgon.Util;
using Voxelgon.Util.MeshGeneration;
using Voxelgon.Util.Geometry;
using CoherentNoise.Generation.Fractal;

namespace Voxelgon.Generation {

    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]

    public class Galaxy : MonoBehaviour {

        public float radius;
        [RangeAttribute(0, 1)]
        public float height;
        [RangeAttribute(0, 1)]
        public float distortion;
        public int resolution;

        public float cloudScale;
        public float cloudLayerDistance;

        public int cloudOctaves;

        public Color[] cloudColors;

        // Use this for initialization
        void Start() {
            var sphere = MeshFragment.TruncatedUVSphere(radius, resolution, Color.blue, Vector3.zero, Vector3.up, height);
            var mesh = sphere.ToMesh();

            var size = mesh.vertexCount;
            var vertices = mesh.vertices;
            var texcoords = new Vector2[size];
            var colors = new Color[size];

            var clouds = new PinkNoise(42);
            clouds.OctaveCount = cloudOctaves;

            for (int i = 0; i < size; i++) {
                var pos2d = (vertices[i] / radius).xz() * cloudScale;
                vertices[i] = Quaternion.Lerp(Quaternion.identity, UnityEngine.Random.rotation, distortion) * vertices[i];
                var y = vertices[i].y / (radius * height);
                y *= Mathf.Abs(y);
                vertices[i].y = y * (radius * height);

                for (int c = 0; c < cloudColors.Length; c++) {
                    var colorScale = (0.1f + 0.8f * Mathf.Clamp01((clouds.GetValue(pos2d.x, pos2d.y, c * cloudLayerDistance) + 1) * 0.5f));
                    var colorThickness = Mathf.Sqrt(0.1f + 0.8f * Mathf.Clamp01((clouds.GetValue(pos2d.x, pos2d.y, -c * cloudLayerDistance) + 1) * 0.5f));  
                    colors[i] += Mathf.Clamp01(1 - Mathf.Abs(y) / colorThickness) * cloudColors[c] * MathVG.SmoothStep(colorScale);
                }

                var scale = 0.1f + 0.8f * Mathf.Clamp01((clouds.GetValue(pos2d.x, pos2d.y, -3 * cloudLayerDistance) + 1) * 0.5f);

                vertices[i].y *= scale - 2;
                texcoords[i].y = ((y) + 1f) / 2f;
            }

            mesh.vertices = vertices;
            mesh.uv = texcoords;
            mesh.colors = colors;

            GetComponent<MeshFilter>().mesh = mesh;
        }

        // Update is called once per frame
        void Update() {

        }
    }
}