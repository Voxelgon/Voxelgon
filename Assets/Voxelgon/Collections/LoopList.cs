using System.Collections.Generic;
using System;

namespace Voxelgon.Collections {

    public class LoopList<T> : ICollection<T>, System.Collections.ICollection {

        // FIELDS

        private Dictionary<T, Node> _dictionary;
        private Node _head;
        private object _syncRoot;


        // PROPERTIES

        public int Count {
            get { return _dictionary.Count; }
        }

        public T First {
            get { return _head.item; }
        }

        public T Last {
            get { return _head.prev.item; }
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

        public LoopList() {
            _dictionary = new Dictionary<T, Node>();
        }

        public LoopList(IEnumerable<T> values) : base() {
            foreach (var v in values) {
                Add(v);
            }
        }


        // METHODS

        public void Add(T value) {
            if (_dictionary.Count == 0 || _head == null) {
                _head = new Node(value);
                _dictionary.Add(value, _head);
            } else {
                var newNode = new Node(value, _head.prev, _head);
                _head.prev.next = newNode;
                _head.prev = newNode;
                _dictionary.Add(value, newNode);
            }
        }

        public void AddAfter(T obj, T value) {
            var node = _dictionary[obj];
            var newNode = new Node(value, node, node.next);
            node.next.prev = newNode;
            node.next = newNode;
            _dictionary.Add(value, newNode);
        }

        public void AddBefore(T obj, T value) {
            var node = _dictionary[obj];
            var newNode = new Node(value, node.prev, node);
            node.prev.next = newNode;
            node.prev = newNode;
            _dictionary.Add(value, newNode);
        }

        public T GetNext(T obj) {
            return _dictionary[obj].next.item;
        }

        public T GetPrevious(T obj) {
            return _dictionary[obj].prev.item;
        }

        public bool Remove(T value) {
            var node = _dictionary[value];
            if (node != null) {
                _dictionary.Remove(value);
                node.prev.next = node.next;
                node.next.prev = node.prev;
                return true;
            }
            return false;
        }

        public void Clear() {
            _dictionary.Clear();
        }

        public bool Contains(T value) {
            return _dictionary.ContainsKey(value);
        }

        public void ForEach(Action<T> action) {
            if (action == null) throw new ArgumentNullException("action");

            var node = _head;
            if (node != null) {
                do {
                    action(node.item);
                    node = node.next;
                } while (node != _head);
            }
        }

        public void CopyTo(T[] array, int index) {
            if (array == null) throw new ArgumentNullException("array");

            if (index < 0 || index > array.Length) throw new ArgumentOutOfRangeException("index", "index out of range");

            if (array.Length - index < Count) throw new ArgumentException("insufficient space for copy");

            Node node = _head;
            if (node != null) {
                do {
                    array[index++] = node.item;
                    node = node.next;
                } while (node != _head);
            }
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

        public Enumerator GetEnumerator() {
            return new Enumerator(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            return GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }


        // CLASSES

        public struct Enumerator : IEnumerator<T>, System.Collections.IEnumerator {

            // FIELDS

            private Node _head;
            private Node _node;
            private T _current;


            // CONSTRUCTORS

            internal Enumerator(LoopList<T> list) {
                _head = list._head;
                _node = list._head;
                _current = default(T);
            }


            // PROPERTIES

            public T Current {
                get { return _current; }
            }

            object System.Collections.IEnumerator.Current {
                get { return _current; }
            }


            // METHODS

            public bool MoveNext() {
                _current = _node.item;
                _node = _node.next;

                if (_node == null || _node == _head) {
                    return false;
                }

                return true;
            }

            void System.Collections.IEnumerator.Reset() {
                _current = default(T);
                _node = _head;
            }

            public void Dispose() { }
        }

        private class Node {

            // FIELDS

            public Node prev;
            public Node next;

            public readonly T item;


            // CONSTRUCTORS

            public Node(T value) {
                this.prev = this.next = this;

                this.item = value;
            }

            public Node(T value, Node prev, Node next) {
                this.prev = prev;
                this.next = next;

                this.item = value;
            }
        }
    }
}