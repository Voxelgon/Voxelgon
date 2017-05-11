using System;
using System.Collections.Generic;
using System.Collections;

namespace Voxelgon.Collections {
    public class Indexer<T> : IList<T>, IList{

        #region Fields

        private Func<int, T> _expression;
        private int _length;

        #endregion

        #region Constructors

        public Indexer(Func<int, T> expression, int length) {
            _length = length;
            _expression = expression;
        }

        #endregion

        void ICollection<T>.Add(T item) {
            throw new NotSupportedException();
        }

        void ICollection<T>.Clear() {
            throw new NotSupportedException();
        }

        public bool Contains(T value) {
            return IndexOf(value) != -1;
        }

        public void CopyTo(T[] array, int index) {
            if (array.Length - index < _length) throw new ArgumentException("not enough room in array");
            for (var i = 0; i < _length; i++) {
                array[index + i] = _expression(i);
            }
        }

        public IEnumerator<T> GetEnumerator() {
            for (var i = 0; i < _length; i++) {
                yield return _expression(i);
            }
        }

        public int IndexOf(T value) {
            for (var i = 0; i < _length; i++) {
                if (_expression(i).Equals(value)) return i;
            }
            return -1;
        }

        void IList<T>.Insert(int index, T item) {
            throw new NotSupportedException();
        }

        bool ICollection<T>.Remove(T item) {
            throw new NotSupportedException();
        }

        void IList<T>.RemoveAt(int index) {
            throw new NotSupportedException();
        }

        public int Count {
            get { return _length; }
        }

        public T this[int index] {
            get { return _expression(index); }
            set { throw new NotSupportedException(); }
        }

        T IList<T>.this[int index] {
            get { return _expression(index); }
            set { throw new NotSupportedException(); }
        }

        bool ICollection<T>.IsReadOnly {
            get { return true; }
        }

        // NON-GENERIC INTERFACE IMPLEMENTATIONS

        void ICollection.CopyTo(Array array, int index) {
            var casted = array as T[];
            if (casted == null) throw new ArgumentException("Array is not of type 'int'");
            if (casted.Length - index < _length) throw new ArgumentException("not enough room in array");
            for (var i = 0; i < _length; i++) {
                casted[index + i] = this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            for (var i = 0; i < _length; i++) {
                yield return this[i];
            }
        }

        int IList.Add(object value) {
            throw new NotSupportedException();
        }

        void IList.Clear() {
            throw new NotSupportedException();
        }

        bool IList.Contains(object value) {
            return Contains((T)value);
        }

        int IList.IndexOf(object value) {
            return IndexOf((T)value);
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
            get { return _expression(index); }
            set { throw new NotSupportedException(); }
        }
    }
}