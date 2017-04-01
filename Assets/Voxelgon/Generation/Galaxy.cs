using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxelgon.Util;
using Voxelgon.Util.Geometry;

namespace Voxelgon.Generation {

    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]

    public class Galaxy : MonoBehaviour {

        public float radius;
        [RangeAttribute(0, 1)]
        public float circlePerc;
        [RangeAttribute(0, 1)]
        public float distortion;
        public Color Skycolor;


        // Use this for initialization
        void Start() {
            var sphere = MeshFragment.TruncatedUVSphere(radius, 40, Color.blue, Vector3.zero, Vector3.up, circlePerc);
            var mesh = sphere.ToMesh();

            var size = mesh.vertexCount;
            var vertices = mesh.vertices;
            var texcoords = new Vector2[size];
            var colors = new Color[size];

            for (int i = 0; i < size; i++) {
                var rot = Random.rotation;
                rot = Quaternion.Lerp(Quaternion.identity, rot, distortion);
                var yOrig = vertices[i].y / (circlePerc * radius);
                vertices[i] = rot * vertices[i];
                var y = vertices[i].y;
                y *= -1; //hack to invert inwards
                y /= circlePerc * radius;
                y *= Mathf.Abs(y);
                y *= circlePerc * radius;
                vertices[i].y = y;
                texcoords[i].y = ((yOrig * yOrig) + 1f) / 2f;
            }

            mesh.vertices = vertices;
            mesh.uv = texcoords;

            GetComponent<MeshFilter>().mesh = mesh;
        }

        // Update is called once per frame
        void Update() {

        }
    }
}