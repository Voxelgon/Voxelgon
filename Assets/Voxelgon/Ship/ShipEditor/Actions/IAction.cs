
namespace Voxelgon.Ship.Editor {

    public interface IAction {
        void Do();
        void Undo();
    }
}