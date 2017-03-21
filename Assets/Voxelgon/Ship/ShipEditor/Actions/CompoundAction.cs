using System.Collections.Generic;

namespace Voxelgon.Ship.Editor {

    public class CompoundAction : IAction {

        // FIELDS

        private List<IAction> _actions;


        // CONSTRUCTOR

        public CompoundAction(IEnumerable<IAction> actions) {
            _actions = actions as List<IAction>;
        }


        // METHODS

        public void Redo() {
            _actions.ForEach(a => a.Redo());
        }

        public void Undo() {
            var inverse = new List<IAction>(_actions);
            inverse.Reverse();
            inverse.ForEach(a => a.Undo());
        }
    }
}