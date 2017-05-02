using System.Collections.Generic;


namespace Voxelgon.Ship.Editor {

    public class UndoRedo {

        // FIELDS

        private Stack<IAction> _undoStack = new Stack<IAction>();
        private Stack<IAction> _redoStack = new Stack<IAction>();


        // CONSTRUCTORS

        public UndoRedo() {
            _undoStack = new Stack<IAction>();
            _redoStack = new Stack<IAction>();
        }


        // METHODS

        public bool Undo() {
            if (_undoStack.Count == 0) return false;

            var action = _undoStack.Pop();
            action.Undo();
            _redoStack.Push(action);
            return true;
        }

        public bool Redo() {
            if (_redoStack.Count == 0) return false;
            
            var action = _redoStack.Pop();
            action.Do();
            _undoStack.Push(action);
            return true;
        }

        public void Add(IAction action) {
            _undoStack.Push(action);
            _redoStack.Clear();
        }

        public void DoAndAdd(IAction action) {
            action.Do();
            Add(action);
        }
    }
}