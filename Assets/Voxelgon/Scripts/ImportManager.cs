using UnityEngine;
using Voxelgon.Asset;

namespace Voxelgon {
    public class ImportManager : MonoBehaviour {

        public void Start() {
            Filesystem.Setup();
            Filesystem.Import();
        }
    }
}
