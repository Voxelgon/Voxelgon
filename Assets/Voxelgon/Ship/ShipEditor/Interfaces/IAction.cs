
namespace Voxelgon.Ship.Editor {

    public interface IAction {
        void Redo();
        void Undo();
    }
}