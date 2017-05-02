using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Voxelgon.Collections {

    public sealed class SequencialList : IList<int>, IList, ICollection {

        // FIELDS

        private int _length;


        // CONSTRUCTOR

        public SequencialList(int length) {
            _length = length;
        }


        // METHODS

        void ICollection<int>.Add(int item) {
            throw new NotSupportedException();
        }

        void ICollection<int>.Clear() {
            throw new NotSupportedException();
        }

        public bool Contains(int value) {
            return (value >= 0 && value < _length);
        }

        public void CopyTo(int[] array, int index) {
            if (array.Length - index < _length) throw new ArgumentException("not enough room in array");
            for (var i = 0; i < _length; i++) {
                array[index + i] = i;
            }
        }

        public IEnumerator<int> GetEnumerator() {
            for (var i = 0; i < _length; i++) {
                yield return i;
            }
        }

        public int IndexOf(int value) {
            if (Contains(value)) {
                return value;
            } else {
                return -1;
            }
        }

        void IList<int>.Insert(int index, int item) {
            throw new NotSupportedException();
        }

        bool ICollection<int>.Remove(int item) {
            throw new NotSupportedException();
        }

        void IList<int>.RemoveAt(int index) {
            throw new NotSupportedException();
        }

        public int Count {
            get { return _length; }
        }

        public int this[int index] {
            get { return index; }
        }

        int IList<int>.this[int index] {
            get { return index; }
            set { throw new NotSupportedException(); }
        }

        bool ICollection<int>.IsReadOnly {
            get { return true; }
        }

        // NON-GENERIC INTERFACE IMPLEMENTATIONS

        void ICollection.CopyTo(Array array, int index) {
            var intArray = array as int[];
            if (intArray == null) throw new ArgumentException("Array is not of type 'int'");
            if (array.Length - index < _length) throw new ArgumentException("not enough room in array");
            for (var i = 0; i < _length; i++) {
                intArray[index + i] = i;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            for (var i = 0; i < _length; i++) {
                yield return i;
            }
        }

        int IList.Add(object value) {
            throw new NotSupportedException();
        }

        void IList.Clear() {
            throw new NotSupportedException();
        }

        bool IList.Contains(object value) {
            return Contains((int)value);
        }

        int IList.IndexOf(object value) {
            return IndexOf((int)value);
        }

        void IList.Insert(int index, object value) {
            throw new NotSupportedException();
        }

        void IList.Remove(object value) {
            throw new NotSupportedException();
        }

        void IList.RemoveAt(int index) {
            throw new NotSupportedException();
        }

        bool ICollection.IsSynchronized {
            get { return false; }
        }

        object ICollection.SyncRoot {
            get { return this; }
        }

        bool IList.IsFixedSize {
            get { return true; }
        }

        bool IList.IsReadOnly {
            get { return true; }
        }

        object IList.this[int index] {
            get { return index; }
            set { throw new NotSupportedException(); }
        }
    }
}