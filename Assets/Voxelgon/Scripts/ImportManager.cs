using UnityEngine;
using Voxelgon.Asset;
using System.Collections.Generic;

namespace Voxelgon {
    public class ImportManager : MonoBehaviour {

        public List<Material> _materials;
        public List<Mesh> _meshes;

        public void Start() {
            Filesystem.Setup();
            Filesystem.Import();
        }
    }
}
