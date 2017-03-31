using System;
using System.Collections;
using System.Collections.Generic;

namespace Voxelgon.Collections {

    class DropOutStack<T> : IEnumerable<T> {

        // FIELDS

        private int _capacity;
        private int _count;
        private int _top = 0;
        private T[] _items;


        // PROPERTIES

        public T this[int index] {
            get { return Peek(index); }
        }

        public int Capacity {
            get { return _capacity; }
        }

        public int Count {
            get { return _count; }
        }


        // CONSTRUCTORS

        public DropOutStack(int capacity) {
            _capacity = capacity;
            _count = 0;
            _top = 0;
            _items = new T[capacity];
        }


        // METHODS

        public void Push(T item) {
            _items[_top] = item;
            _top = (_top + 1) % _capacity;

            if (_count != _capacity) {
                _count++;
            }
        }

        public T Pop() {
            if (_count == 0) throw new InvalidOperationException();
            _top = (_capacity + _top - 1) % _capacity;

            var result = _items[_top];

            _items[_top] = default(T);
            _count--;

            return result;
        }

        public T Peek() {
            if (_count == 0) throw new InvalidOperationException();

            return _items[_top - 1];
        }

        public T Peek(int index) {
            if (_count == 0) throw new InvalidOperationException();
            if (index >= _count) throw new IndexOutOfRangeException();

            return _items[(_top - 1 - index + _capacity) % _capacity];
        }

        public void Clear() {
            for (var i = 0; i < _count; i++) {
                Pop();
            }
        }

        public int IndexOf(T item) {
            for (var i = 0; i < _count; i++) {
                if (Pop().Equals(item)) return i;
            }
            return -1;
        }

        public IEnumerator<T> GetEnumerator() {
            for (var i = 0; i < _count; i++) {
                yield return Peek(i);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}