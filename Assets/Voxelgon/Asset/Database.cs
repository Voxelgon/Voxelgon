using UnityEngine;
using System.Collections.Generic;

namespace Voxelgon.Asset {
    public static class Database {

        // FIELDS

        private static Dictionary<string, Material> _materialMap = new Dictionary<string, Material>();
        private static Dictionary<string, Mesh>     _meshMap     = new Dictionary<string, Mesh>();
        private static Dictionary<string, Part>     _partMap     = new Dictionary<string, Part>();


        // METHODS

        public static void AddMaterial(string key, Material value) {
            _materialMap.Add(key, value);
        }     

        public static Material GetMaterial(string key) {
            return _materialMap[key];
        }


        public static void AddMesh(string key, Mesh value) {
            _meshMap.Add(key, value);
        }

        public static Mesh GetMesh(string key) {
            return _meshMap[key];
        }


        public static void AddPart(string key, Part value) {
            _partMap.Add(key, value);
        }

        public static Part GetPart(string key) {
            return _partMap[key];
        }
    }
}