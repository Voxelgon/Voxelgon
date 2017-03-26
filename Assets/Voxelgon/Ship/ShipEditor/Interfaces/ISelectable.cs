using Voxelgon.Util;
using UnityEngine;

namespace Voxelgon.Ship {

    public interface ISelectable : IBoundable {

        void MakeSelector(Transform parent);
        void DestroySelector();

        GameObject Selector { get; }
    }
}