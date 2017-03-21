
namespace Voxelgon.Ship.Editor {

    public interface IComplexAction {
        void LocalRedo();
        void LocalUndo();
    }
}