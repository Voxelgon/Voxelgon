using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Voxelgon.Collections {
    public sealed class SequencialList<T> : IList<T>, IList {
        #region Fields

        private readonly int _length;
        private readonly T _constant;

        #endregion

        #region Constructor

        public SequencialList(int length, T constant) {
            _length = length;
            _constant = constant;
        }

        #endregion

        #region Public Methods

        void ICollection<T>.Add(T item) {
            throw new NotSupportedException();
        }

        void ICollection<T>.Clear() {
            throw new NotSupportedException();
        }

        public bool Contains(T value) {
            return (value.Equals(_constant));
        }

        public void CopyTo(T[] array, int index) {
            if (array.Length - index < _length) throw new ArgumentException("not enough room in array");
            for (var i = 0; i < _length; i++) {
                array[index + i] = _constant;
            }
        }

        public IEnumerator<T> GetEnumerator() {
            for (var i = 0; i < _length; i++) {
                yield return _constant;
            }
        }

        public int IndexOf(T value) {
            if (value.Equals(_constant)) {
                return 0;
            } else {
                return -1;
            }
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
            get { return _constant; }
            set { throw new NotSupportedException(); }
        }

        T IList<T>.this[int index] {
            get { return _constant; }
            set { throw new NotSupportedException(); }
        }

        bool ICollection<T>.IsReadOnly {
            get { return true; }
        }

        #endregion

        #region Non-Generic Public Methods

        void ICollection.CopyTo(Array array, int index) {
            var casted = array as T[];
            if (casted != null) CopyTo(casted, index);
            else throw new ArgumentException();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            for (var i = 0; i < _length; i++) {
                yield return _constant;
            }
        }

        int IList.Add(object value) {
            throw new NotSupportedException();
        }

        void IList.Clear() {
            throw new NotSupportedException();
        }

        bool IList.Contains(object value) {
            return Contains((T) value);
        }

        int IList.IndexOf(object value) {
            return IndexOf((T) value);
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

        #endregion
    }
}