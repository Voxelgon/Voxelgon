using UnityEngine;
using System.Collections.Generic;

namespace Voxelgon.Asset {
    public static class AssetDatabase {

        // FIELDS

        private static Dictionary<string, Material> _materialMap = new Dictionary<string, Material>();
        private static Dictionary<string, Mesh>     _meshMap     = new Dictionary<string, Mesh>();
        private static Dictionary<string, Part>     _partMap     = new Dictionary<string, Part>();


        // METHODS

        public static void AddMaterial(string key, Material value) {
            _materialMap.Add(key, value);
        }     

        public static Material GetMaterial(string key) {
            if (!_materialMap.ContainsKey(key)) {
                return _materialMap["Standard"];
            }
            return _materialMap[key];
        }


        public static void AddMesh(string key, Mesh value) {
            _meshMap.Add(key, value);
        }

        public static Mesh GetMesh(string key) {
            Mesh mesh;
            try {
                mesh = _meshMap[key];
            } catch (KeyNotFoundException e) {
                Debug.LogWarning("Could not find a mesh matching \"" + key + "\"");
                return _meshMap["Error"];
            }

            return mesh;
        }


        public static void AddPart(string key, Part value) {
            _partMap.Add(key, value);
        }

        public static Part GetPart(string key) {
            return _partMap[key];
        }

        public static void Populate() {
            Mesh[] models = Resources.LoadAll<Mesh>("Models");
            Material[] materials = Resources.LoadAll<Material>("Materials");

            foreach (Mesh m in models) {
                AddMesh(m.name, m);
            }

            foreach (Material m in materials) {
                AddMaterial(m.name, m);
            }

        }
    }
}