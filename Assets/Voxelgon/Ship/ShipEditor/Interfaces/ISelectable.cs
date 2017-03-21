using Voxelgon.Util;
using UnityEngine;

namespace Voxelgon.Ship {

    public interface ISelectable : IBoundable {

        void MakeCollider(Transform parent);
        void DestroyCollider();

        GameObject Collider { get; }
    }
}