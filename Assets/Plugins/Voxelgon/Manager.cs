using UnityEngine;
using System.Collections;

namespace Voxelgon {
    public class Manager : MonoBehaviour {

        public void Start() {
            Voxelgon.Asset.Import();
        }
    }
}
