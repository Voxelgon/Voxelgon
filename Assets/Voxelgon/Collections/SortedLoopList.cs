using System.Collections.Generic;
using System.Linq;
using System;

namespace Voxelgon.Collections {

    public class SortedLoopList<T> : ICollection<T>, System.Collections.ICollection {

        // FIELDS

        protected LoopList<T> _loopList;
        protected IComparer<T> _comparer;

        private object _syncRoot;


        // PROPERTIES

        public int Count {
            get { return _loopList.Count; }
        }

        public T First {
            get { return _loopList.First; }
        }

        public T Last {
            get { return _loopList.Last; }
        }

        bool System.Collections.ICollection.IsSynchronized {
            get { return false; }
        }

        object System.Collections.ICollection.SyncRoot {
            get {
                if (_syncRoot == null) {
                    System.Threading.Interlocked.CompareExchange<Object>(ref _syncRoot, new Object(), null);
                }
                return _syncRoot;
            }
        }

        bool ICollection<T>.IsReadOnly {
            get { return false; }
        }


        // CONSTRUCTORS

        public SortedLoopList() {
            _loopList = new LoopList<T>();
            _comparer = default(Comparer<T>);
        }

        public SortedLoopList(IComparer<T> comparer) {
            _loopList = new LoopList<T>();
            _comparer = comparer;
        }

        public SortedLoopList(IEnumerable<T> collection) : base() {
            foreach (var v in collection) {
                Add(v);
            }
        }

        public SortedLoopList(IEnumerable<T> collection, IComparer<T> comparer) : this(comparer) {
            foreach (var v in collection) {
                Add(v);
            }
        }


        // METHODS

        public void Add(T value) {
            T last;
            try {
                last = _loopList.Last(v => _comparer.Compare(v, value) <= 0);
            } catch(InvalidOperationException) {
                // looplist is empty, or has nothing that comes before value
                // in either case, add the the beginning of the loopList
                _loopList.Add(value);
                return;
            }

            if (last != null) _loopList.AddAfter(last, value);
        }

        public T GetNext(T obj) {
            return _loopList.GetNext(obj);
        }

        public T GetPrevious(T obj) {
            return _loopList.GetPrevious(obj);
        }

        public bool Remove(T value) {
            return _loopList.Remove(value);
        }

        public void Clear() {
            _loopList.Clear();
        }

        public bool Contains(T value) {
            return _loopList.Contains(value);
        }

        public void ForEach(Action<T> action) {
            _loopList.ForEach(action);
        }

        public void CopyTo(T[] array, int index) {
            _loopList.CopyTo(array, index);
        }

        void System.Collections.ICollection.CopyTo(Array array, int index) {
            if (array == null) throw new ArgumentNullException("array");

            if (array.Rank != 1) throw new ArgumentException("Array is not rank 1");

            if (array.GetLowerBound(0) != 0) throw new ArgumentException("Array has non-zero lower bound");

            if (index < 0) throw new ArgumentOutOfRangeException("index", "index out of range");

            if (array.Length - index < Count) throw new ArgumentException("insufficient space for copy");

            T[] tArray = array as T[];
            if (tArray != null) {
                CopyTo(tArray, index);
            } else {
                throw new ArgumentException("invalid array type");
            }
        }

        public LoopList<T>.Enumerator GetEnumerator() {
            return _loopList.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            return GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}