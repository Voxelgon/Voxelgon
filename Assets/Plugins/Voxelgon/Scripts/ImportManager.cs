using UnityEngine;
using System.Collections;
using Voxelgon;

namespace Voxelgon {
    public class ImportManager : MonoBehaviour {

        public void Start() {
            Voxelgon.Asset.Setup();
            Voxelgon.Asset.Load();
            Voxelgon.Asset.Import();
        }
    }
}
