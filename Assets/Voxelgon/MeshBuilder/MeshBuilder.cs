using UnityEngine;
using System.Collections.Generic;

namespace Voxelgon.MeshBuilder {
    public class MeshBuilder {
        private List<CompoundMesh> _compoundMeshes;

        public MeshBuilder() {
            _compoundMeshes = new List<CompoundMesh>();
        }
    }
}