using System.Collections;
using System.Collections.Generic;

namespace Voxelgon.Collections {

    public class ReadOnlyEnumerable<T> : IEnumerable<T> {

        // FIELDS

        private IEnumerable<T> _enumerable;


        // CONSTRUCTORS

        public ReadOnlyEnumerable(IEnumerable<T> enumerable) {
            _enumerable = enumerable;
        }


        // METHODS

        public IEnumerator<T> GetEnumerator() {
            return _enumerable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _enumerable.GetEnumerator();
        }
    }
}