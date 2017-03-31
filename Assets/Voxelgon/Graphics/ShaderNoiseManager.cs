using System;
using UnityEngine;

namespace Voxelgon.Graphics {

    [Serializable]
    public class ShaderNoiseManager : MonoBehaviour {

        // FIELDS

        [SerializeField]
        NoiseManagerEntry[] _entries;


        // METHODS

        public void Apply() {
            foreach (var e in _entries) {
                Shader.SetGlobalTexture(e.propName, e.texture);
            }
        }

        public void Start() {
            Apply();
        }


        // CLASSES

        [Serializable]
        private class NoiseManagerEntry {

            public Texture2D texture;
            public string propName;
        }
        
    }
}